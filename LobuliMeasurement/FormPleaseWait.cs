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
    public partial class FormPleaseWait : Form
    {
        public FormPleaseWait(string subline = "")
        {
            InitializeComponent();

            label2.Text = subline;
        }
    }
}
