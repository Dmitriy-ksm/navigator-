﻿using System;
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
    public partial class TransportCost : Form
    {
        public TransportCost()
        {
            InitializeComponent();
        }
        public string getData()
        {
            return textBox1.Text + "/"+textBox2.Text + "/" + textBox3.Text + "/" + textBox4.Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}
