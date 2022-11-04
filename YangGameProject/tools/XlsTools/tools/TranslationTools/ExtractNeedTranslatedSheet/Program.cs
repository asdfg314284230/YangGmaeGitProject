using System;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Diagnostics;

/// <summary>
/// 把手机、微博等人工翻译表提取出来,并把翻译文本对应加到TextTranslation表中
/// </summary>
namespace ExtractNeedTranslatedSheet
{
    class Program
    {
        static string xlsFolderPath = string.Empty;
        static string writePath = string.Empty;
        //已经被翻译的文件路径
        static string translatedPath = string.Empty;
        static int beginId;
        static string writeSheetName = "TextTranslation";

        static List<string> packagesNames = new List<string>();
        static List<string> extractedList = new List<string>();

        //key:原文本 value：翻译文本
        static Dictionary<string, string> transDic = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            string cfgPath = ".\\tools\\TranslationTools\\ExtractNeedTranslatedSheet\\bin\\Debug\\net5.0\\PathCfg.txt";
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            if (File.Exists(cfgPath))
            {
                using (StreamReader sr = new StreamReader(cfgPath))
                {
                    for (int i = 0; i < 6; i++)
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
                                writePath = info.Replace(@"\", "/");
                            }
                            else if(i==2)
                            {
                                translatedPath = info.Replace(@"\", "/");
                            }
                            else if (i == 3)
                            {
                                beginId = int.Parse(info);
                            }
                            else if (i == 4)
                            {
                                if (string.IsNullOrEmpty(info)) continue;
                                string[] splits = info.Split('|');

                                int len = splits.Length;
                                for (int j = 0; j < len; j++)
                                {
                                    splits[j] = Path.Combine(xlsFolderPath, splits[j]);
                                    splits[j] =  splits[j].Replace(@"\", "/");
                                }
                                packagesNames.AddRange(splits);
                            }
                            else if(i==5)
                            {
                                if (string.IsNullOrEmpty(info)) continue;
                                string[] splits = info.Split('|');
                                extractedList.AddRange(splits);
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
                Console.WriteLine("配置文件路径不存在 + "+ cfgPath);
            }

            Console.WriteLine("找出需要人工翻译的文本....");
            //得到配置文件中指定的表
            List<ExcelWorksheet> appointSheets = GetAppointSheet(GetCfgTxtAllPackages());

            for (int i = 0; i < appointSheets.Count; i++)
            {
                var curSheet = appointSheets[i];
                //逐列找TR
                for (int j = 1; j <= curSheet.Dimension.Columns; j++)
                {
                    if (appointSheets[i].Cells[2, j].Text.EndsWith("TR"))
                    {
                        //逐行写入字典
                        for (int m = 4; m <= curSheet.Dimension.Rows; m++)
                        {
                            if (!transDic.ContainsKey(curSheet.Cells[m, j].Text))
                            {
                                transDic.Add(curSheet.Cells[m, j].Text, string.Empty);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("把原文本写入到TextTranslation表中....");

            if (!File.Exists(writePath))
            {
                Console.WriteLine($"路径{writePath}不存在");
                Console.ReadLine();
                return;
            }
            FileInfo textInfo = new FileInfo(writePath);
            textInfo.IsReadOnly = false;
            ExcelPackage textPackage = new ExcelPackage(textInfo);
            ExcelWorksheet textTransSheet = null;

            var textSheets = textPackage.Workbook.Worksheets;
            for (int i = 0; i < textSheets.Count; i++)
            {
                if(textSheets[i].Name == writeSheetName)
                {
                    textTransSheet = textSheets[i];
                    break;
                }
            }
            //如果没找到就自动创建
            if (textTransSheet == null)
            {
                textTransSheet = textSheets.Add(writeSheetName);
                textTransSheet.Cells[1, 1].Value = "Id";
                textTransSheet.Cells[1, 2].Value = "OriginalContent";
                textTransSheet.Cells[1, 3].Value = "TranslatedContent";
                textTransSheet.Cells[2, 1].Value = "文本id";
                textTransSheet.Cells[2, 2].Value = "原文本";
                textTransSheet.Cells[2, 3].Value = "翻译后文本";
                textTransSheet.Cells[3, 1].Value = "int";
                textTransSheet.Cells[3, 2].Value = "string";
                textTransSheet.Cells[3, 3].Value = "string";
            }
            //var enumerator = transDic.GetEnumerator();
            //int column = 1;
            //int row = 4;
            //do
            //{
            //    textTransSheet.Cells[row++, column].Value = enumerator.Current.Key;
            //}while(enumerator.MoveNext());
            //Console.WriteLine("把翻译后文本写入到TextTranslation表中....");

            FileInfo transInfo = new FileInfo(translatedPath);
            ExcelPackage transPackage = new ExcelPackage(transInfo);
            var transSheets = transPackage.Workbook.Worksheets;
            for (int i = 0; i < transSheets.Count; i++)
            {
                var curSheet = transSheets[i];
                int originalCo = 2;
                int transCo = 4;
                for (int m = 4; m <= curSheet.Dimension.Rows; ++m)
                {
                    string val = curSheet.Cells[m, originalCo].Text;
                    if (string.IsNullOrEmpty(val)) continue;
                    if (transDic.ContainsKey(val))
                    {
                        transDic[val] = curSheet.Cells[m, transCo].Text;
                    }
                    else
                    {
                        Console.WriteLine(val+"  键不在提取出的textTranslation原文本表中,已加入");
                        transDic.Add(val, curSheet.Cells[m, transCo].Text);
                    }
                }
            }
            Console.WriteLine("为翻译文本生成id....");

            int row = 4;
            foreach (var item in transDic)
            {
                textTransSheet.Cells[row, 1].Value = ++beginId;
                textTransSheet.Cells[row, 2].Value = item.Key;
                textTransSheet.Cells[row, 3].Value = item.Value;
                row++;
            }

            textPackage.Save();
            textInfo.IsReadOnly = true;
        }


        /// <summary>
        /// 得到配置文本中，所配置的表
        /// </summary>
        /// <returns></returns>
        static List<ExcelPackage> GetCfgTxtAllPackages()
        {
            List<ExcelPackage> packages = new List<ExcelPackage>();

            foreach (var item in packagesNames)
            {
                if (!File.Exists(item))
                {
                    Console.WriteLine(item + "   路径不存在");
                    continue;
                }

                FileInfo info = new FileInfo(item);
                ExcelPackage excelPackage = new ExcelPackage(info);
                packages.Add(excelPackage);
            }
            return packages;
        }

        /// <summary>
        /// 得到配置文件中指定的表
        /// </summary>
        /// <returns></returns>
        static List<ExcelWorksheet> GetAppointSheet(List<ExcelPackage> packages)
        {
            List<ExcelWorksheet> sheets = new List<ExcelWorksheet>();
            foreach (var item in packages)
            {
                for (int i = 0; i < item.Workbook.Worksheets.Count; i++)
                {
                    ExcelWorksheet sheet = item.Workbook.Worksheets[i];

                    for (int j = 0; j < extractedList.Count; j++)
                    {
                        if(extractedList[j] == sheet.Name)
                        {
                            sheets.Add(sheet);
                            break;
                        }
                    }
                }
            }
            return sheets;
        }

    }
}
