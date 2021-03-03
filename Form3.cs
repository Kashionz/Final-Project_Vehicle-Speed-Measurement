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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        public void setValue(double totalFrame, double frameCount, double videoTime, double FPS)
        {
            totalFrame_lbl.Text = totalFrame.ToString();
            frameCount_lbl.Text = frameCount.ToString();
            videoTime_lbl.Text = videoTime.ToString();
            FPS_lbl.Text = FPS.ToString();
        }
    }
}
