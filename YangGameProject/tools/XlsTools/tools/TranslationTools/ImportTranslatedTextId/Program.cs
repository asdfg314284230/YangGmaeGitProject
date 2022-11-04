using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 将人工翻译文本的单独id写入到xls表中，在进行导表
/// </summary>
namespace ImportTranslatedTextId
{
    class Program
    {

        static string readPath = string.Empty;
        static string writePath = string.Empty;
        static string readSheetName = "TextTranslation";
        static List<string> packagesPath = new List<string>();
        static List<string> readSheetList = new List<string>();

        static void Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string cfgPath = ".\\tools\\TranslationTools\\ImportTranslatedTextId\\bin\\Debug\\net5.0\\PathCfg.txt";
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
                                readPath = info.Replace(@"\", "/");
                            }
                            else if (i == 1)
                            {
                                writePath = info.Replace(@"\", "/");
                            }
                            else if (i == 2)
                            {
                                if (string.IsNullOrEmpty(info)) continue;
                                string[] splits = info.Split('|');

                                int len = splits.Length;
                                for (int j = 0; j < len; j++)
                                {
                                    splits[j] = Path.Combine(writePath, splits[j]);
                                    splits[j] = splits[j].Replace(@"\", "/");
                                }
                                packagesPath.AddRange(splits);
                            }
                            else if (i == 3)
                            {
                                if (string.IsNullOrEmpty(info)) continue;
                                string[] splits = info.Split('|');
                                readSheetList.AddRange(splits);
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
                Console.WriteLine("配置文件路径不存在 + " + cfgPath);
            }

            var readSheet = GetReadSheet();
            if (readSheet == null)
            {
                Console.WriteLine("textTranslation表没找到");
                return;
            }
            Dictionary<string, string> curDic = new Dictionary<string, string>();

            for (int i = 4; i <= readSheet.Dimension.Rows; ++i)
            {
                var text = readSheet.Cells[i, 2].Text;
                var id = readSheet.Cells[i, 1].Text;
                if (!curDic.ContainsKey(text))
                {
                    curDic.Add(text, id);
                }
                else
                {
                    Console.WriteLine($"键  {text}  重复");
                }
            }
            List<ExcelPackage> packages;
            var appointSheets = GetAppointSheet(out packages);
            Console.WriteLine("换成id....");
            for (int i = 0; i < appointSheets.Count; ++i)
            {
                var curSheet = appointSheets[i];
                for (int j = 1; j <= curSheet.Dimension.Columns; ++j)
                {
                    if (curSheet.Cells[2, j].Text.EndsWith("TR"))
                    {
                        //如果是数组
                        if (curSheet.Cells[3, j].Text.Contains("*"))
                        {
                            curSheet.Cells[3, j].Value = "int*";
                            for (int m = 4; m <= curSheet.Dimension.Rows; ++m)
                            {
                                string curTxt = curSheet.Cells[m, j].Text;
                                string[] splits = curTxt.Split(';');
                                string joinStr;
                                int len = splits.Length;
                                for (int x = 0; x < len; x++)
                                {
                                    if (string.IsNullOrEmpty(splits[x])) continue;
                                    if (curDic.ContainsKey(splits[x]))
                                    {
                                        splits[x] = curDic[splits[x]];
                                    }
                                    else
                                    {
                                        Console.WriteLine($"TextTranslation表不包含 <color=#fff000>{splits[x]}</color> 的翻译文本");
                                    }
                                }
                                joinStr = string.Join(';', splits);
                                curSheet.Cells[m, j].Value = joinStr;
                            }
                        }
                        else
                        {
                            curSheet.Cells[3, j].Value = "int";
                            for (int m = 4; m <= curSheet.Dimension.Rows; ++m)
                            {
                                string curTxt = curSheet.Cells[m, j].Text;
                                if (string.IsNullOrEmpty(curTxt)) continue;
                                if (curDic.ContainsKey(curTxt))
                                {
                                    curSheet.Cells[m, j].Value = curDic[curTxt];
                                }
                                else
                                {
                                    Console.WriteLine($"TextTranslation表不包含 <color=#fff000>{curTxt}</color> 的翻译文本");
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < packages.Count; i++)
            {
                packages[i].Save();
            }
            Console.WriteLine("转换完成....");
        }

        /// <summary>
        /// 得到读取id，和翻译内容的表
        /// </summary>
        /// <returns></returns>
        static ExcelWorksheet GetReadSheet()
        {
            FileInfo info = new FileInfo(readPath);
            ExcelPackage readPackage = new ExcelPackage(info);
            foreach (var item in readPackage.Workbook.Worksheets)
            {
                if(item.Name == readSheetName)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 得到所有需要被写入的表
        /// </summary>
        /// <returns></returns>
        static List<ExcelWorksheet> GetAppointSheet(out List<ExcelPackage> packages)
        {
            packages = new List<ExcelPackage>();
            List<ExcelWorksheet> sheets = new List<ExcelWorksheet>();
            foreach (var item in packagesPath)
            {
                FileInfo fileInfo = new FileInfo(item);
                if (fileInfo == null)
                {
                    Console.WriteLine(item+" 路径找不到");
                    continue;
                }
                fileInfo.IsReadOnly = false;
                ExcelPackage excelPackage = new ExcelPackage(item);
                packages.Add(excelPackage);
                var curSheets = excelPackage.Workbook.Worksheets;

                for (int i = 0; i < curSheets.Count; i++)
                {
                    for (int j = 0; j < readSheetList.Count; j++)
                    {
                        if(curSheets[i].Name == readSheetList[j])
                        {
                            sheets.Add(curSheets[i]);
                        }
                    }
                }
            }
            return sheets;
        }

    }
}
