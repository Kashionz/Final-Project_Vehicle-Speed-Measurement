using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Project_Vehicle_Speed_Measurement
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        public float set_value_1()
        {
            return float.Parse(textBox1.Text);
        }

        public float set_value_2()
        {
            return float.Parse(textBox2.Text);
        }

        public string set_path()
        {
            return textBox3.Text;
        }
    }
}
