namespace WindowsFormsApplicationVideoReader
{
    partial class Form1
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
            this.Time_Label = new System.Windows.Forms.Label();
            this.Frame_lbl = new System.Windows.Forms.Label();
            this.Video_CNTRL = new System.Windows.Forms.HScrollBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonPlayStop = new System.Windows.Forms.Button();
            this.labelPosition = new System.Windows.Forms.Label();
            this.buttonStream = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.openFileDialogImage = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // Time_Label
            // 
            this.Time_Label.AutoSize = true;
            this.Time_Label.Location = new System.Drawing.Point(23, 28);
            this.Time_Label.Name = "Time_Label";
            this.Time_Label.Size = new System.Drawing.Size(51, 20);
            this.Time_Label.TabIndex = 0;
            this.Time_Label.Text = "label1";
            // 
            // Frame_lbl
            // 
            this.Frame_lbl.AutoSize = true;
            this.Frame_lbl.Location = new System.Drawing.Point(27, 52);
            this.Frame_lbl.Name = "Frame_lbl";
            this.Frame_lbl.Size = new System.Drawing.Size(51, 20);
            this.Frame_lbl.TabIndex = 1;
            this.Frame_lbl.Text = "label1";
            // 
            // Video_CNTRL
            // 
            this.Video_CNTRL.Location = new System.Drawing.Point(606, 52);
            this.Video_CNTRL.Name = "Video_CNTRL";
            this.Video_CNTRL.Size = new System.Drawing.Size(529, 26);
            this.Video_CNTRL.TabIndex = 3;
            this.Video_CNTRL.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Video_CNTRL_Scroll);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // buttonOpen
            // 
            this.buttonOpen.Location = new System.Drawing.Point(214, 28);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(149, 66);
            this.buttonOpen.TabIndex = 4;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            this.buttonOpen.Click += new System.EventHandler(this.openVideoToolStripMenuItem_Click);
            // 
            // buttonPlayStop
            // 
            this.buttonPlayStop.Location = new System.Drawing.Point(401, 22);
            this.buttonPlayStop.Name = "buttonPlayStop";
            this.buttonPlayStop.Size = new System.Drawing.Size(179, 79);
            this.buttonPlayStop.TabIndex = 5;
            this.buttonPlayStop.Text = "Play Stop";
            this.buttonPlayStop.UseVisualStyleBackColor = true;
            this.buttonPlayStop.Click += new System.EventHandler(this.buttonPlayStop_Click);
            this.buttonPlayStop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.play_pause_BTN_MouseUp);
            // 
            // labelPosition
            // 
            this.labelPosition.AutoSize = true;
            this.labelPosition.Location = new System.Drawing.Point(747, 22);
            this.labelPosition.Name = "labelPosition";
            this.labelPosition.Size = new System.Drawing.Size(18, 20);
            this.labelPosition.TabIndex = 6;
            this.labelPosition.Text = "0";
            // 
            // buttonStream
            // 
            this.buttonStream.Location = new System.Drawing.Point(401, 123);
            this.buttonStream.Name = "buttonStream";
            this.buttonStream.Size = new System.Drawing.Size(179, 71);
            this.buttonStream.TabIndex = 7;
            this.buttonStream.Text = "Stream";
            this.buttonStream.UseVisualStyleBackColor = true;
            this.buttonStream.Click += new System.EventHandler(this.buttonStream_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1211, 70);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(1211, 99);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(134, 80);
            this.buttonSave.TabIndex = 9;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // openFileDialogImage
            // 
            this.openFileDialogImage.FileName = "openFileDialog2";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(1390, 940);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonStream);
            this.Controls.Add(this.labelPosition);
            this.Controls.Add(this.buttonPlayStop);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.Video_CNTRL);
            this.Controls.Add(this.Frame_lbl);
            this.Controls.Add(this.Time_Label);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1 = new System.Windows.Forms.Button();
        private Emgu.CV.UI.ImageBox Video_Image = new Emgu.CV.UI.ImageBox();
        private System.Windows.Forms.TextBox Codec_lbl = new System.Windows.Forms.TextBox();
        private System.Windows.Forms.Label Time_Label;
        private System.Windows.Forms.Label Frame_lbl;
        private System.Windows.Forms.HScrollBar Video_CNTRL;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonPlayStop;
        private System.Windows.Forms.Label labelPosition;
        private System.Windows.Forms.Button buttonStream;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.OpenFileDialog openFileDialogImage;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}

