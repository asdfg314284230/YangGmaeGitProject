using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

/// <summary>
/// 和上一次迭代xlsx翻译文本比较，得到最新的xlsx翻译文本(里面包含的空白项即为需要翻译的项)
/// </summary>
namespace IsContinuousProj
{
    class Program
    {   
        //上一个版本的翻译表
        static string previousVersionXls;
        //最新需要翻译的表
        static string lastVersionXls;

        static void Main(string[] args)
        {

            string cfgPath = "./PathCfg.txt";
            if (File.Exists(cfgPath))
            {
                using (StreamReader sr = new StreamReader(cfgPath))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        string info = sr.ReadLine();
                        if (!string.IsNullOrEmpty(info))
                        {
                            if (i == 0)
                            {
                                previousVersionXls = info.Replace(@"\", "/");
                            }
                            else if (i == 1)
                            {
                                lastVersionXls = info.Replace(@"\", "/");
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


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            FileInfo previousInfo = new FileInfo(previousVersionXls);
            JudgeNull(previousInfo, "previousInfo==null");
            ExcelPackage previousPacakage = new ExcelPackage(previousInfo);
            JudgeNull(previousPacakage, "previousPacakage==null");
            ExcelWorksheet previousSheet = previousPacakage.Workbook.Worksheets[0];
            JudgeNull(previousSheet, "previousSheet==null");

            FileInfo lastInfo = new FileInfo(lastVersionXls);
            JudgeNull(lastInfo, "lastInfo==null");
            ExcelPackage lastPacakage = new ExcelPackage(lastInfo);
            JudgeNull(lastPacakage, "lastPacakage==null");
            ExcelWorksheet lastSheet = lastPacakage.Workbook.Worksheets[0];
            JudgeNull(lastSheet, "lastSheet==null");

            Dictionary<string, string> previousDic = new Dictionary<string, string>();
            for (int i = 1; i <= previousSheet.Dimension.Rows; i++)
            {
                string key = previousSheet.Cells[i, 1].Text;
                if (!previousDic.ContainsKey(key))
                {
                    string val = previousSheet.Cells[i, 2].Text;
                    previousDic.Add(key, val);
                }
                else
                {
                    Console.WriteLine("文本有问题，有重复项 key: "+key);
                }
            }

            for (int i = 1; i <= lastSheet.Dimension.Rows; i++)
            {   
                string str = lastSheet.Cells[i, 1].Text;
                if (previousDic.ContainsKey(str))
                {
                    lastSheet.Cells[i, 2].Value = previousDic[str];
                }
            }

            lastPacakage.Save();

        }


        private static void JudgeNull(object obj, string desc)
        {
            if (obj == null)
            {
                Console.WriteLine(desc);
                Console.ReadLine();
                return;
            }
        }

    }
}
