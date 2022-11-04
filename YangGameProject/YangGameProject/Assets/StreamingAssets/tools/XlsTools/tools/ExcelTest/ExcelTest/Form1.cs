using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDna.Integration;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace ExcelTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        List<Range> selectedCellList = new List<Range>();

        public void Refresh(List<Range> selectedCellList)
        {
            this.selectedCellList = selectedCellList;
            if (selectedCellList.Count == 1)
            {
                this.WorkPanel.Visible = true;
                var range = selectedCellList[0];
                var textCfgIdStrVal = range.Value;
                if(textCfgIdStrVal!=null)
                {
                    var textCfgIdStr = textCfgIdStrVal.ToString();
                    if (!string.IsNullOrEmpty(textCfgIdStr))
                    {
                        this.panel2.Visible = true;
                        int textCfgId = 0;
                        int.TryParse(textCfgIdStr, out textCfgId);
                        this.textBox1.Text = Excel_DNA_Template_CS.Class1.TextCfg(textCfgId);
                    }
                    else
                    {
                        this.panel2.Visible = false;
                    }
                }
                else
                {
                    this.panel2.Visible = false;
                }
            }
            else if (selectedCellList.Count > 1)
            {
                this.WorkPanel.Visible = true;
                this.panel2.Visible = false;
            }
            else
            {
                this.WorkPanel.Visible = false;
            }
        }
    }
}
