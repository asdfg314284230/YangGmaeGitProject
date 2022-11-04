using System;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;

namespace GenerateUidAndImport
{
    class Program
    {
        static int uidBeginVal;     //起始id
        static int sheetidWidth;     //表id区间宽度
        static string jsonPath;
        static string sheetJsonPath = string.Empty;
        static string originalFilePath;
        static string copyOutputPath;

        static Dictionary<string, List<CellInfo>> ObjDic;
        static List<SheetInfo> sheetList;
        //static Dictionary<string, SheetInfo> strSheetInfoDic=new Dictionary<string, SheetInfo>();

        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string pathCfg = ".\\tools\\TranslationTools\\GenerateUidAndImport\\bin\\Debug\\net5.0\\PathCfg.txt";

            if (File.Exists(pathCfg))
            {
                using (StreamReader sw = new StreamReader(pathCfg))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        string str = sw.ReadLine();
                        if (string.IsNullOrEmpty(str))
                        {
                            Console.WriteLine("存在路径为空的情况");
                            Console.ReadLine();
                            return;
                        }
                        str = str.Replace(@"\", "/");

                        if (i == 0)
                        {
                            jsonPath = str;
                        }
                        else if (i == 1)
                        {
                            copyOutputPath = str;
                        }
                        else if (i == 2)
                        {
                            originalFilePath = str;
                        }
                        else if (i == 3)
                        {
                            sheetJsonPath = str;
                        }
                        else if(i == 4)
                        {
                            uidBeginVal = int.Parse(str);
                        }
                        else if (i == 5)
                        {
                            sheetidWidth = int.Parse(str);
                        }
                        else
                        {
                            Console.WriteLine("未知列");
                            Console.ReadLine();
                            return;
                        }
                    }
                }
            }
            else
            {   
                Console.WriteLine("文件不存在 pathCfg : "+ pathCfg);
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists(copyOutputPath))
            {
                Directory.CreateDirectory(copyOutputPath);
            }

            Console.WriteLine("复制文件...");

            string[] outputPaths = Directory.GetFiles(copyOutputPath);
            int opLenght = outputPaths.Length;

            for (int i = 0; i < opLenght; i++)
            {
                FileInfo fileInfo = new FileInfo(outputPaths[i]);
                fileInfo.IsReadOnly = false;
                fileInfo.Delete();
            }

            string[] paths = Directory.GetFiles(originalFilePath, "*.xlsx");
            int lenght = paths.Length;
            for (int i = 0; i < lenght; i++)
            {
                string[] splits = paths[i].Split("\\");

                FileInfo fileInfo = new FileInfo(paths[i]);
                var newFileInfo = fileInfo.CopyTo(Path.Combine(copyOutputPath, splits[splits.Length - 1]));
                newFileInfo.IsReadOnly = false;
            }



            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (File.Exists(jsonPath))
            {
                string objStr;
                Console.WriteLine("读取json文件...");
                using (StreamReader sr = new StreamReader(jsonPath, Encoding.UTF8))
                {
                    objStr = sr.ReadToEnd();
                }
                if (string.IsNullOrEmpty(objStr))
                {
                    Console.WriteLine("什么都没读到!请检查目标文件");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("反序列化....");
                ObjDic = JsonConvert.DeserializeObject<Dictionary<string, List<CellInfo>>>(objStr);
                
                var sheetObjDic = new Dictionary<string, Dictionary<string, List<CellInfo>>>();
                foreach(var kv in ObjDic)
                {
                    var str = kv.Key;
                    var cellInfoList = kv.Value;
                    if(!sheetObjDic.ContainsKey(str))
                    {
                        sheetObjDic.Add(str, new Dictionary<string, List<CellInfo>>());
                    }

                    var sheetCellDic = sheetObjDic[str];
                    foreach(var cellInfo in cellInfoList)
                    {
                        var sheetname = cellInfo.sheetname;
                        if (!sheetCellDic.ContainsKey(sheetname))
                        {
                            sheetCellDic.Add(sheetname, new List<CellInfo>());
                        }
                        var sheetCellInfoList = sheetCellDic[sheetname];
                        sheetCellInfoList.Add(cellInfo);
                    }
                }

                sheetList = GetSheetListFromDisk();
                ProcessCellAndSheet();


                List<CellInfo> allCellInfos = GenerateUid(sheetObjDic, uidBeginVal);
                var packages = GetAllXls();
                List<ExcelWorksheet> sheets = GetAllSheets(packages);


                Console.WriteLine("写入文本表....");
                //写入文本
                ExcelWorksheet textSheet = GetOneSheet(sheets, "Text");

                if (textSheet != null)
                {
                    int beginRow = textSheet.Dimension.Rows + 1;
                    foreach (var kv in sheetObjDic)
                    {
                        var str = kv.Key;
                        var sheetCellDic = kv.Value;
                        foreach(var sheetCellKv in sheetCellDic)
                        {
                            var sheetname = sheetCellKv.Key;
                            var cellInfoList = sheetCellKv.Value;
                            var uid = cellInfoList[0].uid;
                            textSheet.Cells[beginRow, 1].Value = uid;
                            textSheet.Cells[beginRow, 4].Value = str;
                            beginRow++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("textSheet==null");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine("替换文本为id...");
                //替换所有文本为id
                foreach (var item in allCellInfos)
                {
                    var sheet = GetOneSheet(sheets, item.sheetname);
                    if (sheet != null)
                    {
                        string val = sheet.Cells[3, item.column].Text;
                        //数组情况
                        if (val == "int*" || val == "string*")
                        {
                            sheet.Cells[3, item.column].Value = "int*";
                            string[] splits = sheet.Cells[item.row, item.column].Text.Split(';');
                            int len = splits.Length;

                            for (int i = 0; i < len; i++)
                            {
                                if(splits[i] == item.cellContent)
                                {
                                     splits[i] = (item.uid.ToString());
                                }
                            }

                            sheet.Cells[item.row, item.column].Value = string.Join(';', splits);

                        }
                        else
                        {
                            sheet.Cells[3, item.column].Value = "int";
                            sheet.Cells[item.row, item.column].Value = item.uid;
                        }
                    }
                }

                for (int i = 0; i < packages.Count; i++)
                {
                    packages[i].Save();
                }

                string[] outputPathss = Directory.GetFiles(copyOutputPath, "*.xlsx");

                for (int i = 0; i < lenght; i++)
                {
                    string[] splits = outputPathss[i].Split("\\");

                    FileInfo fileInfo = new FileInfo(outputPathss[i]);
                    fileInfo.IsReadOnly = true;
                }

                if(File.Exists(jsonPath))
                    File.Delete(jsonPath);

                stopwatch.Stop();
                Console.WriteLine("消耗时间:  " + (float)stopwatch.ElapsedMilliseconds / 1000+"秒");
            }
        }

        private static ExcelWorksheet GetOneSheet(List<ExcelWorksheet> sheets, string name)
        {
            for (int i = 0; i < sheets.Count; i++)
            {
                if (sheets[i].Name == name) return sheets[i];
            }
            return null;
        }


        /// <summary>
        /// 得到配置路径下所有的xls
        /// </summary>
        /// <returns></returns>
        private static List<ExcelPackage> GetAllXls()
        {
            if (!Directory.Exists(copyOutputPath))
            {
                Console.WriteLine("路径不存在");
                Console.ReadLine();
                return null;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(copyOutputPath);
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

        private static List<ExcelWorksheet> GetAllSheets(List<ExcelPackage> packages)
        {

            //List<ExcelPackage> packages = GetAllXls();
            List<ExcelWorksheet> sheets = new List<ExcelWorksheet>();

            for (int i = 0; i < packages.Count; i++)
            {
                ExcelWorksheets info = packages[i].Workbook.Worksheets;
                for (int j = 0; j < info.Count; j++)
                {
                    if (info[j].Name.StartsWith("#")) continue;
                    sheets.Add(info[j]);
                }
            }
            return sheets;
        }

        /// <summary>
        /// 遍历文本字典，把每个字典出现的键的对应uid写入CellInfo
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="beginUid"></param>
        /// <returns></returns>
        private static List<CellInfo> GenerateUid(Dictionary<string, Dictionary<string, List<CellInfo>>> dic, int beginUid)
        {
            Dictionary<SheetInfo, int> sheetCellUidDic = new Dictionary<SheetInfo, int>();
            List<CellInfo> allCellInfos = new List<CellInfo>();
            var str = "";
            foreach (var item in dic.Keys)
            {
                var sheetCellDic = dic[item];
                foreach(var sheetCellKv in sheetCellDic)
                {
                    var sheetname = sheetCellKv.Key;
                    var sheetInfo = sheetList.Find(t => t.sheetname == sheetname);
                    if(sheetInfo!=null)
                    {
                        if(!sheetCellUidDic.ContainsKey(sheetInfo))
                        {
                            sheetCellUidDic.Add(sheetInfo, 0);
                        }
                        var cellInfoList = sheetCellKv.Value;
                        foreach(var cellInfo in cellInfoList)
                        {
                            var uid = beginUid + sheetidWidth * sheetInfo.sheetid + sheetCellUidDic[sheetInfo];
                            cellInfo.uid = uid;
                            allCellInfos.Add(cellInfo);
                        }
                        sheetCellUidDic[sheetInfo] = sheetCellUidDic[sheetInfo] + 1;
                    }
                    else
                    {
                        Console.WriteLine("此sheetname不存在:" + sheetname);
                    }
                }
            }

            //var json = JsonConvert.SerializeObject(dic);
            //using (StreamWriter sw = new StreamWriter("./test2.json", false, encoding: Encoding.UTF8))
            //{
            //    sw.Write(json);
            //}
            //using (StreamWriter sw = new StreamWriter("./test2", false, encoding: Encoding.UTF8))
            //{
            //    sw.Write(str);
            //}



            return allCellInfos;
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
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

            public SheetInfo(int sheetid, string sheetname)
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

        private static void ProcessCellAndSheet()
        {
            //strSheetInfoDic = new Dictionary<string, SheetInfo>();
            //if (ObjDic!=null&&sheetList!=null)
            //{
            //    foreach(var kv in ObjDic)
            //    {
            //        var cellInfoList = kv.Value;
            //        if(cellInfoList.Count>0)
            //        {
            //            var firstCellInfo = cellInfoList[0];
            //            var firstCellSheetname = firstCellInfo.sheetname;
            //            var sheetInfo = sheetList.Find(t => t.sheetname == firstCellSheetname);
            //            if(sheetInfo!=null)
            //            {
            //                if(!strSheetInfoDic.ContainsKey(kv.Key))
            //                {
            //                    strSheetInfoDic.Add(kv.Key, null);
            //                }
            //                strSheetInfoDic[kv.Key] = sheetInfo;
            //            }
            //        }
            //    }
            //}

            //var json= JsonConvert.SerializeObject(strSheetInfoDic);
            //using (StreamWriter sw=new StreamWriter("./test.json",false,encoding:Encoding.UTF8))
            //{
            //    sw.Write(json);
            //}
        }

        private static int SheetListSort(SheetInfo x, SheetInfo y)
        {
            return x.sheetid - y.sheetid;
        }
        #endregion

    }
}
