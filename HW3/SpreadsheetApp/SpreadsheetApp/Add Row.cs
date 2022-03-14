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
    public partial class Form2 : Form
    {
        public bool clicked = false;
        public int Row;
        public Form2()
        {
            InitializeComponent();
            this.AcceptButton = button1;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            clicked = true;
            Row = Int32.Parse(textBox1.Text);
            this.DialogResult = DialogResult.OK;
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public bool Clicked()
        {
            return clicked;
        }
    }
}
