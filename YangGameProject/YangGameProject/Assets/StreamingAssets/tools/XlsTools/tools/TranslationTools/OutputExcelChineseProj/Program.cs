using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;

namespace OutputExcelChineseProj
{
    class Program
    {
        static string  xlsFolderPath = string.Empty;
        static string jsonOutputPath = string.Empty;
        static string sheetJsonPath = string.Empty;
        static List<string> blackList = new List<string>();
        static void Main(string[] args)
        {
            string cfgPath = ".\\tools\\TranslationTools\\OutputExcelChineseProj\\bin\\Debug\\net5.0\\PathCfg.txt";

            if (File.Exists(cfgPath))
            {
                using (StreamReader sr = new StreamReader(cfgPath))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        string info = sr.ReadLine();
                        if (!string.IsNullOrEmpty(info))
                        {
                            if (i == 0)
                            {
                                xlsFolderPath = info.Replace(@"\", "/");
                            }
                            else if (i == 1)
                            {
                                jsonOutputPath = info.Replace(@"\", "/");
                            }
                            else if (i == 2)
                            {
                                sheetJsonPath = info.Replace(@"\", "/");
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(info)) continue;
                                string[] splits = info.Split('|');
                                blackList.AddRange(splits);
                            }
                        }
                        else
                        {
                            Console.WriteLine(" ./PathCfg.txt 请确保PathCfg.txt 文件存在");
                            Console.ReadLine();
                            return;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("配置文件路径不存在");
            }

            if (string.IsNullOrEmpty(xlsFolderPath))
            {
                Console.WriteLine("目标文件夹不存在，请读“使用必看.txt”");
                Console.ReadLine();
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<SheetInfo> sheetList = GetSheetListFromDisk();
            List<string> currentSheetNameList = new List<string>();
            List<CellInfo> cellInfos = new List<CellInfo>();
            Dictionary<string, List<CellInfo>> cellDic = new Dictionary<string, List<CellInfo>>();
            Console.WriteLine("读表，收集数据....");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<ExcelPackage> packages = GetAllXls();
            CreateSaveFile();
            for (int i = 0; i < packages.Count; i++)
            {
                ProcessSheetList(packages[i], currentSheetNameList);
                SetInfo(packages[i], cellInfos);
            }
            ProcessSheetListFinal(sheetList, currentSheetNameList);

            for (int i = 0; i < cellInfos.Count; i++)
            {
                string key = cellInfos[i].cellContent;
                if (cellDic.ContainsKey(key))
                {
                    cellDic[key].Add(cellInfos[i]);
                }
                else
                {
                    cellDic.Add(key, new List<CellInfo>() { cellInfos[i] });
                }
            }

            Console.WriteLine("转成json");
            string jsonStr = JsonConvert.SerializeObject(cellDic);
            Console.WriteLine("转换完成，写入文本");
            using (StreamWriter sw = new StreamWriter(jsonOutputPath, false, Encoding.UTF8))
            {
                sw.Write(jsonStr);
            }
            SaveSheetList(sheetList);
            Console.WriteLine("写入完成，文件已生成");
            stopwatch.Stop();
            Console.WriteLine($"消耗时间:{(((double)stopwatch.ElapsedMilliseconds) / 1000)}秒");
        }


        /// <summary>
        /// 得到配置路径下所有的xls
        /// </summary>
        /// <returns></returns>
        private static List<ExcelPackage> GetAllXls()
        {
            if (!Directory.Exists(xlsFolderPath))
            {
                Console.WriteLine("路径不存在");
                Console.ReadLine();
                return null;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(xlsFolderPath);
            FileInfo[] fileInfos = directoryInfo.GetFiles();
            int len = fileInfos.Length;

            //筛选出符合条件的文件
            List<FileInfo> tempLists = new List<FileInfo>();
            for (int i = 0; i < len; i++)
            {
                if (fileInfos[i].Extension != ".xlsx") break;
                tempLists.Add(fileInfos[i]);
            }

            List<ExcelPackage> packages = new List<ExcelPackage>();
            for (int i = 0; i < tempLists.Count; i++)
            {
                packages.Add(new ExcelPackage(tempLists[i]));

            }

            return packages;
        }

        private static void SetInfo(ExcelPackage package, List<CellInfo> cellInfos)
        {
            var sheets = package.Workbook.Worksheets;
            for (int i = 0; i < sheets.Count; i++)
            {
                OutputChineseToTxt(package.File.Name, sheets[i], cellInfos);
            }
        }

        private static void OutputChineseToTxt(string workPackageName, ExcelWorksheet worksheet, List<CellInfo> cellInfos)
        {
            if (worksheet.Name.StartsWith("#")) return;

            for (int i = 0; i < blackList.Count; i++)
            {
                if (worksheet.Name == blackList[i]) return;
            }

            CellInfo cellInfo;
            for (int j = 1; j <= worksheet.Dimension.Columns; ++j)
            {
                if (string.IsNullOrEmpty(worksheet.Cells[1, j].Text)) break;
                if(worksheet.Cells[2, j].Text.EndsWith("TR"))
                {
                    var isArray = worksheet.Cells[3, j].Text.EndsWith("*");
                    for (int i = 4; i <= worksheet.Dimension.Rows; ++i)
                    {
                        string txt = worksheet.Cells[i, j].Text;
                        if (string.IsNullOrEmpty(txt)) continue;
                        //if (Regex.IsMatch(txt, @"[\u4e00-\u9fbb]+"))
                        {
                            //处理字符数组的情况
                            List<string> strList = new List<string>();
                            if(isArray)
                            {
                                strList.AddRange(txt.Split(';'));
                            }
                            else
                            {
                                strList.Add(txt);
                            }
                            int len = strList.Count;
                            for (int m = 0; m < len; ++m)
                            {
                                cellInfo = new CellInfo(workPackageName, worksheet.Name, strList[m], i, j);
                                cellInfos.Add(cellInfo);
                            }
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// 创建输出文件
        /// </summary>
        private static void CreateSaveFile()
        {
            if (File.Exists(jsonOutputPath))
            {
                FileInfo fileInfo = new FileInfo(jsonOutputPath);
                fileInfo.Delete();
            }
            else
            {
                File.Create(jsonOutputPath);
            }

        }


        public class CellInfo
        {
            public int uid;
            public string workName;
            public string sheetname;
            public string cellContent;
            public int row;
            public int column;

            public CellInfo(string workName, string sheetname, string cellContent, int row, int column)
            {
                this.workName = workName;
                this.sheetname = sheetname;
                this.cellContent = cellContent;
                this.row = row;
                this.column = column;
            }

        }


        #region 表信息相关
        public class SheetInfo
        {
            public int sheetid;
            public string sheetname;

            public SheetInfo(int sheetid,string sheetname)
            {
                this.sheetid = sheetid;
                this.sheetname = sheetname;
            }
        }
        private static List<SheetInfo> GetSheetListFromDisk()
        {
            List<SheetInfo> sheetList;
            if (!File.Exists(sheetJsonPath))
            {
                File.WriteAllText(sheetJsonPath, "");
            }
            using (StreamReader sr = new StreamReader(sheetJsonPath))
            {
                sheetList = JsonConvert.DeserializeObject<List<SheetInfo>>(sr.ReadToEnd());
            }
            if (sheetList == null)
            {
                sheetList = new List<SheetInfo>();
            }
            sheetList.Sort(SheetListSort);
            return sheetList;
        }

        private static void SaveSheetList(List<SheetInfo> sheetList)
        {
            string sheetJsonStr = JsonConvert.SerializeObject(sheetList);
            using (StreamWriter sw = new StreamWriter(sheetJsonPath, false, Encoding.UTF8))
            {
                sw.Write(sheetJsonStr);
            }
        }

        private static void ProcessSheetList(ExcelPackage package, List<string> currentSheetNameList)
        {
            var sheets = package.Workbook.Worksheets;
            for (int i = 0; i < sheets.Count; i++)
            {
                var worksheet = sheets[i];
                if (worksheet.Name.StartsWith("#")) continue;

                var blackListFlag = false;
                for (int j = 0; j < blackList.Count; j++)
                {
                    if (worksheet.Name == blackList[j])
                    {
                        blackListFlag = true;
                        break;
                    }
                }
                if(blackListFlag)
                {
                    continue;
                }

                var isValidSheet = false;

                for (int j = 1; j <= worksheet.Dimension.Columns; ++j)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[1, j].Text)) break;
                    if (worksheet.Cells[2, j].Text.EndsWith("TR"))
                    {
                        isValidSheet = true;
                        break;
                    }
                }

                if (isValidSheet)
                {
                    if (!currentSheetNameList.Contains(worksheet.Name))
                    {
                        currentSheetNameList.Add(worksheet.Name);
                    }
                }
            }

        }
        private static void ProcessSheetListFinal(List<SheetInfo> sheetList, List<string> currentSheetNameList)
        {
            // 查找sheetlist里已经没用的sheet 清理掉
            var toDeleteSheetList = sheetList.FindAll(t => !currentSheetNameList.Contains(t.sheetname));
            foreach (var toDeleteSheet in toDeleteSheetList)
            {
                sheetList.Remove(toDeleteSheet);
            }

            // 将新的sheet保存到sheetlist中
            var toAddSheetNameList = currentSheetNameList.FindAll(t => sheetList.Find(k => k.sheetname == t) == null);
            foreach (var toAddSheetName in toAddSheetNameList)
            {
                var toAddSheetId = 1;
                foreach (var sheet in sheetList)
                {
                    if (sheet.sheetid > toAddSheetId)
                    {
                        break;
                    }
                    else
                    {
                        toAddSheetId += 1;
                    }
                }
                sheetList.Add(new SheetInfo(toAddSheetId, toAddSheetName));
                sheetList.Sort(SheetListSort);
            }

            //Console.WriteLine(string.Join("\n", sheetList.ConvertAll(t => t.sheetid + " " + t.sheetname)));
        }
        private static int SheetListSort(SheetInfo x, SheetInfo y)
        {
            return x.sheetid - y.sheetid;
        }
        #endregion

    }

}
