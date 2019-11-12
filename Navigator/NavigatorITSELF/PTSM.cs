using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Navigator
{
    public partial class PTSM : Form
    {
        public PTSM()
        {
            InitializeComponent();
        }

        public string getData()
        {
            return textBox4.Text;
        }
        public void setData(long point_id)
        {
            textBox1.Text = point_id.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox4.Text="<"+textBox2.Text+" point=\""+textBox1.Text+ "\" count=\"" + textBox3.Text + "\" avarage_time=\"" + textBox5.Text+"\" >";
            Close();
            Dispose();
        }
    }
}
