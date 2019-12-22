using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.IO;
using System.Threading;

namespace Face_Detection
{
    public partial class DetectionAndRecognition : Form
    {
        frmMain _Home;
        // Declar Variables
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.6d, 0.6d);
        HaarCascade faceDetect;
        Image<Bgr, Byte> frame;
        Capture camera;
        Image<Gray, byte> result;
        Image<Gray, byte> TrainedFace = null;
        Image<Gray, byte> grayFace = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> Labels = new List<string>();
        List<string> Users = new List<string>();
        int Count, NumLabels;
        string name;

        public DetectionAndRecognition()
        {
            InitializeComponent();
            // HaarCascade
            faceDetect = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                string labelsinf = File.ReadAllText(Application.StartupPath + "/Faces/Faces.text");
                string[] labels = labelsinf.Split(',');
                // The first label befor, will be the number of faces saved.
                NumLabels = Convert.ToInt16(labels[0]);
                Count = NumLabels;
                string FacesLoad;
                for (int i = 1; i < NumLabels + 1; i++)
                {
                    FacesLoad = "face" + i + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(Application.StartupPath + "/Faces/Faces.txt"));
                    Labels.Add(labels[i]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Nothing in Database !");
            }
        }

        public DetectionAndRecognition(frmMain home)
        {
            InitializeComponent();
            _Home = home;
        }

            private void btnStartDetect_Click(object sender, EventArgs e)
        {
            camera = new Capture();
            camera.QueryFrame();
            Application.Idle += new EventHandler(FramePocedure);
        }

        private void FramePocedure(object sender, EventArgs e)
        {
            Users.Add("");
            frame = camera.QueryFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);
            grayFace = frame.Convert<Gray, byte>();
            MCvAvgComp[][] faceDetectedNow = grayFace.DetectHaarCascade(faceDetect, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
            foreach (MCvAvgComp f in faceDetectedNow[0])
            {
                result = frame.Copy(f.rect).Convert<Gray, Byte>().Resize(100, 100, INTER.CV_INTER_CUBIC);
                frame.Draw(f.rect, new Bgr(Color.Green), 3);
                if (trainingImages.ToArray().Length != 0)
                {
                    MCvTermCriteria termCriterias = new MCvTermCriteria(Count, 0.001);
                    EigenObjectRecognizer recognizer = new EigenObjectRecognizer(trainingImages.ToArray(), Labels.ToArray(), 1500, ref termCriterias);
                    name = recognizer.Recognize(result);
                    frame.Draw(name, ref font, new Point(f.rect.X - 2, f.rect.Y - 2), new Bgr(Color.Red));
                }
                //Users[t - 1] = name;
                Users.Add("");
            }
            cameraBox.Image = frame;
            name = "";
            Users.Clear();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_Home != null)
            {
                _Home.Apply();
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            Thread thr = new Thread(ReturnMain);
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
            this.Close();
        }

        static void ReturnMain()
        {
            Application.Run(new frmMain());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Count = Count + 1;
            grayFace = camera.QueryGrayFrame().Resize(320, 240, INTER.CV_INTER_CUBIC);
            MCvAvgComp[][] DetectedFaces = grayFace.DetectHaarCascade(faceDetect, 1.2, 10, HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));
            foreach (MCvAvgComp f in DetectedFaces[0])
            {
                TrainedFace = frame.Copy(f.rect).Convert<Gray, byte>();
                break;
            }
            TrainedFace = result.Resize(100, 100, INTER.CV_INTER_CUBIC);
            trainingImages.Add(TrainedFace);
            Labels.Add(txtBoxName.Text);
            File.WriteAllText(Application.StartupPath + "/Faces/Faces.txt", trainingImages.ToArray().Length.ToString() + ",");
            for (int i = 1; i < trainingImages.ToArray().Length; i++)
            {
                trainingImages.ToArray()[i - 1].Save(Application.StartupPath + "/Faces/face" + i + ".bmp");
                File.AppendAllText(Application.StartupPath + "/Faces/Faces.txt", Labels.ToArray()[i - 1] + ",");
            }

            MessageBox.Show(txtBoxName.Text + " Added Succefully");
        }
    }
}
