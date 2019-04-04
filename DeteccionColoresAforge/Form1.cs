using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Drawing.Imaging;

namespace DeteccionColoresAforge
{
    public partial class Form1 : Form
    {

        private FilterInfoCollection dispositivosDeVideo;
        private VideoCaptureDevice fuentev = null;
        private bool AforgeStatus = false;

        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (AforgeStatus)
            {
                fuentev.Stop();
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            dispositivosDeVideo = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo dispositivo in dispositivosDeVideo)
            {
                comboBox1.Items.Add(dispositivo.Name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Iniciar"))
            {
                fuentev = new VideoCaptureDevice(dispositivosDeVideo[comboBox1.SelectedIndex].MonikerString);
                fuentev.NewFrame += new NewFrameEventHandler(fuentevNewFrame);
                fuentev.Start();
                AforgeStatus = true;
                comboBox1.Enabled = false;
                button1.Text = "Detener";
            }
            else
            {
                button1.Text = "Iniciar";
                pictureBox1.Image = null;
                pictureBox2.Image = null;
                lbColor.Text = "Color";
                lbColor.BackColor = Color.White;
                lbColor.ForeColor = Color.Black;
                AforgeStatus = false;
                comboBox1.Enabled = true;
                fuentev.Stop();
            }
            

        }

        string colorSelect = "amarillo";
        int cont = 0;

        void fuentevNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap video = (Bitmap)eventArgs.Frame.Clone();
            Bitmap temp = video.Clone() as Bitmap;
            
            ColorFiltering filter = new ColorFiltering();
            // set color ranges to keep

            switch (colorSelect)
            {
                case "verde":
                    filter.Red = new IntRange(0, 75);
                    filter.Green = new IntRange(115, 255);
                    filter.Blue = new IntRange(0, 120);
                    break;
                case "anaranjado":
                    filter.Red = new IntRange(150, 255);
                    filter.Green = new IntRange(75, 128);
                    filter.Blue = new IntRange(0, 100);
                    break;
                case "azul":
                    filter.Red = new IntRange(15, 80);
                    filter.Green = new IntRange(50, 110);
                    filter.Blue = new IntRange(129, 255);
                    break;
                case "rojo":
                    filter.Red = new IntRange(115, 255);
                    filter.Green = new IntRange(0, 80);
                    filter.Blue = new IntRange(0, 75);
                    break;
                case "amarillo":
                    filter.Red = new IntRange(140, 255);
                    filter.Green = new IntRange(140, 255);
                    filter.Blue = new IntRange(0, 100);
                    break;
                case "rosa":
                    filter.Red = new IntRange(180, 255);
                    filter.Green = new IntRange(57, 135);
                    filter.Blue = new IntRange(112, 196);
                    break;
                    //default:
                    //    colorSelect = "verde";
                    //    break;

            }
            //filter.Radius = 90;
            filter.FillOutsideRange = true;
            filter.ApplyInPlace(temp);

            //BlobCounter es una clase que se usa para distingir objetos, colores, etc;
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            blobCounter.ProcessImage(temp);

            Rectangle[] rects = blobCounter.GetObjectsRectangles();

            //foreach (Rectangle rec in rects)
            if (rects.Length > 0)
            {
                lbColor.Text = colorSelect;
                lbColor.ForeColor = Color.White;
                switch (colorSelect)
                {
                    case "verde":
                        pictureBox2.Image = Properties.Resources.verde;
                        break;
                    case "anaranjado":
                        pictureBox2.Image = Properties.Resources.anaranjado;
                        break;
                    case "azul":
                        pictureBox2.Image = Properties.Resources.azul;
                        break;
                    case "rojo":
                        pictureBox2.Image = Properties.Resources.rojo;
                        break;
                    case "amarillo":
                        pictureBox2.Image = Properties.Resources.amarillo;
                        break;
                    case "rosa":
                        pictureBox2.Image = Properties.Resources.rosa;
                        break;
                }
                for (int i = 0; rects.Length > i; i++)
                {
                    

                    Rectangle objectRect = rects[0];
                    Graphics g = Graphics.FromImage(video);
                    using (Pen pen = new Pen(Color.Red, 3))
                    {

                        if (checkBox1.Checked)
                        {
                            g.DrawRectangle(pen, objectRect);
                            //g.DrawLine(pen, objectRect.Width- objectRect.Width, objectRect.Width- objectRect.Width, objectRect.Width, objectRect.Width);


                            Bitmap ColorDetect = video;
                            //Color x = ColorDetect.GetPixel(objectRect.Width/2, objectRect.Height/2);
                            Color x = ColorDetect.GetPixel(objectRect.X + (objectRect.Width / 3), objectRect.Top + (objectRect.Height / 3));
                            lbColor.BackColor = x;


                            //PointF drawPoin = new PointF(objectRect.Width / 2, objectRect.Width / 2);
                            //PointF drawPoin = new PointF(objectRect.X, objectRect.Y);
                            PointF drawPoin = new PointF(objectRect.X + (objectRect.Width / 3), objectRect.Top + (objectRect.Height / 3));
                            int objectX = objectRect.X + objectRect.Width / 2 - video.Width / 2;
                            int objectY = video.Height / 2 - (objectRect.Y + objectRect.Height / 2);
                            String Blobinformation = "X= " + objectX.ToString() + "\nY= " + objectY.ToString() + "\nSize=" + objectRect.Size.ToString();
                            //g.DrawString(Blobinformation, new Font("Arial", 12), new SolidBrush(Color.Blue), drawPoin);
                            g.DrawString("+", new Font("Arial", 12), new SolidBrush(Color.White), drawPoin);
                            //g.DrawString((i+1).ToString(), new Font("Arial", 12), new SolidBrush(Color.White), objectRect);

                        }
                    }

                    g.Dispose();
                }
            }
            else
            {
                lbColor.Text = "Detectando...";
                lbColor.BackColor = Color.White;
                lbColor.ForeColor = Color.Black;
                pictureBox2.Image = null;

                switch (colorSelect)
                {
                    case "verde":
                        colorSelect = "anaranjado";
                        break;
                    case "anaranjado":
                        colorSelect = "azul";
                        break;
                    case "azul":
                        colorSelect = "rojo";
                        break;
                    case "rojo":
                        colorSelect = "amarillo";
                        break;
                    case "amarillo":
                        colorSelect = "rosa";
                        break;
                    case "rosa":
                        colorSelect = "verde";
                        break;
                }

                cont++;
                Console.WriteLine("ELSE " + cont + " - " + colorSelect);
            }
            
            pictureBox1.Image = video;
            //pictureBox2.Image = temp;
        }

        public Rectangle objectRect { get; set; }
    }
}
