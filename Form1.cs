using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Euresys.Open_eVision_2_12;
using Excel = Microsoft.Office.Interop.Excel;

namespace Final_Project_Vehicle_Speed_Measurement
{
    public partial class Form1 : Form
    {
        EImageC24 EC24Image1 = new EImageC24(); //eVision的彩色圖像物件
        EImageC24 EC24Image2 = new EImageC24(); // EImageC24 instance
        EImageBW8 EBW8Image1 = new EImageBW8(); //eVision的灰階圖像物件
        
        ECannyEdgeDetector cannyEdgeDetector1 = new ECannyEdgeDetector(); // ECannyEdgeDetector instance

        EColorLookup EColorLookup1 = new EColorLookup(); // EColorLookup instance

        EBW8Vector In = new EBW8Vector(); // EBW8Vector instance
        EBW8Vector Out = new EBW8Vector(); // EBW8Vector instance

        string[] files;

        float ScalingRatio = 0; //Picturebox與原始影像大小的縮放比例

        static Excel.Application Excel_APP1 = new Excel.Application();
        Excel.Workbook Excel_WB1 = Excel_APP1.Workbooks.Add();
        Excel.Worksheet Excel_WS1 = new Excel.Worksheet();

        public Form1()
        {
            InitializeComponent();
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = Application.StartupPath;
            FileListBox.Items.Clear();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.jpg").OrderBy(x => x.Length).ThenBy(x => x).ToArray();
                foreach (string f in files)
                {
                    FileListBox.Items.Add(Path.GetFileName(f));
                }
            }
        }

        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel|*.xlsx";
            saveFileDialog1.Title = "Save a Excel";

            Excel_WS1 = Excel_WB1.Worksheets[1];
            Excel_WS1.Name = "Data";
            Excel_APP1.Cells[1, 1] = "圖片(.jpg)";
            Excel_APP1.Cells[2, 1] = "Profile下";
            Excel_APP1.Cells[3, 1] = "Profile上";

            EC24Image1.Load(files[0]);

            /*============================計算scaling ratio============================*/
            float PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
            float ImageSizeRatio = (float)EC24Image1.Width / EC24Image1.Height;
            if (ImageSizeRatio > PictureBoxSizeRatio)
                ScalingRatio = (float)pbImg1.Width / EC24Image1.Width;
            else
                ScalingRatio = (float)pbImg1.Height / EC24Image1.Height;
            /*=========================================================================*/

            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.SelectedIndex = i;
                FileListBox.Refresh();
                EC24Image1.Load(files[i]);
                EC24Image1.Draw(pbImg1.CreateGraphics(), ScalingRatio);

                //EC24Image2.SetSize(EC24Image1);
                //EasyImage.Oper(EArithmeticLogicOperation.Copy, new EC24(0, 0, 0), EC24Image2);

                //EC24Image1.ColorSystem = EColorSystem.Rgb;
                //EColorLookup1.ConvertFromRgb(EColorSystem.Yiq);
                //EColorLookup1.Transform(EC24Image1, EC24Image2);

                //EC24Image2.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                EBW8Image1.SetSize(EC24Image1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), EBW8Image1);
                EasyImage.Convert(EC24Image1, EBW8Image1); //轉灰階

                cannyEdgeDetector1.HighThreshold = 1.00f;
                cannyEdgeDetector1.LowThreshold = 0.20f;
                cannyEdgeDetector1.Apply(EBW8Image1, EBW8Image1);

                EBW8Image1.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                EasyImage.ImageToLineSegment(EBW8Image1, In, 1000, 1079, 1750, 1079); //設定偵測線在最底部，判斷車子是否準備進來
                EasyImage.ImageToLineSegment(EBW8Image1, Out, 1000, 0, 1750, 0); //設定偵測線在最頂部，判斷車子是否準備出去

                Excel_APP1.Cells[1, 2 + i] = Path.GetFileNameWithoutExtension(files[i]);
                Excel_APP1.Cells[2, 2 + i] = getProfileValuesSum(In);
                Excel_APP1.Cells[3, 2 + i] = getProfileValuesSum(Out);

                Console.WriteLine(files[i]);
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != "")
                Excel_WB1.SaveAs(saveFileDialog1.FileName);

            Excel_WS1 = null;
            Excel_WB1.Close();
            Excel_WB1 = null;
            Excel_APP1.Quit();
            Excel_APP1 = null;
        }

        private double getProfileValuesSum(EBW8Vector eBW8) //Profile線上的值相加起來
        {
            double sum = 0;
            for (int i = 0; i < eBW8.NumElements; i++)
            {
                sum += eBW8.GetElement(i).Value;
            }
            return sum;
        }

        private void vehicleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EC24Image1.Load(files[0]);

            /*============================計算scaling ratio============================*/
            float PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
            float ImageSizeRatio = (float)EC24Image1.Width / EC24Image1.Height;
            if (ImageSizeRatio > PictureBoxSizeRatio)
                ScalingRatio = (float)pbImg1.Width / EC24Image1.Width;
            else
                ScalingRatio = (float)pbImg1.Height / EC24Image1.Height;
            /*=========================================================================*/

            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.SelectedIndex = i;
                FileListBox.Refresh();
                EC24Image1.Load(files[i]);
                EC24Image1.Draw(pbImg1.CreateGraphics(), ScalingRatio);

                //EC24Image2.SetSize(EC24Image1);
                //EasyImage.Oper(EArithmeticLogicOperation.Copy, new EC24(0, 0, 0), EC24Image2);

                //EC24Image1.ColorSystem = EColorSystem.Rgb;
                //EColorLookup1.ConvertFromRgb(EColorSystem.Yiq);
                //EColorLookup1.Transform(EC24Image1, EC24Image2);

                //EC24Image2.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                EBW8Image1.SetSize(EC24Image1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), EBW8Image1);
                EasyImage.Convert(EC24Image1, EBW8Image1); //轉灰階

                cannyEdgeDetector1.HighThreshold = 1.00f;
                cannyEdgeDetector1.LowThreshold = 0.10f;
                cannyEdgeDetector1.Apply(EBW8Image1, EBW8Image1);

                EBW8Image1.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                Console.WriteLine(files[i]);
            }
        }

        private void FileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            EC24Image1.Load(files[0]);

            /*============================計算scaling ratio============================*/
            float PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
            float ImageSizeRatio = (float)EC24Image1.Width / EC24Image1.Height;
            if (ImageSizeRatio > PictureBoxSizeRatio)
                ScalingRatio = (float)pbImg1.Width / EC24Image1.Width;
            else
                ScalingRatio = (float)pbImg1.Height / EC24Image1.Height;
            /*=========================================================================*/

            EC24Image1.Load(files[FileListBox.SelectedIndex]);
            EC24Image1.Draw(pbImg1.CreateGraphics(), ScalingRatio);

            //EC24Image2.SetSize(EC24Image1);
            //EasyImage.Oper(EArithmeticLogicOperation.Copy, new EC24(0, 0, 0), EC24Image2);

            //EC24Image1.ColorSystem = EColorSystem.Rgb;
            //EColorLookup1.ConvertFromRgb(EColorSystem.Yiq);
            //EColorLookup1.Transform(EC24Image1, EC24Image2);

            //EC24Image2.Draw(pbImg2.CreateGraphics(), ScalingRatio);

            EBW8Image1.SetSize(EC24Image1);
            EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), EBW8Image1);
            EasyImage.Convert(EC24Image1, EBW8Image1); //轉灰階

            cannyEdgeDetector1.HighThreshold = 1.00f;
            cannyEdgeDetector1.LowThreshold = 0.10f;
            cannyEdgeDetector1.Apply(EBW8Image1, EBW8Image1);

            EBW8Image1.Draw(pbImg2.CreateGraphics(), ScalingRatio);

            Console.WriteLine(files[FileListBox.SelectedIndex]);
        }

        private void excelToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void excelToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "JPEG|*.jpg";
            saveFileDialog1.FileName = System.IO.Path.GetFileNameWithoutExtension(files[FileListBox.SelectedIndex]);
            saveFileDialog1.Title = "Save the Picture";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != "")
                EBW8Image1.Save(saveFileDialog1.FileName, EImageFileType.Jpeg);
        }
    }
}
