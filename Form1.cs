using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplicationVideoReader
{

    public partial class Form1 : Form
    {
        #region Variables
        //file IO variable
        OpenFileDialog OF = new OpenFileDialog();
        SaveFileDialog SF = new SaveFileDialog();

        //current video mode and state
        bool playstate = false;
        bool recordstate = false;

        VideoMethod CurrentState = VideoMethod.Viewing; //default state
        public enum VideoMethod
        {
            Viewing,
            Recording
        };

        //Capture Setting and variables
        Capture _Capture;

        double FrameRate = 0;
        double TotalFrames = 0;

        VideoWriter VW;
        Stopwatch SW;

        int Frame_width;
        int Frame_Height;
        int FrameCount;


        #endregion

        /// <summary>
        /// Initialises controls on Form1
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Processes Each frame according to CurrentState Viewing/Recording Video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void ProcessFrame(object sender, EventArgs arg)
        {
            if (CurrentState == VideoMethod.Viewing)
            {
                try
                {
                    //Show image
                 Mat img =   _Capture.QueryFrame();
                    Video_Image.Image = img;
                    
                //    DisplayImage(_Capture.QueryFrame());

                    //Show time stamp
                    double FrameRate = _Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                    if (FrameRate.Equals( double.NaN))
                    {
                        FrameRate = 25;
                    }
                    double time_index = _Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosMsec);//  Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_MSEC);
                    UpdateTextBox("Time: " + TimeSpan.FromMilliseconds(time_index).ToString(), Time_Label);

                    //show frame number
                    double framenumber = _Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames); //_Capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES);
                    UpdateTextBox("Frame: " + framenumber.ToString(), Frame_lbl);

                    //update trackbar
                    UpdateVideo_CNTRL(framenumber);

                    /*Note: We can increase or decrease this delay to fastforward of slow down the display rate
                     if we want a re-wind function we would have to use _Capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES, FrameNumber*);
                    //and call the process frame to update the picturebox ProcessFrame(null, null);. This is more complicated.*/

                    //Wait to display correct framerate
                    Thread.Sleep((int)(1000.0 / FrameRate)); //This may result in fast playback if the codec does not tell the truth
                    Application.DoEvents();

                    //Lets check to see if we have reached the end of the video
                    //If we have lets stop the capture and video as in pause button was pressed
                    //and reset the video back to start
                    if (framenumber == TotalFrames)
                    {
                        //pause button update
                      //  play_pause_BTN_MouseUp(null, null);

                        framenumber = 0;
                        UpdateVideo_CNTRL(framenumber);
                        _Capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, framenumber);
                        //call the process frame to update the picturebox
                   //     ProcessFrame(null, null);
                    }
                }
                catch
                {
                }
            }
            if (CurrentState == VideoMethod.Recording)
            {
                // Image<Bgr, Byte> frame = _Capture.RetrieveBgrFrame(); //capture to a Image variable so we can use it for writing to the VideoWriter
                Mat frame = _Capture.QueryFrame();//.RetrieveBgrFrame(); //capture to a Image variable so we can use it for writing to the VideoWriter
                DisplayImage(_Capture.QueryFrame()); //Show the image

                //if we wanted to compresse the image to a smaller size to save space on our video we could use
                //frame.Resize(100,100, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR)
                //But the VideoWriter must be set up with the correct size

                if (recordstate && VW.Ptr != IntPtr.Zero)
                {
                    VW.Write(frame); //If we are recording and videowriter is avaliable add the image to the videowriter 
                    //Update frame number
                    FrameCount++;
                    UpdateTextBox("Frame: " + FrameCount.ToString(), Frame_lbl);

                    //Show time stamp or there abouts
                    UpdateTextBox("Time: " + TimeSpan.FromMilliseconds(SW.ElapsedMilliseconds).ToString(), Time_Label);
                }
            }
        }

        /// <summary>
        /// Play/Pause video or Record/Stop video from default video device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void play_pause_BTN_MouseUp(object sender, MouseEventArgs e)
        {
            //Check to ensure a video file is selected or video capture device is available
            if (_Capture != null)
            {
                if (CurrentState == VideoMethod.Viewing)
                {
                    playstate = !playstate; //change playstate to the opposite
                    /*Update Play panel image*/
                    if (playstate)
                    {
                      //  play_pause_BTN.BackgroundImage = VideoCapture.Properties.Resources.Pause;
                        UpdateVideo_CNTRL(false); //disable this as it's not safe when running 
                        //this may work in legacy call method and be cause by a cross threading issue
                        _Capture.Start();
                    }
                    else
                    {
                      //  play_pause_BTN.BackgroundImage = VideoCapture.Properties.Resources.Play;
                        _Capture.Pause();
                        UpdateVideo_CNTRL(true);
                    }
                }
                else if (CurrentState == VideoMethod.Recording)
                {
                    recordstate = !recordstate; //change playstate to the opposite
                    /*Update Play panel image*/
                    if (recordstate)
                    {
                        //Set up image/varibales
                    //    play_pause_BTN.BackgroundImage = VideoCapture.Properties.Resources.Stop;
                        FrameCount = 0;
                        SW = new Stopwatch();
                        //check to see if we have disposed of the video before
                        if (VW.Ptr == IntPtr.Zero)
                        {
                            //explain to the user what's happening
                            MessageBox.Show("VideoWriter has been finilised, please re-initalise a video file");
                            //lets re-call the recordVideoToolStripMenuItem_Click to save on programing
                            recordVideoToolStripMenuItem_Click(null, null);
                        }
                        SW.Start();
                    }
                    else
                    {
                        //Stop recording and dispose of the VideoWriter this will finialise the video
                        //Some codecs don't dispose correctley use uncompressed to stop this error
                        //VLC video player will play videos where the index has been corrupted. http://www.videolan.org/vlc/index.html
                        VW.Dispose();

                        //set image/variable
                //        play_pause_BTN.BackgroundImage = VideoCapture.Properties.Resources.Record;
                        SW.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Changes the current frame from what the video will start from or look at 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Video_CNTRL_MouseCaptureChanged(object sender, EventArgs e)
        {
            //if (_Capture != null)
            //{

            //    //we don't use this when running since it has an unstable call and wil cause a crash
            //    if (_Capture.GrabProcessState == System.Threading.ThreadState.Running)
            //    {
            //        _Capture.Pause();
            //        while (_Capture.GrabProcessState == System.Threading.ThreadState.Running) ;//do nothing wait for stop
            //        _Capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES, Video_CNTRL.Value);
            //        _Capture.Start();
            //    }
            //    else
            //    {
            //        _Capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_POS_FRAMES, Video_CNTRL.Value);
            //        //call the process frame to update the picturebox
            //        ProcessFrame(null, null);
            //    }

            //}

        }

        /// <summary>
        /// Open a video to play
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //set up filter
            OF.Filter = "all | *";// "Video Files|*.avi;*.mp4;*.mpg";
            //open file dialog to select file
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //dispose of old capture if one exists
                if (_Capture != null)
                {
                     _Capture.Dispose();//dispose of current capture
                }
                try
                {
                    this.Text = "Viewing Video: " + OF.FileName; //display the viewing method and location

                    //set the current video state
                    CurrentState = VideoMethod.Viewing;

                    //set up new capture and get video information
                    _Capture = new Capture(OF.FileName);
                  //  _Capture.ImageGrabbed += ProcessFrame; //attache event call to process frames

                    //Get information about the video file
                    FrameRate = _Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);// (Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS);
                    TotalFrames = _Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);// (Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_COUNT);
                    //The four_cc returns a double so we must convert it
                    double codec_double = _Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FourCC);// (Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FOURCC);

                    //for more on fourcc video discriptors
                    /* http://www.fourcc.org/codecs.php */

                    //step by step
                    //UInt32 Udouble = Convert.ToUInt32(codec_double);
                    //byte[] bytes = BitConverter.GetBytes(Udouble);
                    //char[] char_array = System.Text.Encoding.UTF8.GetString(bytes).ToCharArray();
                    //string s = new string(char_array);


                    //Debug
                    if (FrameRate.Equals(double.NaN))
                    {
                        FrameRate = 25;
                    }

                    if (TotalFrames < 0)
                    {
                        TotalFrames = 1000;
                    }
                    //or in one
                    string s = new string(System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(Convert.ToUInt32(codec_double))).ToCharArray());
                    Codec_lbl.Text = "Codec: " + s;

                    //set up the trackerbar 
                    UpdateVideo_CNTRL(true); //re-enable incase it is disabled by record video
                    Video_CNTRL.Minimum = 0;
                    Video_CNTRL.Maximum = (int)TotalFrames;

                    //set up the button and images 
              //      play_pause_BTN.BackgroundImage = VideoCapture.Properties.Resources.Play;
               //     playstate = true;
                //    _Capture.Start();
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
        }

        /// <summary>
        /// Open a Capture device to record from
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void recordVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //set up filter
            SF.Filter = "Video Files|*.avi;*.mp4;*.mpg";
            //Get information about the video file save location
            if (SF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //check to see if capture exists if it does dispose of it
                if (_Capture != null)
                {
              //      if (_Capture.GrabProcessState == System.Threading.ThreadState.Running) _Capture.Stop(); //Stop urrent capture if running 
                    _Capture.Dispose();//dispose of current capture
                }
                try
                {
                    //record the save location
                    this.Text = "Saving Video: " + SF.FileName; //display the save method and location

                    //set the current video state
                    CurrentState = VideoMethod.Recording;

                    //set up new capture
                    _Capture = new Capture(); //Use the default device 
              //      _Capture.ImageGrabbed += ProcessFrame; //attach event call to process frames

                    //get/set the capture video information

                    Frame_width = (int)_Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth);// (Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH);
                    Frame_Height = (int)_Capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight);// (Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT);

                    FrameRate = 15; //Set the framerate manually as a camera would retun 0 if we use GetCaptureProperty()

                    //Set up a video writer component
                    /*                                        ---USE----
                    /* VideoWriter(string fileName, int compressionCode, int fps, int width, int height, bool isColor)
                     *
                     * Compression code. 
                     *      Usually computed using CvInvoke.CV_FOURCC. On windows use -1 to open a codec selection dialog. 
                     *      On Linux, use CvInvoke.CV_FOURCC('I', 'Y', 'U', 'V') for default codec for the specific file name. 
                     * 
                     * Compression code. 
                     *      -1: allows the user to choose the codec from a dialog at runtime 
                     *       0: creates an uncompressed AVI file (the filename must have a .avi extension) 
                     *
                     * isColor.
                     *      true if this is a color video, false otherwise
                     */
                 //   VW = new VideoWriter(@SF.FileName, -1, (int)FrameRate, Frame_width, Frame_Height, true);
                    VW = new VideoWriter(@SF.FileName, -1, (int)FrameRate,new Size( Frame_width, Frame_Height), true);

                    //set up the trackerbar 
                    UpdateVideo_CNTRL(false);//disable the trackbar

                    //set up the button and images 
               //     play_pause_BTN.BackgroundImage = VideoCapture.Properties.Resources.Record;
                    recordstate = false;

                    //Start aquring from the webcam
                    _Capture.Start();

                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
            }
        }



        /*ThreadSafe Operations*/
        /// <summary>
        /// Thread safe method to display Image in Pictureboxe
        /// </summary>
        /// <param name="Image"></param>
        private delegate void DisplayImageDelegate(Bitmap Image);
        private void DisplayImage(Mat Image)
        {
            if (Video_Image.InvokeRequired)
            {
                //try
                //{
                //    DisplayImageDelegate DI = new DisplayImageDelegate(DisplayImage);
                //    this.BeginInvoke(DI, new object[] { Image });
                //}
                //catch (Exception ex)
                //{
                //}
            }
            else
            {
                Video_Image.Image = Image;
            }
        }
        /// <summary>
        /// Thread safe method to update any of the Label controls on the form
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Control"></param>
        private delegate void UpdateTextBoxDelegate(String Text, Label Control);
        private void UpdateTextBox(String Text, Label Control)
        {
            if (Control.InvokeRequired)
            {
                try
                {
                    UpdateTextBoxDelegate UT = new UpdateTextBoxDelegate(UpdateTextBox);
                    this.BeginInvoke(UT, new object[] { Text, Control });
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                Control.Text = Text;
                this.Refresh();
            }
        }
        /// <summary>
        /// Thread safe method to update the value of the Video_CNTRL trackbar
        /// </summary>
        /// <param name="Value"></param>
        private delegate void UpdateVideo_CNTRLDelegate(double Value);
        private void UpdateVideo_CNTRL(double Value)
        {
            if (Video_CNTRL.InvokeRequired)
            {
                try
                {
                    UpdateVideo_CNTRLDelegate UVC = new UpdateVideo_CNTRLDelegate(UpdateVideo_CNTRL);
                    this.BeginInvoke(UVC, new object[] { Value });
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                //Do a quick in range check as sometime the codec may not tell the truth
                if (Value < Video_CNTRL.Maximum) Video_CNTRL.Value = (int)Value;
            }
        }
        /// <summary>
        /// Threadsafe method toe Enable/Disable the Video_CNTRL trackbar
        /// </summary>
        /// <param name="State"></param>
        private delegate void EnableVideo_CNTRLDelegate(bool State);
        private void UpdateVideo_CNTRL(bool State)
        {
            if (Video_CNTRL.InvokeRequired)
            {
                try
                {
                    EnableVideo_CNTRLDelegate UVC = new EnableVideo_CNTRLDelegate(UpdateVideo_CNTRL);
                    this.BeginInvoke(UVC, new object[] { State });
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                //Do a quick in range check as sometime the codec may not tell the truth
                Video_CNTRL.Enabled = State;
            }
        }

        /// <summary>
        /// Exit the program via menu option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Ensure that the Camera Setting are reset if the form is just clossed and the camera is released
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_Capture != null)
            {
               // if (_Capture.GrabProcessState == System.Threading.ThreadState.Running) _Capture.Stop();
                _Capture.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Video_Image =  new Emgu.CV.UI.ImageBox();
            this.Video_Image.Location = new System.Drawing.Point(100, 100);
            this.Video_Image.Size = new System.Drawing.Size(800, 800);
            this.Video_Image.BackColor = Color.Blue;
            this.Controls.Add(Video_Image);
        }

        private void buttonPlayStop_Click(object sender, EventArgs e)
        {

        }

        private void Video_CNTRL_Scroll(object sender, ScrollEventArgs e)
        {
            ////   _Capture.Stop();
            //   int frame = Video_CNTRL.Value;
            //   if (_Capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, frame))
            //   {
            //       _Capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, frame);
            //   }     

            ////   if (_Capture.Grab())
            //   {
            //       Mat img = _Capture.QueryFrame();
            //       Video_Image.Image = img;

            //    //   img.Save("test.jpg");
            //   }

            if (Images.Count  > 0)
            {
                Video_Image.Image = Images[Video_CNTRL.Value];
            }

            labelPosition.Text = Video_CNTRL.Value.ToString();

        }

        private void buttonStream_Click(object sender, EventArgs e)
        {
            //set up filter
            OF.Filter = "all | *";// "Video Files|*.avi;*.mp4;*.mpg";
            //open file dialog to select file
            if (OF.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                _Capture = new Emgu.CV.Capture(OF.FileName);
                _Capture.ImageGrabbed += _Capture_ImageGrabbed;
                _Capture.Start();
            }
        }

        List<Mat> Images = new List<Mat>();

        private void _Capture_ImageGrabbed(object sender, EventArgs e)
        {           
            Mat frame = new Mat();
            _Capture.Retrieve(frame, 0);

            Images.Add(frame);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Video_CNTRL.Maximum = Images.Count-1;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
          //  if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                VideoWriter VWtmp = new VideoWriter("test.mjpeg", VideoWriter.Fourcc('M', 'J', 'P', 'G'),
                    2, new Size(768, 576), true);
              
              //  {
                   

               //     int i = 0;
                    foreach (var im in Images)
                    {
                        VWtmp.Write(im); //If we are recording and videowriter is avaliable add the image to the videowriter 

                      //    im.Save(i++.ToString() +".jpg");
                    }
             //   }
             //   VW = new VideoWriter(saveFileDialog1.FileName, -1, (int)FrameRate, new Size(Frame_width, Frame_Height), true);
                //Save the image
                
              //  VW.Dispose();
            }
        }
    }
    
 
}
