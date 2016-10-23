using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OMK
{
    public partial class FormInterpreterDialog : Form
    {
        public FormInterpreterDialog()
        {
            InitializeComponent();
        }
        public String dialogText
        {
            get {return label.Text;}
            set {label.Text = value;}
        }
        public double Value
        {
            get { return double.Parse(textBox.Text); }
            set { textBox.Text = value.ToString(); }
        }
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double.Parse(textBox.Text);
                textBox.ForeColor = System.Drawing.Color.Black;
            }
            catch (Exception ex)
            {
                textBox.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
