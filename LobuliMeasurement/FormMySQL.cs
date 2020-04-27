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
    public partial class FormMySQL : Form
    {
        public FormMySQL()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Config.Safe(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, textBox5.Text);
            Close();
        }
    }
}
