using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetApp
{
    public partial class Form1 : Form
    {
        private ShareableSpreadSheet shareableSpreadSheet;

        //Constructor
        public Form1()
        {
            InitializeComponent();
            
        }

        //Load
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = Screen.PrimaryScreen.WorkingArea.Size;
        }

        //New SpreadSheet button
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            label1.Visible = true;
            label3.Visible = true;
        }

        //Load SpreadSheet button
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button1.Visible = false;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                shareableSpreadSheet = new ShareableSpreadSheet(1, 1);
                shareableSpreadSheet.load(openFileDialog.FileName);                
            }
            show_grid();
        }

        //create spreadsheet button
        private void button3_Click(object sender, EventArgs e)
        {
            int rows = Int32.Parse(textBox1.Text);
            int cols = Int32.Parse(textBox2.Text);
            shareableSpreadSheet = new ShareableSpreadSheet(rows, cols);
            button3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            label1.Visible = false;
            label3.Visible = false;
            dataGridView1.Visible = true;
            editToolStripMenuItem.Visible = true;
            toolStripMenuItem3.Visible = true;
            label2.Visible = false;
            pictureBox1.Visible = false;
            show_grid();
        }

        //open
        private void oToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                if (shareableSpreadSheet != null)
                {
                    shareableSpreadSheet.load(openFileDialog.FileName);
                }
                else
                {
                    shareableSpreadSheet = new ShareableSpreadSheet(1, 1);
                    shareableSpreadSheet.load(openFileDialog.FileName);

                }
            }
            show_grid();

        }

        //save
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                shareableSpreadSheet.save(saveFileDialog.FileName);
            }

        }

        //add row
        private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            if (form2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int Row = form2.Row;
                shareableSpreadSheet.addRow(Row-1);
            }
            show_grid();
        }

        //add column
        private void addColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            if (form3.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int Col = form3.Col;
                shareableSpreadSheet.addCol(Col-1);
            }
            show_grid();
        }

        //setcell
        private void setCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            if (form4.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int Row = form4.Row;
                int Col = form4.Col;
                String Str = form4.Str;
                shareableSpreadSheet.setCell(Row-1, Col-1, Str);//we started from ( 1 , 1 )
            }
            show_grid();
        }

        //clear All
        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int r = 0;
            int c = 0;
            shareableSpreadSheet.getSize(ref r, ref c);
            shareableSpreadSheet = new ShareableSpreadSheet(r, c);
            show_grid();
        }

        //search
        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search form5 = new Search();
            if (form5.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String Str = form5.Str;
                int row = 0;
                int col = 0;
                shareableSpreadSheet.searchString(Str, ref row, ref col);
                //dataGridView1[row, col].Style.BackColor = Color.Blue;
                dataGridView1.CurrentCell = dataGridView1.Rows[row+1].Cells[col+1];
                if(!shareableSpreadSheet.searchString(Str, ref row, ref col))
                {
                    MessageBox.Show(Str + " not found");
                }
            }
        }
    
        //about
        private void aboutSpreadSheetAppToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About form5 = new About();
            form5.Show();
            if (form5.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                form5.Close();
            }
        }

        //show
        private void show_grid()
        {
            //remove all the elments from
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = true;
            button2.Visible = false;
            button1.Visible = false;
            button3.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            label1.Visible = false;
            label3.Visible = false;
            label2.Visible = false;
            pictureBox1.Visible = false;
            dataGridView1.Visible = true;
            editToolStripMenuItem.Visible = true;
            toolStripMenuItem3.Visible = true;

            //show the grid
            int r = 0;
            int c = 0;
            shareableSpreadSheet.getSize(ref r, ref c);
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.ColumnCount = c;
            for (int j = 0; j < c; j++)
            {
                dataGridView1.Columns[j].Name = (j + 1).ToString();
            }
            for (int i = 0; i < r; i++)
            {
                dataGridView1.Rows.Add();
                for (int j = 0; j < c; j++)
                {
                    dataGridView1[j, i].Value = shareableSpreadSheet.getCell(i, j);
                }
            }
        }

        //handle edit event
        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                shareableSpreadSheet.setCell(e.RowIndex, e.ColumnIndex, dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString());
            }
            catch (Exception exception)
            {
                shareableSpreadSheet.setCell(e.RowIndex, e.ColumnIndex, "");
            }
        }

        //handle close event
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (shareableSpreadSheet != null)
            {
                DialogResult dialogResult = MessageBox.Show("Did you want to save the file before closing it ?", " before Closing ", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //save
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        shareableSpreadSheet.save(saveFileDialog.FileName);
                    }
                }
                else if (dialogResult == DialogResult.No)
                {
                    //nothing
                }
            }
        }


        //exchange rows
        /*
        private void exchangeRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exchangeR forum = new exchangeR();
            if (forum.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int Row1 = forum.Row1;
                int Row2 = forum.Row2;
                shareableSpreadSheet.exchangeRows(Row1, Row2);
            }
            show_grid();
        }

        //exchange columns
        private void exchangeColsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            exchangeC forum = new exchangeC();
            if (forum.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                int Col1 = forum.Col1;
                int Col2 = forum.Col2;
                shareableSpreadSheet.exchangeCols(Col1, Col2);
            }
            show_grid();

        }
        */
    }
}
