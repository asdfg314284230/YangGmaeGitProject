using System;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;
using System.Text;
using System.Linq;

namespace ExtractTranslatedText
{
    class Program
    {
        //从该路径下提取文本
        static string extractTxtPath = "C:/Users/10799/Desktop/test/xls_output/W文本.xlsx";
        static string extractOutPutTxtPath = "C:/Users/10799/Desktop/test/xls_output/W文本.xlsx";
        //得到的差异Content的输出路径
        static string outputPath;
        //老版本的文本
        static string oldVersionFilePath;
        //当前版本的文本
        static string currentVersionFilePath;

        static void Main(string[] args)
        {
            string cfgPath = "./PathCfg.txt";
            if (File.Exists(cfgPath))
            {
                using (StreamReader sr = new StreamReader(cfgPath))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string info = sr.ReadLine();
                        if (!string.IsNullOrEmpty(info))
                        {
                            if (i == 0)
                            {
                                outputPath = info.Replace(@"\", "/");
                            }
                            else if (i == 1)
                            {
                                extractOutPutTxtPath = info.Replace(@"\", "/");
                            }
                            else if (i == 2)
                            {
                                oldVersionFilePath = info.Replace(@"\", "/");
                            }
                            else if (i == 3)
                            {
                                currentVersionFilePath = info.Replace(@"\", "/");
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

            int input;
            if (int.TryParse(Console.ReadLine(), out input))
            {
                //输出xlsx不重复翻译文本
                if (input == 1)
                {
                    CreateSaveFile(extractOutPutTxtPath);

                    #region 
                    if (!File.Exists(extractTxtPath))
                    {
                        Console.WriteLine("extractTxtPath路径不存在");
                        Console.ReadLine();
                        return;
                    }

                    FileInfo fileInfo = new FileInfo(extractTxtPath);
                    ExcelPackage textPackage = new ExcelPackage(fileInfo);
                    if (textPackage == null)
                    {
                        Console.WriteLine("textPackage == null,文件可能有问题");
                        Console.ReadLine();
                        return;
                    }

                    ExcelWorksheet textSheet = textPackage.Workbook.Worksheets[0];
                    if (textSheet == null)
                    {
                        Console.WriteLine("textSheet不存在");
                        Console.ReadLine();
                        return;
                    }

                    Dictionary<string, List<int>> noRepeatedContentDic = new Dictionary<string, List<int>>();
                    int column = 4;
                    for (int i = 4; i <= textSheet.Dimension.Rows; i++)
                    {
                        string val = textSheet.Cells[i, column].Text;
                        if (noRepeatedContentDic.ContainsKey(val))
                        {
                            string idStr = textSheet.Cells[i, 1].Text;
                            if (string.IsNullOrEmpty(idStr)) continue;
                            int id;
                            //id必须不能为空和可转换成int才行
                            if (int.TryParse(idStr, out id))
                            {
                                noRepeatedContentDic[val].Add(id);
                            }
                        }
                        else
                        {
                            string idStr = textSheet.Cells[i, 1].Text;
                            if (string.IsNullOrEmpty(idStr)) continue;
                            int id;
                            if (int.TryParse(idStr, out id))
                            {
                                noRepeatedContentDic.Add(val, new List<int>() { id });
                            }
                        }
                    }

                    FileInfo outputPathInfo = new FileInfo(extractOutPutTxtPath);
                    outputPathInfo.IsReadOnly = false;
                    ExcelPackage outputPackage = new ExcelPackage(outputPathInfo);
                    outputPackage.Workbook.Worksheets.Add("NeedTranslationTxt");

                    ExcelWorksheet outputSheet = outputPackage.Workbook.Worksheets[0];
                    int outputColumn = 1;
                    int outputRow = 1;
                    foreach (var item in noRepeatedContentDic)
                    {
                        outputSheet.Cells[outputRow++, outputColumn].Value = item.Key;
                    }
                    outputPackage.Save();

                    #endregion

                }
                else
                {
                    CreateSaveFile(outputPath);


                    FileInfo oldInfo = new FileInfo(oldVersionFilePath);
                    FileInfo curInfo = new FileInfo(currentVersionFilePath);
                    JudgeNull(oldInfo, "不存在老版本文件");
                    JudgeNull(curInfo, "不存在当前版本文件");
                    ExcelPackage oldPackage = new ExcelPackage(oldInfo);
                    ExcelPackage curPackage = new ExcelPackage(curInfo);
                    JudgeNull(oldPackage, "oldPackage==null");
                    JudgeNull(curPackage, "curPackage == null");
                    ExcelWorksheet oldSheet = oldPackage.Workbook.Worksheets[0];
                    ExcelWorksheet curSheet = curPackage.Workbook.Worksheets[0];
                    JudgeNull(oldSheet, "oldSheet==null");
                    JudgeNull(curSheet, "curSheet == null");

                    List<string> oldList = new List<string>();
                    int column = 1;
                    for (int i = 1; i <= oldSheet.Dimension.Rows; ++i)
                    {
                        string str = oldSheet.Cells[i, column].Text;
                        if (!string.IsNullOrEmpty(str))
                        {
                            oldList.Add(str);
                        }
                    }

                    List<string> curList = new List<string>();
                    for (int i = 1; i <= curSheet.Dimension.Rows; ++i)
                    {
                        string str = curSheet.Cells[i, column].Text;
                        if (!string.IsNullOrEmpty(str))
                        {
                            curList.Add(str);
                        }
                    }

                    var tempList = curList.Intersect<string>(oldList).ToList();
                    var resList = curList.Except<string>(tempList).ToList();

                    if (resList.Count <= 0)
                    {
                        Console.WriteLine("没有差异");
                        Console.ReadLine();
                        return;
                    }

                    foreach (var item in resList)
                    {
                        Console.WriteLine(item);
                    }

                    FileInfo outputPathInfo = new FileInfo(outputPath);
                    outputPathInfo.IsReadOnly = false;
                    ExcelPackage outputPackage = new ExcelPackage(outputPathInfo);
                    outputPackage.Workbook.Worksheets.Add("NeedTranslationTxt");

                    ExcelWorksheet outputSheet = outputPackage.Workbook.Worksheets[0];
                    int outputColumn = 1;
                    int outputRow = 1;
                    foreach (var item in resList)
                    {
                        outputSheet.Cells[outputRow++, outputColumn].Value = item;
                    }
                    outputPackage.Save();
                }
            }

            
        }

        private static void CreateSaveFile(string path)
        {
            if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                fileInfo.Delete();
            }
            File.Create(path);
        }

        private static void JudgeNull(object obj,string desc)
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
