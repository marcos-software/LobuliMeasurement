using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormNote : Form
    {
        public string annotation { get; set; }
        public FormNote()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            annotation = textBox1.Text;
            this.DialogResult = DialogResult.OK;
        }
    }
}
