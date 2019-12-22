using System;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Threading;

namespace Face_Detection
{
    public partial class frmMain : Form
    {
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 1, 1);
        private Capture cap;
        private HaarCascade haar; 

        private void frmMain_Load(object sender, EventArgs e)
        {
            //timer1.Start();
            // passing 0 gets zeroth webcam
            //cap = new Capture(0);
            // adjust path to find your xml
            //haar = new HaarCascade("haarcascade_frontalface_default.xml"); //haarcascade_frontalface_alt2.xml
        }

        public frmMain()
        {
            InitializeComponent();  
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //using (Image<Bgr, byte> nextFrame = cap.QueryFrame())
            //{
            //    if (nextFrame != null)
            //    {
            //        // there's only one channel (greyscale), hence the zero index
            //        //var faces = nextFrame.DetectHaarCascade(haar)[0];
            //        Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
            //        var faces =
            //                grayframe.DetectHaarCascade(
            //                        haar, 1.4, 4,
            //                        HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
            //                        new Size(nextFrame.Width / 8, nextFrame.Height / 8)
            //                        )[0];
            //        foreach (var face in faces)
            //        {
            //            nextFrame.Draw(face.rect, new Bgr(0, 255, 0),3); //black (0, double.MaxValue, 0)
            //            nextFrame.Draw("Person",ref font ,new Point(face.rect.X,face.rect.Y-10),new Bgr(0, double.MaxValue, 0));
            //        }
            //        pictureBox1.Image = nextFrame.ToBitmap();
            //    }
            //}
        }

        private void detectionRecognitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DetectionAndRecognition frmDR = new Face_Detection.DetectionAndRecognition(this);
            //frmDR.Show();
            Thread thr = new Thread(Start);
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();

            timer1.Stop();
            this.Close();
        }

        private void Start()
        {
            Application.Run(new DetectionAndRecognition());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void Apply()
        {
            MessageBox.Show("Hello World");
        }
    }
}
