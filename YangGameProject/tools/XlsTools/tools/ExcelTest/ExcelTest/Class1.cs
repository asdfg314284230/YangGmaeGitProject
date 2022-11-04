using System.Runtime.InteropServices;
using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;
using System.Windows.Forms;
using System.IO;
using OfficeOpenXml;
using System.Data;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using ExcelTest;
using Application = Microsoft.Office.Interop.Excel.Application;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using OfficeOpenXml.FormulaParsing;

namespace Excel_DNA_Template_CS
{
    [ComVisible(true)]
    public class Class1 : ExcelRibbon, IExcelAddIn
    {
        public IRibbonUI R;
        public override string GetCustomUI(string RibbonID)
        {
            string xml = @"<customUI xmlns='http://schemas.microsoft.com/office/2009/07/customui' onLoad='OnLoad'>
    <ribbon startFromScratch='false'>
        <tabs>
            <tab id='Tab1' label='文本表工具'>
                <group id='Group1'>
                    <button id='Button1' label='文本表Id查看' imageMso='W' onAction='Button1_Click'/>
                </group>
            </tab>
        </tabs>
    </ribbon>
</customUI>";
            return xml;
        }
        public void OnLoad(IRibbonUI ribbon)
        {
            R = ribbon;
            R.ActivateTab(ControlID: "Tab1");

            Application app = (Application)ExcelDnaUtil.Application;
            app.EnableEvents = true;
            app.SheetSelectionChange -= App_SheetSelectionChange;
            app.SheetSelectionChange += App_SheetSelectionChange;
        }

        public void Button1_Click(IRibbonControl control)
        {
            Module1.ShowOrHideForm();
        }


        void IExcelAddIn.AutoClose()
        {
            Module1.DisposeForm();
        }

        void IExcelAddIn.AutoOpen()
        {
        }

        private void App_SheetSelectionChange(object Sh, Range Target)
        {
            if (Module1.form == null|| !Module1.form.Visible)
            {
                return;
            }
            // Find the last real row
            Application app = (Application)ExcelDnaUtil.Application;
            Workbook activeWorkbook = app.ActiveWorkbook;
            if(activeWorkbook!=null)
            {
                var activeWorksheet= activeWorkbook.ActiveSheet;
                if(activeWorksheet!=null&& activeWorksheet is Worksheet)
                {
                    var usedRange = ((Worksheet)activeWorksheet).UsedRange;
                    var usedRangeIntersectRange = app.Intersect(Target, usedRange);
                    UpdateActiveCell(usedRangeIntersectRange);
                }
            }
        }

        Range selectedRange;
        private void UpdateActiveCell(Range activeCellRange)
        {
            Debug.WriteLine("UpdateActiveCell");
            selectedRange = null;
            Application app = (Application)ExcelDnaUtil.Application;
            if (activeCellRange != null)
            {
                foreach (Range activeCell in activeCellRange)
                {
                    string str = activeCell.Text.ToString();
                    if(!string.IsNullOrEmpty(str))
                    {
                        if(selectedRange==null)
                        {
                            selectedRange = activeCell;
                        }
                        else
                        {
                            selectedRange = app.Union(selectedRange, activeCell);
                        }
                    }
                    //Debug.WriteLine(str);
                }
            }

            var list = new List<Range>();
            if(selectedRange!=null)
            {
                foreach (Range selectedRangeCell in selectedRange)
                {
                    list.Add(selectedRangeCell);
                }            
            }

            Module1.Refresh(list);
        }

        //private Range GetAllRange(Range range)
        //{
        //    if(range.Count==1)
        //    {
        //        return range.Row;
        //    }
        //    var last = range.Find("*", SearchOrder: XlSearchOrder.xlByRows, SearchDirection: XlSearchDirection.xlPrevious);
        //    if(last == null)
        //    {
        //        return 0;
        //    }
        //    return last.Row;
        //}

        public static string TextCfg(int textCfgId)
        {
            string textCfgWorkbookName = "W文本.xlsx";
            string textCfgWorkbookPath = Path.Combine( "./",textCfgWorkbookName);
            if (!File.Exists(textCfgWorkbookPath))
            {
                Application app = (Application)ExcelDnaUtil.Application;
                Workbook activeWorkbook = app.ActiveWorkbook;
                if (activeWorkbook != null)
                {
                    var activeWorkbookPath = activeWorkbook.Path;
                    if (!string.IsNullOrEmpty(textCfgWorkbookPath))
                    {
                        textCfgWorkbookPath = Path.Combine(activeWorkbookPath, textCfgWorkbookName);
                    }
                }
            }

            if (!string.IsNullOrEmpty(textCfgWorkbookPath)&& File.Exists(textCfgWorkbookPath))
            {
                using (var stream = File.Open(textCfgWorkbookPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var excelPackage= new ExcelPackage(stream))
                    {
                        var workbook = excelPackage.Workbook;
                        if(workbook!=null)
                        {
                            var sheet= workbook.Worksheets["Text"];
                            if(sheet!=null)
                            {
                                var end = sheet.Dimension.End;
                                var idColumnIdx = 0;
                                var contentColumnIdx = 3;

                                for(int i=3;i<=end.Row;++i)
                                {
                                    var idStr = sheet.Cells[i+1, idColumnIdx+1].Text.Trim();
                                    if (int.TryParse(idStr, out int rowId))
                                    {
                                        if (rowId == textCfgId)
                                        {
                                            var rowContentStr = sheet.Cells[i + 1, contentColumnIdx + 1].Text;
                                            return rowContentStr;
                                        }
                                    }
                                }
                                
                            }
                        }
                    }
                }
                return "文本表Id有误:" + textCfgId;
            }
            else
            {
                return "未找到文本表";
            }

        }

    }

    public static class Module1
    {
        public static Form1 form;

        public static void DisposeForm()
        {
            form?.Close();
            form = null;
        }

        public static void ShowOrHideForm()
        {
            if (form == null || form.IsDisposed)
            {
                form = new Form1();
                form.Visible = true;
                form.TopMost = true;
                form.MinimizeBox = false;
            }
            else
            {
                form.Visible = !form.Visible;
            }
        }

        public static void Refresh(List<Range> selectedCellList)
        {
            form?.Refresh(selectedCellList);
        }
    }
}
