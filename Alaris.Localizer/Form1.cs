using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Alaris.API.Database;

namespace Alaris.Localizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DatabaseManager.Initialize("ALARIS");
            
            RefreshGridSizes();
            DatabaseManager.Query("PRAGMA encoding = 'UTF-8'");

            RefreshGridValues();
            

            gridView.CellValueChanged += CellValueChanged;
           
        }

        private void RefreshGridValues()
        {
            
            foreach (var cell in
                from DataGridViewRow row in gridView.Rows from DataGridViewCell cell in row.Cells select cell)
            {
                cell.Value = string.Empty;
            }


            var table = DatabaseManager.Query("SELECT * FROM localization");

            if(table == null)
            {
                MessageBox.Show("The program couldn't initialize because the required database couldn't be loaded.",
                                "Error", MessageBoxButtons.OK);

                Application.Exit();
                return;
            }

            foreach(DataRow row in table.Rows)
            {
                gridView.Rows.Add(row["id"], row["originalText"], row["text"], row["locale"]);
            }

            Closing += (s, e) => Application.Exit();
        }

        private void CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var row = gridView.Rows[e.RowIndex];

            if (row.Cells.Cast<DataGridViewCell>().Any(cell => cell.Value == null))
            {
                return;
            }

            var origTxt = row.Cells["originalText"].Value.ToString();
            var rtext = row.Cells["text"].Value.ToString();
            var loc = row.Cells["locale"].Value.ToString();
            var rid = row.Cells["id"].Value;

            if (string.IsNullOrEmpty(origTxt) || string.IsNullOrEmpty(rtext) || string.IsNullOrEmpty(loc))
                return;

            DatabaseManager.Query(string.Format("UPDATE localization SET originalText = '{0}', text = '{1}', locale = '{2}' WHERE id = '{3}'", origTxt, rtext, loc, rid));
            
            RefreshGridValues();
        }

        private void RefreshGridSizes()
        {
            gridView.Width = Width;
            gridView.Height = Height - 150;
            var columnWidth = (Width/4);
            var idWidth = columnWidth - 30;
            var textWidth = columnWidth + 30;

            foreach(DataGridViewColumn column in gridView.Columns)
            {
                if (column.Name == "id" || column.Name == "locale") column.Width = idWidth;
                if (column.Name == "text" || column.Name == "originalText") column.Width = textWidth;             
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            RefreshGridSizes();
        }
    }
}
