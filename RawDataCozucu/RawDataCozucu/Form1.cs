using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawDataCozucu
{
       public struct frame
    {
        public byte[] yDegerleri;
        public byte[] uDegerleri;
        public byte[] vDegerleri;
    };
    
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbFormat.Items.Add("4:4:4");
            cmbFormat.Items.Add("4:2:2");
            cmbFormat.Items.Add("4:2:0");
        }
        
        private byte[] Okuma(FileStream file)
        {
            int a = 0;
            int sayac = 0;

            byte[] icerik = new byte[file.Length];
            
            while (a>-1)
            {
                a = file.ReadByte();
                if(a != -1 )
                {
                    icerik[sayac] = (byte)a;
                }
                sayac++;
            }
            file.Close();
            return icerik;
        }

        private frame[] YuvBulma(byte[] dizi,int Boyut, string format)
        {
            int index = 0, counter = 1, yindex = 0,uindex = 0, vindex=0;
            frame[] frameler = new frame[10000];
            //frameler = new frame[dizi.Length / (int)(Boyut * 1.5)]; //4:2:0 için 

            if (format == "4:2:0")
            {
                frameler = new frame[dizi.Length / (int)(Boyut * 1.5)]; //4:2:0 için 

                for (int j = 0; j < frameler.Length; j++)
                {
                    frameler[j].yDegerleri = new byte[Boyut];
                    frameler[j].vDegerleri = new byte[(int)(Boyut / 4)];
                    frameler[j].uDegerleri = new byte[(int)(Boyut / 4)];
                }

               
                for (int i = 0; i < dizi.Length; i++)
                {
                    if (counter == 1)
                    {
                        if (yindex != Boyut)
                        {
                            frameler[index].yDegerleri[yindex] = dizi[i];
                            yindex++;
                        }
                        else
                            counter = 2;
                    }
                    if (counter == 2)
                    {
                        if (uindex != (Boyut / 4))
                        {
                            frameler[index].uDegerleri[uindex] = dizi[i];
                            uindex++;
                        }
                        else
                            counter = 3;
                    }
                    if (counter == 3)
                    {
                        if (vindex != (Boyut / 4))
                        {
                            frameler[index].vDegerleri[vindex] = dizi[i];
                            vindex++;
                        }
                        else
                        {
                            counter = 1;
                            uindex = 0;
                            vindex = 0;
                            yindex = 0;
                            index++;
                            i--;
                        }
                    }
                }
            }
            else if(format == "4:2:2")
            {
                frameler = new frame[dizi.Length / (int)(Boyut * 2)]; //4:2:2 için 

                for (int j = 0; j < frameler.Length; j++)
                {
                    frameler[j].yDegerleri = new byte[Boyut];
                    frameler[j].vDegerleri = new byte[(int)(Boyut / 2)];
                    frameler[j].uDegerleri = new byte[(int)(Boyut / 2)];
                }

                for (int i = 0; i < dizi.Length; i++)
                {
                    if (counter == 1)
                    {
                        if (yindex != Boyut)
                        {
                            frameler[index].yDegerleri[yindex] = dizi[i];
                            yindex++;
                        }
                        else
                            counter = 2;
                    }
                    if (counter == 2)
                    {
                        if (uindex != (Boyut / 2))
                        {
                            frameler[index].uDegerleri[uindex] = dizi[i];
                            uindex++;
                        }
                        else
                            counter = 3;
                    }
                    if (counter == 3)
                    {
                        if (vindex != (Boyut / 2))
                        {
                            frameler[index].vDegerleri[vindex] = dizi[i];
                            vindex++;
                        }
                        else
                        {
                            counter = 1;
                            uindex = 0;
                            vindex = 0;
                            yindex = 0;
                            index++;
                            i--;
                        }
                    }
                }

            }
            else if (format == "4:4:4")
            {
                frameler = new frame[dizi.Length / (int)(Boyut * 4)]; //4:4:4 için 

                for (int j = 0; j < frameler.Length; j++)
                {
                    frameler[j].yDegerleri = new byte[Boyut];
                    frameler[j].vDegerleri = new byte[(int)(Boyut)];
                    frameler[j].uDegerleri = new byte[(int)(Boyut)];
                }

                for (int i = 0; i < dizi.Length; i++)
                {
                    if(index != frameler.Length)
                    {
                        if (yindex != Boyut)
                        {
                            frameler[index].yDegerleri[yindex] = dizi[i];
                            frameler[index].uDegerleri[yindex] = dizi[i + Boyut];
                            frameler[index].vDegerleri[yindex] = dizi[i + Boyut + Boyut];
                            yindex++;
                        }
                        else
                        {
                            yindex = 0;
                            index++;
                        }
                    }
                    
                    
                    
                }
            }

            return frameler;
        }

        private void YuvtoBitmap(frame[] frames,int en, int boy )
        {
            Bitmap bm = new Bitmap(en, boy);
            int count = frames.Length;
            for(int y=0; y<count; y++)
            {
                for (int i = 0; i < en; i++)
                {
                    for (int j = 0; j < boy; j++)
                    {
                        bm.SetPixel(i, j, Color.FromArgb(frames[y].yDegerleri[j * en + i], frames[y].yDegerleri[j * en + i], frames[y].yDegerleri[j * en + i]));
                    }
                }
                bm.Save("frame" + (y + 1) + ".bmp");
                //pictureBox1.Image = Image.FromFile("C:\\Users\\Yagmur\\Desktop\\RawDataCozucu\\RawDataCozucu\\bin\\Debug\\frame" + (y + 1) + ".bmp");
                pictureBox1.Image = bm;
                this.Update();
            }
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            FileStream file;
            byte[] Dizi;
            frame[] frame;
            string format ="";
            int en = Convert.ToInt32(txtEn.Text.ToString());
            int boy = Convert.ToInt32(txtBoy.Text.ToString());
            int Boyut = en * boy;

            if (cmbFormat.SelectedItem.ToString() == "4:2:0")
            {
                format = "4:2:0";
                file = new FileStream("C:\\Users\\zynps\\OneDrive\\Masaüstü\\RawDataCozucu\\RawDataCozucu\\akiyo_qcif.yuv", FileMode.Open);
                Dizi = Okuma(file);
                frame = YuvBulma(Dizi, Boyut,format);
                YuvtoBitmap(frame, en, boy);
                
            }
            else if(cmbFormat.SelectedItem.ToString() == "4:2:2")
            {
                format = "4:2:2";
                file = new FileStream("C:\\Users\\zynps\\OneDrive\\Masaüstü\\RawDataCozucu\\RawDataCozucu\\422P.yuv", FileMode.Open);
                Dizi = Okuma(file);
                frame = YuvBulma(Dizi, Boyut,format);
               
            }
            else if(cmbFormat.SelectedItem.ToString() == "4:4:4")
            {
                format = "4:4:4";
                file = new FileStream("C:\\Users\\zynps\\OneDrive\\Masaüstü\\RawDataCozucu\\RawDataCozucu\\444P.yuv", FileMode.Open);
                Dizi = Okuma(file);
                frame = YuvBulma(Dizi, Boyut,format);
                YuvtoBitmap(frame, en, boy);
            }

        }
        
        
    }
}
