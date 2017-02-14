using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge;
using AForge.Video;
using System.IO;

namespace Maalitaululaskuri
{
    public partial class Form1 : Form
    {
        bool laskettu=true;
        double pistetarkkuus = 0;
        double kuvatarkkuus = 0;
        double keskipiste = 0;
        double toinen = 0;
        double kolmas = 0;
        double tulosmax=0;
        double tulostoinen = 0;
        double tuloskolmas = 0;
        private FilterInfoCollection CaptureDevice; 
        private VideoCaptureDevice FinalFrame;
        Bitmap x;
        Bitmap y;
        Color color = Color.GreenYellow;
        Color color2 = Color.Red;
        Color color3 = Color.Purple;
        Color color4 = Color.Blue;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo Device in CaptureDevice)
                {
                    comboBox1.Items.Add(Device.Name);
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Ei kameroita kytketty");
            }
            
            label1.Text = "KESKIOSUMAT: ";
            label3.Text = "KAKKOSRIVIN OSUMAT: ";
            label4.Text = "KOLMOSRIVIN OSUMAT: ";
            try
            {
                comboBox1.SelectedIndex = 0;
                FinalFrame = new VideoCaptureDevice(CaptureDevice[comboBox1.SelectedIndex].MonikerString);
                FinalFrame.VideoResolution = FinalFrame.VideoCapabilities[0];
                FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
                FinalFrame.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Ei kameroita kytketty");

            }

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image!=null)
            {
                pictureBox2.Image.Dispose();
            }
            
            pictureBox2.Image = (Bitmap)pictureBox1.Image.Clone();
            if (x==null)
            {               
               x= (Bitmap)pictureBox1.Image.Clone();

            }
            else
            {
                y= (Bitmap)pictureBox1.Image.Clone();
            }
            laskettu = true;
            keskipiste = 0;
            toinen = 0;
            kolmas = 0;
             

        }

        void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs) 
                                                                             
        {                   
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
            

        }
        public Bitmap getDifferencBitmap(Bitmap bmp1, Bitmap bmp2, Color diffColor)
        {
            int leveys = bmp1.Width;
            int korkeus = bmp1.Height;
            leveys = leveys / 2;
            korkeus = korkeus / 2;
            

            double gap = korkeus * 0.20;
            
            bool aaa;
            bool rrr;
            bool ggg;
            bool bbb;
            Size s1 = bmp1.Size;
            Size s2 = bmp2.Size;
            if (s1 != s2) return null;

            Bitmap bmp3 = new Bitmap(s1.Width, s1.Height);

            for (int y = 0; y < s1.Height; y++)
                for (int x = 0; x < s1.Width; x++)
                {
                    aaa  = false;
                    rrr  = false;
                    ggg = false;
                    bbb = false;
                    Color c1 = bmp1.GetPixel(x, y);
                    Color c2 = bmp2.GetPixel(x, y);
                    int c1a = c1.A;
                    int c1R= c1.R;
                    int c1G = c1.G;
                    int c1B = c1.B;

                    int c2A = c2.A;
                    int c2R = c2.R;
                    int c2G = c2.G;
                    int c2B = c2.B;
                    int aa = c1a - c2A;
                    int rr = c1R - c2R;
                    int gg = c1G - c2G;
                    int bb = c1B - c2B;
                    if(aa >5 ||aa < -5)
                    {
                        aaa = true;
                    }
                   
                    if (rr > kuvatarkkuus|| rr < -kuvatarkkuus)
                    {
                        rrr = true;
                    }
                    if (gg > kuvatarkkuus || gg < -kuvatarkkuus)
                    {
                        ggg = true;
                    }
                    if (bb > kuvatarkkuus || bb < -kuvatarkkuus)
                    {
                        bbb = true;
                    }
                    if (aaa==true)
                    {
                        bmp3.SetPixel(x, y, diffColor);
                        pisteet(x, y,leveys,korkeus);
                    }
                    if (rrr==true)
                    {
                        bmp3.SetPixel(x, y, diffColor);
                        pisteet(x, y, leveys, korkeus);
                    }
                    else if(ggg==true)
                    {
                        bmp3.SetPixel(x, y, diffColor);
                        pisteet(x, y, leveys, korkeus); ;
                    }
                    else if (bbb==true)
                    {
                        bmp3.SetPixel(x, y, diffColor);
                        pisteet(x, y, leveys, korkeus);
                    }
                    else
                    {
                        bmp3.SetPixel(x, y, c1);
                        
                    }

                    if (x == leveys - gap&& y>korkeus-gap && y<korkeus+gap || x == leveys + gap&& y>korkeus-gap && y<korkeus+gap)
                    {
                        bmp3.SetPixel(x, y, color2);
                    }
                    if (y == korkeus - gap && x > leveys - gap && x < leveys + gap || y == korkeus + gap && x> leveys - gap && x < leveys + gap)
                    {
                        bmp3.SetPixel(x, y, color2);

                    }
                    if (x == leveys + gap * 2 && y > korkeus - gap*2 && y < korkeus + gap*2 || x == leveys - gap * 2 && y > korkeus - gap*2 && y < korkeus + gap*2)
                    {
                        bmp3.SetPixel(x, y, color3);
                    }

                    if (y == korkeus + gap * 2 && x > leveys - gap*2 && x < leveys + gap *2|| y == korkeus - gap * 2 && x > leveys - gap*2 && x < leveys + gap*2)
                    {
                        bmp3.SetPixel(x, y, color3);
                    }
                    if (x == leveys + gap * 4 && y > korkeus - gap * 4 && y < korkeus + gap * 4 || x == leveys - gap * 4 && y > korkeus - gap * 4 && y < korkeus + gap * 4)
                    {
                        bmp3.SetPixel(x, y, color4);
                    }
                    if (y == korkeus + gap * 4 && x > leveys - gap * 4 && x < leveys + gap * 4 || y == korkeus - gap * 4 && x > leveys - gap * 4 && x < leveys + gap * 4)
                    {
                        bmp3.SetPixel(x, y, color4);
                    }
                }
            if (keskipiste > pistetarkkuus)
            {
                tulosmax = tulosmax + 1;
                label1.Text = "KESKIOSUMAT: " + tulosmax.ToString();
            }
            if (toinen > pistetarkkuus)
            {
               tulostoinen = tulostoinen + 1;
                label3.Text = "KAKKOSRIVIN OSUMAT: " + tulostoinen.ToString();
            }
            if (kolmas > pistetarkkuus)
            {
                tuloskolmas = tuloskolmas + 1;
                label4.Text = "KOLMOSRIVIN OSUMAT: " + tulostoinen.ToString();
            }
            return bmp3;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (y!= null)
            {
                if (laskettu == true)
                {
                    kuvatarkkuus = int.Parse(textBox2.Text);
                    pistetarkkuus = int.Parse(textBox1.Text);
                    Bitmap diff = getDifferencBitmap(x, y, color);
                    pictureBox2.Image = diff;
                    laskettu = false;
                }
                else
                {
                    MessageBox.Show("Ota uusi kuva ennen seuraavaa vertausta!");
                }
            }
            else
            {
                MessageBox.Show("Ota ensin kuva tyhjästä taulusta ja laukauksen jälkeen toinen jonka jälkeen paina vertaa!");
            }
            
           
        }
        private void pisteet(int x, int y, int leveys, int korkeus)
        {
            double gap = korkeus*0.10;
            if (x>leveys-gap&&x<leveys+gap)
            {
                if (y>korkeus-gap && y<korkeus+gap)
                {
                    keskipiste++;
                }
            }
            if (x > leveys + gap && x < leveys + gap*2 || x > leveys - gap * 2  && x < leveys - gap)
            {               
                  if (y > korkeus + gap && y < korkeus + gap * 2 || y > korkeus - gap * 2  && y < korkeus - gap)
                  {
                    toinen++;
                  }               
            }
            if (x > leveys + gap*2 && x < leveys + gap * 4 || x > leveys - gap *4 && x < leveys - gap*2)
            {
                if (y > korkeus + gap*2 && y < korkeus + gap * 4 || y > korkeus - gap * 4 && y < korkeus - gap*2)
                {
                    kolmas++;
                }
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            x = null;
            y = null;
            tuloskolmas = 0;
            tulosmax = 0;
            tulostoinen = 0;
            label1.Text = "KESKIOSUMAT:";

            label3.Text = "KAKKOSRIVIN OSUMAT:";
            label4.Text = "KOLMOSRIVIN OSUMAT:";
            
        }
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FinalFrame.SignalToStop();
            FinalFrame.Stop();
            
            Application.Exit();
           
        }


    }
}
