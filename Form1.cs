using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Euresys.Open_eVision_2_12;
using Excel = Microsoft.Office.Interop.Excel;

namespace Final_Project_Vehicle_Speed_Measurement
{
    public partial class Form1 : Form
    {
        Form2 settings = new Form2();
        Form3 videoInfo = new Form3();

        EImageC24 OriginalImg1 = new EImageC24(); //eVision的彩色圖像物件
        EImageC24 OriginalImg2 = new EImageC24(); // EImageC24 instance
        EImageC24 Background = new EImageC24(); //eVision的灰階圖像物件
        EImageBW8 GrayImg1 = new EImageBW8(); //eVision的灰階圖像物件
        EImageBW8 GrayImg2 = new EImageBW8(); //eVision的灰階圖像物件
        EImageBW8 BackgroundGray = new EImageBW8(); //eVision的灰階圖像物件

        EColorLookup EColorLookup1 = new EColorLookup(); // EColorLookup instance

        EBW8Vector In = new EBW8Vector(); // EBW8Vector instance
        EBW8Vector Out = new EBW8Vector(); // EBW8Vector instance

        EMatcher EMatcher1 = new EMatcher();

        EROIBW8 EBW8Image1Roi1 = new EROIBW8();

        ECodedImage2 codedImage1 = new ECodedImage2(); // ECodedImage2 instance
        EImageEncoder codedImage1Encoder = new EImageEncoder(); // EImageEncoder instance
        EObjectSelection codedImage1ObjectSelection = new EObjectSelection(); // EObjectSelection instance

        BitmapData bmpData = null;
        Bitmap bitmap = null;

        Capture video;
        public bool play = false;

        double fps, totalframe, videotime, framecount;

        string[] files;

        float ScalingRatio = 0; //Picturebox與原始影像大小的縮放比例
        bool selecting = false;

        static Excel.Application Excel_APP1;
        Excel.Workbook Excel_WB1;
        Excel.Worksheet Excel_WS1;

        public Form1()
        {
            InitializeComponent();
        }

        private void folderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = "C:\\Users\\kashi\\Desktop\\畢業專題\\測試資料\\";
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
            Excel_APP1 = new Excel.Application();
            Excel_WB1 = Excel_APP1.Workbooks.Add();
            Excel_WS1 = new Excel.Worksheet();

            saveFileDialog1.Filter = "Excel|*.xlsx";
            saveFileDialog1.Title = "Save a Excel";

            Excel_WS1 = Excel_WB1.Worksheets[1];
            Excel_WS1.Name = "Data";
            Excel_APP1.Cells[1, 1] = "圖片(.jpg)";
            Excel_APP1.Cells[1, 2] = "Profile下";
            Excel_APP1.Cells[1, 3] = "Profile上";

            OriginalImg1.Load(files[0]);

            /*============================計算scaling ratio============================*/
            float PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
            float ImageSizeRatio = (float)OriginalImg1.Width / OriginalImg1.Height;
            if (ImageSizeRatio > PictureBoxSizeRatio)
                ScalingRatio = (float)pbImg1.Width / OriginalImg1.Width;
            else
                ScalingRatio = (float)pbImg1.Height / OriginalImg1.Height;
            /*=========================================================================*/

            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.SelectedIndex = i;
                FileListBox.Refresh();
                OriginalImg1.Load(files[i]);
                OriginalImg1.Draw(pbImg1.CreateGraphics(), ScalingRatio);

                //EC24Image2.SetSize(EC24Image1);
                //EasyImage.Oper(EArithmeticLogicOperation.Copy, new EC24(0, 0, 0), EC24Image2);

                //EC24Image1.ColorSystem = EColorSystem.Rgb;
                //EColorLookup1.ConvertFromRgb(EColorSystem.Yiq);
                //EColorLookup1.Transform(EC24Image1, EC24Image2);

                //EC24Image2.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                GrayImg1.SetSize(OriginalImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), GrayImg1);
                EasyImage.Convert(OriginalImg1, GrayImg1); //轉灰階

                //EasyImage.Oper(EArithmeticLogicOperation.Subtract, GrayImg1, BackgroundGray, GrayImg1);
                //EasyImage.Threshold(GrayImg1, GrayImg1, 56);
                //EasyImage.OpenBox(GrayImg1, GrayImg1, settings.set_value_3());
                
                GrayImg1.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                EasyImage.ImageToLineSegment(GrayImg1, In, 1485, 700, 1683, 700); //設定車子進入的偵測線，判斷車子是否準備進來
                EasyImage.ImageToLineSegment(GrayImg1, Out, 1485, 400, 1683, 400); //設定車子出去的偵測線，判斷車子是否準備出去

                Excel_APP1.Cells[2 + i, 1] = Path.GetFileNameWithoutExtension(files[i]);
                Excel_APP1.Cells[2 + i, 2] = getProfileValuesSum(In);
                Excel_APP1.Cells[2 + i, 3] = getProfileValuesSum(Out);

                //Console.WriteLine(files[i]);
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
            float PictureBoxSizeRatio, ImageSizeRatio;

            codedImage1ObjectSelection.FeretAngle = 0.00f;
            codedImage1Encoder.GrayscaleSingleThresholdSegmenter.WhiteLayerEncoded = true;
            codedImage1Encoder.GrayscaleSingleThresholdSegmenter.BlackLayerEncoded = false;
            codedImage1Encoder.SegmentationMethod = ESegmentationMethod.GrayscaleSingleThreshold;
            codedImage1Encoder.GrayscaleSingleThresholdSegmenter.Mode = EGrayscaleSingleThreshold.MinResidue;

            OriginalImg1.Load(files[0]);

            /*============================計算scaling ratio============================*/
            PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
            ImageSizeRatio = (float)OriginalImg1.Width / OriginalImg1.Height;
            if (ImageSizeRatio > PictureBoxSizeRatio)
                ScalingRatio = (float)pbImg1.Width / OriginalImg1.Width;
            else
                ScalingRatio = (float)pbImg1.Height / OriginalImg1.Height;
            /*=========================================================================*/

            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.SelectedIndex = i;
                FileListBox.Refresh();
                OriginalImg1.Load(files[i]);
                OriginalImg1.Draw(pbImg1.CreateGraphics(), ScalingRatio);

                GrayImg1.SetSize(OriginalImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), GrayImg1);
                EasyImage.Convert(OriginalImg1, GrayImg1); //轉灰階

                EasyImage.Median(BackgroundGray, BackgroundGray);
                EasyImage.Median(GrayImg1, GrayImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Subtract, GrayImg1, BackgroundGray, GrayImg1);

                EasyImage.Threshold(GrayImg1, GrayImg1, unchecked((uint)EThresholdMode.MinResidue));

                EasyImage.ErodeBox(GrayImg1, GrayImg1, 1); //侵蝕
                EasyImage.CloseBox(GrayImg1, GrayImg1, 10); //閉合

                codedImage1ObjectSelection.FeretAngle = 0.00f;
                codedImage1Encoder.Encode(GrayImg1, codedImage1);
                codedImage1ObjectSelection.Clear();
                codedImage1ObjectSelection.AddObjects(codedImage1);
                codedImage1ObjectSelection.AttachedImage = GrayImg1;
                codedImage1ObjectSelection.RemoveUsingUnsignedIntegerFeature(EFeature.RunCount, 1000, ESingleThresholdMode.Less); //移除RunCount小於1000的物件

                if(codedImage1ObjectSelection.ElementCount > 0)
                Console.WriteLine("(" + codedImage1ObjectSelection.GetElement(0).BoundingBoxCenterX + ", " + codedImage1ObjectSelection.GetElement(0).BoundingBoxCenterY + ")");

                codedImage1.DrawFeature(pbImg1.CreateGraphics(), EDrawableFeature.BoundingBox, codedImage1ObjectSelection, ScalingRatio); // 把車的框框畫出來

                GrayImg1.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                Console.WriteLine(files[i]);
            }
        }

        private void FileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selecting)
            {
                OriginalImg1.Load(files[0]);

                //============================計算scaling ratio============================
                float PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
                float ImageSizeRatio = (float)OriginalImg1.Width / OriginalImg1.Height;
                if (ImageSizeRatio > PictureBoxSizeRatio)
                    ScalingRatio = (float)pbImg1.Width / OriginalImg1.Width;
                else
                    ScalingRatio = (float)pbImg1.Height / OriginalImg1.Height;
                //=========================================================================

                OriginalImg1.Load(files[FileListBox.SelectedIndex]);
                OriginalImg1.Draw(pbImg1.CreateGraphics(), ScalingRatio);

                GrayImg1.SetSize(OriginalImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), GrayImg1);
                EasyImage.Convert(OriginalImg1, GrayImg1); //轉灰階

                EasyImage.Median(BackgroundGray, BackgroundGray);
                EasyImage.Median(GrayImg1, GrayImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Subtract, GrayImg1, BackgroundGray, GrayImg1);

                EasyImage.Threshold(GrayImg1, GrayImg1, unchecked((uint)EThresholdMode.MinResidue));

                EasyImage.ErodeBox(GrayImg1, GrayImg1, 1); //侵蝕
                EasyImage.CloseBox(GrayImg1, GrayImg1, 10); //閉合

                GrayImg1.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                Console.WriteLine(files[FileListBox.SelectedIndex]);
            }
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
                GrayImg1.Save(saveFileDialog1.FileName, EImageFileType.Jpeg);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.Show();
        }

        private void grayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.SelectedItem = i;
                FileListBox.Refresh();
                OriginalImg1.Load(files[i]);
                GrayImg1.SetSize(OriginalImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), GrayImg1);
                EasyImage.Convert(OriginalImg1, GrayImg1);

                string path = settings.set_path() + "\\" + Path.GetFileName(files[i]);

                if (!Directory.Exists(settings.set_path()))
                    Directory.CreateDirectory(settings.set_path());

                GrayImg1.SaveJpeg(path);
            }

            MessageBox.Show("灰階化轉換完成", "通知");
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void yIQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < FileListBox.Items.Count; i++)
            {
                FileListBox.SelectedItem = i;
                FileListBox.Refresh();
                OriginalImg1.Load(files[i]);

                OriginalImg2.SetSize(OriginalImg1);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EC24(0, 0, 0), OriginalImg2);

                OriginalImg1.ColorSystem = EColorSystem.Rgb;
                EColorLookup1.ConvertFromRgb(EColorSystem.Yiq);
                EColorLookup1.Transform(OriginalImg1, OriginalImg2);

                //EC24Image2.Draw(pbImg2.CreateGraphics(), ScalingRatio);

                string path = settings.set_path() + "\\" + Path.GetFileName(files[i]);

                if (!Directory.Exists(settings.set_path()))
                    Directory.CreateDirectory(settings.set_path());

                OriginalImg2.SaveJpeg(path);
            }

            MessageBox.Show("YIQ轉換完成", "通知");
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            float PictureBoxSizeRatio, ImageSizeRatio;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Background.Load(openFileDialog1.FileName);
                PictureBoxSizeRatio = (float)pbImg3.Width / pbImg3.Height;
                ImageSizeRatio = (float)Background.Width / Background.Height;
                if (ImageSizeRatio > PictureBoxSizeRatio)
                    ScalingRatio = (float)pbImg3.Width / Background.Width;
                else
                    ScalingRatio = (float)pbImg3.Height / Background.Height;

                //顯示影像於Picturebox
                pbImg3.Refresh(); //先清除目前圖像
                Background.Draw(pbImg3.CreateGraphics(), ScalingRatio); //再繪製上去

                BackgroundGray.SetSize(Background);
                EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), BackgroundGray);
                EasyImage.Convert(Background, BackgroundGray); //轉灰階
            }
        }

        private void selectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selecting)
                selecting = false;
            else
                selecting = true;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            codedImage1ObjectSelection.Clear();
            pbImg1.Refresh();
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            play = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                video = new Capture(openFileDialog1.FileName); //讀影片
                Mat m = new Mat();
                video.Retrieve(m); //擷取影片首個frame
                pbImg1.Image = m.Bitmap; //顯示該frame，當預覽圖
                totalframe = video.GetCaptureProperty(CapProp.FrameCount);
                videoInfo.setValue(totalframe, 0, 0, 0);
                videoInfo.Show();
            }
        }

        private async void videoPlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (video == null) return;

            if (!play)
            {
                play = true;
                videoPlayToolStripMenuItem.Text = "VideoPause";
            }
            else
            {
                play = false;
                videoPlayToolStripMenuItem.Text = "VideoPlay";
            }

            try
            {
                while (play)
                {
                    Mat frame = new Mat();

                    frame = video.QueryFrame(); //擷取影片frame

                    if (frame == null) break;

                    pbImg1.Image = frame.Bitmap; //顯示frame

                    fps = video.GetCaptureProperty(CapProp.Fps); //抓影片的fps
                    videotime = Math.Floor(video.GetCaptureProperty(CapProp.PosMsec)) / 1000; //抓影片時間
                    framecount = video.GetCaptureProperty(CapProp.PosFrames);

                    Bitmap bitmap_source = (Bitmap)frame.Bitmap;

                    if (bitmap == null)
                        bitmap = (Bitmap)bitmap_source.Clone();

                    bitmap = bitmap_source;

                    if (bitmap == null)
                        return;

                    OriginalImg1 = BitmapToEImageC24(ref bitmap);

                    GrayImg1.SetSize(OriginalImg1);
                    EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), GrayImg1);
                    EasyImage.Convert(OriginalImg1, GrayImg1); //轉灰階

                    ShowImage(GrayImg1, pbImg2);

                    bitmap.Dispose();                
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    videoInfo.setValue(totalframe, framecount, videotime, fps);

                    if (framecount == totalframe)
                        break;

                    await Task.Delay(1000 / Convert.ToInt32(fps)); //延遲
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private EImageC24 BitmapToEImageC24(ref Bitmap bitmap)
        {
            EImageC24 EC24Image1 = null; 
            try
            {
                EC24Image1 = new EImageC24();

                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

                EC24Image1.SetImagePtr(bitmap.Width, bitmap.Height, bmpData.Scan0);
                bitmap.UnlockBits(bmpData);

            }
            catch (EException e) //EException為eVision的例外處理
            {             
                Console.WriteLine(e.ToString());        
            }        
            return EC24Image1;
        } 

        private void ShowImage(EImageC24 img, PictureBox pb)
        {
            try 
            {
                Bitmap bmp;
                bmp = new Bitmap(pb.Width, pb.Height);

                float PictureBoxSizeRatio = (float)pb.Width / pb.Height;
                float ImageSizeRatio = (float)img.Width / img.Height;
                if (ImageSizeRatio > PictureBoxSizeRatio)
                    ScalingRatio = (float)pb.Width / img.Width;
                else
                    ScalingRatio = (float)pb.Height / img.Height;

                if(pb.InvokeRequired)
                {
                    pb.Invoke(new MethodInvoker(delegate () {
                        img.Draw(Graphics.FromImage(bmp), ScalingRatio);
                        pb.Image = bmp;
                    }));
                }
                else
                {
                    img.Draw(Graphics.FromImage(bmp), ScalingRatio);
                    pb.Image = bmp;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async void excelToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Excel_APP1 = new Excel.Application();
            Excel_WB1 = Excel_APP1.Workbooks.Add();
            Excel_WS1 = new Excel.Worksheet();

            saveFileDialog1.Filter = "Excel|*.xlsx";
            saveFileDialog1.Title = "Save a Excel";

            Excel_WS1 = Excel_WB1.Worksheets[1];
            Excel_WS1.Name = "Data";
            Excel_APP1.Cells[1, 1] = "圖片(.jpg)";
            Excel_APP1.Cells[1, 2] = "Profile下";
            Excel_APP1.Cells[1, 3] = "Profile上";

            try
            {
                while (framecount != totalframe)
                {
                    Mat frame = new Mat();

                    frame = video.QueryFrame(); //擷取影片frame

                    if (frame == null) break;

                    pbImg1.Image = frame.Bitmap; //顯示frame

                    fps = video.GetCaptureProperty(CapProp.Fps); //抓影片的fps
                    videotime = Math.Floor(video.GetCaptureProperty(CapProp.PosMsec)) / 1000; //抓影片時間
                    framecount = video.GetCaptureProperty(CapProp.PosFrames);

                    Bitmap bitmap_source = (Bitmap)frame.Bitmap;

                    if (bitmap == null)
                        bitmap = (Bitmap)bitmap_source.Clone();

                    bitmap = bitmap_source;

                    if (bitmap == null)
                        return;

                    OriginalImg1 = BitmapToEImageC24(ref bitmap);

                    GrayImg1.SetSize(OriginalImg1);
                    EasyImage.Oper(EArithmeticLogicOperation.Copy, new EBW8(0), GrayImg1);
                    EasyImage.Convert(OriginalImg1, GrayImg1); //轉灰階

                    ShowImage(GrayImg1, pbImg2);

                    EasyImage.ImageToLineSegment(GrayImg1, In, 1485, 700, 1683, 700); //設定車子進入的偵測線，判斷車子是否準備進來
                    EasyImage.ImageToLineSegment(GrayImg1, Out, 1485, 400, 1683, 400); //設定車子出去的偵測線，判斷車子是否準備出去

                    Excel_APP1.Cells[framecount + 1, 1] = framecount.ToString();
                    Excel_APP1.Cells[framecount + 1, 2] = getProfileValuesSum(In);
                    Excel_APP1.Cells[framecount + 1, 3] = getProfileValuesSum(Out);

                    bitmap.Dispose();
                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();

                    videoInfo.setValue(totalframe, framecount, videotime, fps);

                    await Task.Delay(1000 / Convert.ToInt32(fps)); //延遲
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            if (saveFileDialog1.ShowDialog() == DialogResult.OK && saveFileDialog1.FileName != "")
                Excel_WB1.SaveAs(saveFileDialog1.FileName);

            Excel_WS1 = null;
            Excel_WB1.Close();
            Excel_WB1 = null;
            Excel_APP1.Quit();
            Excel_APP1 = null;
        }

        private void ShowImage(EImageBW8 img, PictureBox pb)
        {
            try
            {
                Bitmap bmp;
                bmp = new Bitmap(pb.Width, pb.Height);

                float PictureBoxSizeRatio = (float)pb.Width / pb.Height;
                float ImageSizeRatio = (float)img.Width / img.Height;
                if (ImageSizeRatio > PictureBoxSizeRatio)
                    ScalingRatio = (float)pb.Width / img.Width;
                else
                    ScalingRatio = (float)pb.Height / img.Height;

                if (pb.InvokeRequired)
                {
                    pb.Invoke(new MethodInvoker(delegate () {
                        img.Draw(Graphics.FromImage(bmp), ScalingRatio);
                        pb.Image = bmp;
                    }));
                }
                else
                {
                    img.Draw(Graphics.FromImage(bmp), ScalingRatio);
                    pb.Image = bmp;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            float PictureBoxSizeRatio, ImageSizeRatio;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OriginalImg1.Load(openFileDialog1.FileName);
                PictureBoxSizeRatio = (float)pbImg1.Width / pbImg1.Height;
                ImageSizeRatio = (float)OriginalImg1.Width / OriginalImg1.Height;
                if (ImageSizeRatio > PictureBoxSizeRatio)
                    ScalingRatio = (float)pbImg1.Width / OriginalImg1.Width;
                else
                    ScalingRatio = (float)pbImg1.Height / OriginalImg1.Height;

                //顯示影像於Picturebox
                pbImg1.Refresh(); //先清除目前圖像
                OriginalImg1.Draw(pbImg1.CreateGraphics(), ScalingRatio); //再繪製上去
            }
        }
    }
}
