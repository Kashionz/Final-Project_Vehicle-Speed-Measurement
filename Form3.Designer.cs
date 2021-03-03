
namespace Final_Project_Vehicle_Speed_Measurement
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.totalFrame_lbl = new System.Windows.Forms.Label();
            this.frameCount_lbl = new System.Windows.Forms.Label();
            this.FPS_lbl = new System.Windows.Forms.Label();
            this.videoTime_lbl = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "影片總禎數：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "當前禎數：";
            // 
            // totalFrame_lbl
            // 
            this.totalFrame_lbl.AutoSize = true;
            this.totalFrame_lbl.Location = new System.Drawing.Point(105, 20);
            this.totalFrame_lbl.Name = "totalFrame_lbl";
            this.totalFrame_lbl.Size = new System.Drawing.Size(14, 15);
            this.totalFrame_lbl.TabIndex = 2;
            this.totalFrame_lbl.Text = "0";
            // 
            // frameCount_lbl
            // 
            this.frameCount_lbl.AutoSize = true;
            this.frameCount_lbl.Location = new System.Drawing.Point(105, 51);
            this.frameCount_lbl.Name = "frameCount_lbl";
            this.frameCount_lbl.Size = new System.Drawing.Size(14, 15);
            this.frameCount_lbl.TabIndex = 3;
            this.frameCount_lbl.Text = "0";
            // 
            // FPS_lbl
            // 
            this.FPS_lbl.AutoSize = true;
            this.FPS_lbl.Location = new System.Drawing.Point(105, 80);
            this.FPS_lbl.Name = "FPS_lbl";
            this.FPS_lbl.Size = new System.Drawing.Size(14, 15);
            this.FPS_lbl.TabIndex = 7;
            this.FPS_lbl.Text = "0";
            // 
            // videoTime_lbl
            // 
            this.videoTime_lbl.AutoSize = true;
            this.videoTime_lbl.Location = new System.Drawing.Point(105, 110);
            this.videoTime_lbl.Name = "videoTime_lbl";
            this.videoTime_lbl.Size = new System.Drawing.Size(14, 15);
            this.videoTime_lbl.TabIndex = 6;
            this.videoTime_lbl.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "影片時間：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(63, 80);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "FPS：";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 143);
            this.Controls.Add(this.FPS_lbl);
            this.Controls.Add(this.videoTime_lbl);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.frameCount_lbl);
            this.Controls.Add(this.totalFrame_lbl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form3";
            this.Text = "VideoInfo";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label totalFrame_lbl;
        private System.Windows.Forms.Label frameCount_lbl;
        private System.Windows.Forms.Label FPS_lbl;
        private System.Windows.Forms.Label videoTime_lbl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
    }
}