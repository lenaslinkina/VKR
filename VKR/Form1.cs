using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using MaterialSkin.Animations;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.IO;

namespace VKR
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Cyan700, Primary.Cyan400, Primary.Cyan900, Accent.Cyan700, TextShade.WHITE);
        }
        Image<Bgr, byte> imgInput;
        Image<Gray, byte> imgOutput;
        Image<Gray, byte> imgOutput2;
        int minbl, mingr, minrd, maxbl, maxgr, maxrd;

        string name, namepus, nametr;
        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    imgInput = new Image<Bgr, byte>(openFileDialog1.FileName);
                    imageBox1.Image = imgInput;
                     name = Path.GetFileName(openFileDialog1.FileName);
                    namepus = "пустоты " + name;
                    nametr = "трещины " + name;

                    minbl = 255;
                    mingr = 255;
                    minrd = 255;
                    maxbl = 0;
                    maxgr = 0;
                    maxrd = 0;

                }
                else
                {
                    MessageBox.Show("Файл не выбран", "ошибка");

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ошибка");
            }
        }
        int bl, gr, rd;
        int pix;
        Image<Bgr, byte> temp;
        Image<Bgr, byte> temp2;
        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            Image<Gray, byte> blackBackground = new Image<Gray, byte>(imgInput.Width, imgInput.Height, new Gray(255));
            //// выделяет выбранные цвета
             imgOutput = imgInput.InRange(new Bgr(minbl, mingr, minrd), new Bgr(maxbl, maxgr, maxrd));

            // pictureBox2.Image = imgOutput.Bitmap;
            pictureBox1.Invalidate();

            temp = imgInput.Clone();
            temp.SetValue(new Bgr(0, 0, 255), imgOutput);

            pictureBox1.Image = temp.Bitmap;

            



            using (var bmp = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height))
            {
                pictureBox1.DrawToBitmap(bmp, pictureBox1.ClientRectangle);
                int iv, kv;
                pix = 0;
                for (iv = 1; iv < pictureBox1.Image.Width; iv++)
                {
                    for (kv = 1; kv < pictureBox1.Image.Height; kv++)
                    {
                        var color = bmp.GetPixel(iv, kv);
                        var red = color.R;
                        var green = color.G;
                        var blue = color.B;
                        if (red == 255 && green == 0 && blue == 0)
                        {
                            pix++;
                        }
                       
                        continue;
                    }
                }

                int S = pictureBox1.Width * pictureBox1.Height;
                double Per = (Convert.ToDouble(pix) / Convert.ToDouble(S)) * 100;
                textBox1.Text = (Convert.ToString(Per)) + "%";
            }
        }

        private void materialRaisedButton6_Click_1(object sender, EventArgs e)
        {
            imageBox1.Image = null;
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Загрузите фото. Инструкция по работе с программой кнопка - \"?\"");
        }

       

        private void materialRaisedButton7_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Выберите оттенки пустотного пространства на фотографии (синий цвет). Для этого приблизьте изображение, используя колесо мыши, одним нажатием правой кнопки мыши щелкните по подходящим оттенкам (чем больше количесто нажатых оттенков - тем лучше.)\n" +
                "2. Нажмите на кнопку \"поиск пустотного пространства\" для отображения найденных пустот. Если остались невыделенные участки, снова на входном изображение выберите оставшиеся оттенки, и заново нажмите кнопку \"поиск пустотного пространства\"\n" +
                "3.Для поиска трещин необходимо нажать кнопку \"поиск трещин\". Трещины рассчитаются в заданном масштабе. Для смены масштаба измените числовое значение и снова выполните поиск трещин\n"  +
               "4. Сохраните полученные изображения и параметры помощью кнопки рядом с ними. После нажатия кнопки \"Сохранить\" появится диалоговое окно, позволяющее выбрать папку и написать название файла, либо выбрать существующий файл, а также выбрать формат. Для сохранения текстовых параметров, можно выбрать созданный файл. Система спросит о замене существующего файла. Необходимо нажать \"ок\", чтобы данные записались в новую строку в конце файла. Кнопка \"очистить окно \" - очищает изображения и расчитанные параметры. ");
        }

        private void materialRaisedButton8_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                savedialog.FileName = nametr;

                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {


                      
                        pictureBox2.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                     
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        private void imageBox1_MouseClick_1(object sender, MouseEventArgs e)
        {
            using (var bmp = new Bitmap(imageBox1.Width, imageBox1.Height))
            {
                imageBox1.DrawToBitmap(bmp, imageBox1.ClientRectangle);
                var color = bmp.GetPixel(e.X, e.Y);
                var red = color.R;
                var green = color.G;
                var blue = color.B;


                bl = Convert.ToInt32(blue);
                gr = Convert.ToInt32(green);
                rd = Convert.ToInt32(red);
                // textBox1.Text = Convert.ToString(blue + " синий" + green + " зеленый " + red + " красный ");

                //записываем минимальную переменную

                if (bl < minbl)
                {
                    minbl = bl;
                }


                if (gr < mingr)
                {
                    mingr = gr;
                }

                if (rd < minrd)
                {
                    minrd = rd;
                }

                if (bl > maxbl)
                {
                    maxbl = bl;
                }
                if (gr > maxgr)
                {
                    maxgr = gr;
                }

                if (rd > maxrd)
                {
                    maxrd = rd;
                }
            }
        }

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile1 = new SaveFileDialog();
            saveFile1.DefaultExt = "*.txt";
            saveFile1.Filter = "Text files|*.txt";
            if (saveFile1.ShowDialog() == System.Windows.Forms.DialogResult.OK &&
                saveFile1.FileName.Length > 0)
            {
                using (StreamWriter sw = new StreamWriter(saveFile1.FileName, true))
                {
                    sw.WriteLine("\nОбразец: " + name + "; пористость " + textBox1.Text + "; Общая ширина трещин " + textBox2.Text + "; Общая длина трещин" + textBox3.Text);
                    sw.Close();
                }
            }
        }

        string r;
        private void materialRaisedButton4_Click(object sender, EventArgs e)
        {

            imgOutput2 = imgOutput.SmoothGaussian(9);
          
            imgOutput2 = imgOutput2.Canny(200, 255, 3, true);


            Image<Gray, byte> outputImage = imgOutput2.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(outputImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);

            

            Image<Gray, byte> blackBackground = new Image<Gray, byte>(outputImage.Width, outputImage.Height, new Gray(255));
            CvInvoke.DrawContours(blackBackground, contours, -1, new MCvScalar(0, 0, 255), 2, Emgu.CV.CvEnum.LineType.AntiAlias);
            pictureBox2.Image = blackBackground.Bitmap;

            double a = 0;
            int g = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                double area = CvInvoke.ContourArea(contours[i], true);
                if (area < 0)
                {
                    area = area * (-1);

                }
                a = a + area;
            }
            double areamin = a;
            double areamax = 0;
            r = Convert.ToString(a);
            for (int i = 0; i < contours.Size; i++)
            {
                double chislo = (CvInvoke.ContourArea(contours[i], true));
                if (chislo < 0)
                {
                    chislo = chislo * (-1);
                }
                if (chislo < areamin)
                {
                    areamin = chislo;
                }

            }

            for (int i = 0; i < contours.Size; i++)
            {
                double chislo = (CvInvoke.ContourArea(contours[i], true));
                if (chislo < 0)
                {
                    chislo = chislo * (-1);
                }
                if (chislo > areamax)
                {
                    areamax = chislo;
                    g = i;
                }

            }

            double b = 0;

            for (int i = 0; i < contours.Size; i++)
            {
                double perimetr = CvInvoke.ArcLength(contours[i], true);
                if (perimetr < 0)
                {
                    perimetr = perimetr * (-1);
                }
                b = b + perimetr;
            }
            double min = b;
            double max = -1;
            for (int i = 0; i < contours.Size; i++)
            {
                double chislo = CvInvoke.ArcLength(contours[i], true);
                if (chislo < 0)
                {
                    chislo = chislo * (-1);
                }
                if (chislo < min)
                {
                    min = chislo;
                }

            }


            // надо придумать как пробегать по всем большим конткурам и строить прямгоульники

            for (int i = 0; i < contours.Size; i++)
            {

                double chislo2 = CvInvoke.ArcLength(contours[i], true);
                if (chislo2 < 0)
                {
                    chislo2 = chislo2 * (-1);
                }
                if (chislo2 > max)
                {
                    max = chislo2;

                }

            }
            RotatedRect recct;
            double pol;
            pol = max / 6;
            int chet = 0;
            double hig = 0, wid = 0;
            double hight = 0, width = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                double chislo = (CvInvoke.ArcLength(contours[i], true));
                if (chislo < 0)
                {
                    chislo = chislo * (-1);
                }

                if (chislo > pol)
                {
                    chet = chet + 1;
                    recct = CvInvoke.MinAreaRect(contours[i]);

                    hig = Convert.ToDouble(recct.Size.Height);
                    wid = Convert.ToDouble(recct.Size.Width);

                    if (hig > wid)
                    {
                        width = width + hig;
                        hight = hight + wid;
                    }
                    else
                    {
                        width = width + wid;
                        hight = hight + hig;
                    }
                }

            }
            double hightt, widthh, delit;
            delit = chet * Convert.ToDouble(numericUpDown1.Value);
            hightt = hight / delit;
            widthh = width / Convert.ToDouble(numericUpDown1.Value);
           
            textBox2.Text = (Convert.ToString(hightt));
            textBox3.Text = (Convert.ToString(widthh));

        }

      

        private void materialRaisedButton6_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = imgOutput.Bitmap;
        }

        private void materialRaisedButton5_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить картинку как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.JPG)|*.JPG| Image Files(*.BMP)|*.BMP| Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                savedialog.FileName = namepus;

                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {


                        // imageBox1.BackgroundImage.Save(savedialog.FileName);
                        // blackBackground.Save(savedialog.FileName);
                         pictureBox1.Image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                       // pictureBox1.Image.Save(name, System.Drawing.Imaging.ImageFormat.Jpeg);
                       // pictureBox1.Image.Save(@""+ name);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }


        }
        double minrasch, maxrasch;
        
        private void materialRaisedButton9_Click(object sender, EventArgs e)
        {
            minrasch = 10000000;
            maxrasch = -1;
            imgOutput2 = imgOutput.SmoothGaussian(9);

            imgOutput2 = imgOutput2.Canny(200, 255, 3, true);


            Image<Gray, byte> outputImage = imgOutput2.Convert<Gray, byte>().ThresholdBinary(new Gray(100), new Gray(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(outputImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);

            

            Image<Gray, byte> blackBackground = new Image<Gray, byte>(outputImage.Width, outputImage.Height, new Gray(255));
      

             RotatedRect recct;
           
            int chet = 0;
            double hig = 0, wid = 0;
            double hight = 0, width = 0;
            for (int i=0; i < contours.Size; i++)
            {
                double perimetr = Math.Abs( CvInvoke.ArcLength(contours[i], true));
                double area = Math.Abs(CvInvoke.ContourArea(contours[i], true));
                double otnoshen = area / (perimetr * perimetr);
                if (otnoshen <minrasch)
                {
                    minrasch = otnoshen;
                }
                if (otnoshen > maxrasch)
                {
                    maxrasch = otnoshen;
                }

                // if (otnoshen < Convert.ToDouble(numericUpDown2))
                if (otnoshen < 0.0009)
                {
                    CvInvoke.DrawContours(blackBackground, contours, i, new MCvScalar(0, 0, 255), 2);
                    double chislo = (CvInvoke.ArcLength(contours[i], true));
                    if (chislo < 0)
                    {
                        chislo = chislo * (-1);
                    }

                    
                        chet = chet + 1;
                        recct = CvInvoke.MinAreaRect(contours[i]);
                        //    blackBackground.Draw(recct, new Gray(5), -1);

                        hig = Convert.ToDouble(recct.Size.Height);
                        wid = Convert.ToDouble(recct.Size.Width);

                        if (hig > wid)
                        {
                            width = width + hig;
                            hight = hight + wid;
                        }
                        else
                        {
                            width = width + wid;
                            hight = hight + hig;
                        }

                    
                }
            }
            pictureBox2.Image = blackBackground.Bitmap;
            //0.0733
            double hightt, widthh, delit;
            delit = chet * Convert.ToDouble(numericUpDown1.Value);
            hightt = hight / delit;
            widthh = width / Convert.ToDouble(numericUpDown1.Value);
            // hight = hight / (chet);

            //    RotatedRect plate_feature=  CvInvoke.MinAreaRect(contours[g]);
            //blackBackground.Draw(plate_feature, new Gray(5), -1);
            //imageBox1.Image = blackBackground;
            // b = CvInvoke.ArcLength(contours,true);
            // MessageBox.Show(" Средняя ширина " + Convert.ToString(hight) + " Общая длина= " + Convert.ToString(width));
            // MessageBox.Show(" Средняя ширина " + Convert.ToString(hight) + " Общая длина= " + Convert.ToString(width));

            textBox2.Text = (Convert.ToString(hightt));
            textBox3.Text = (Convert.ToString(widthh));
        }

        // int minbl, mingr, minrd, maxbl, maxgr, maxrd;
        private void imageBox1_MouseClick(object sender, MouseEventArgs e)
        {

            using (var bmp = new Bitmap(imageBox1.Width, imageBox1.Height))
            {
                imageBox1.DrawToBitmap(bmp, imageBox1.ClientRectangle);
                var color = bmp.GetPixel(e.X, e.Y);
                var red = color.R;
                var green = color.G;
                var blue = color.B;


                bl = Convert.ToInt32(blue);
                gr = Convert.ToInt32(green);
                rd = Convert.ToInt32(red);
               // textBox1.Text = Convert.ToString(blue + " синий" + green + " зеленый " + red + " красный ");

                //записываем минимальную переменную

                if (bl < minbl)
                {
                    minbl = bl;
                }


                if (gr < mingr)
                {
                    mingr = gr;
                }

                if (rd < minrd)
                {
                    minrd = rd;
                }

                if (bl > maxbl)
                {
                    maxbl = bl;
                }
                if (gr > maxgr)
                {
                    maxgr = gr;
                }

                if (rd > maxrd)
                {
                    maxrd = rd;
                }
                //  textBox1.Text = Convert.ToString(red + " красный " + green + " зеленый " + blue + " синий");
                // imageBox1.BackColor = (imageBox1 as Bitmap).GetPixel(e.X, e.Y);
                //textBox3.Text = Convert.ToString(minbl);
                //textBox4.Text = Convert.ToString(mingr);
                //textBox5.Text = Convert.ToString(minrd);

                //textBox6.Text = Convert.ToString(maxbl);
                //textBox7.Text = Convert.ToString(maxgr);
                //textBox8.Text = Convert.ToString(maxrd);
              //  textBox4.Text = (" min" + Convert.ToString(minbl) + " " + Convert.ToString(mingr) + " " + Convert.ToString(minrd) + " " + Convert.ToString(maxbl) + " " + Convert.ToString(maxgr) + " " + Convert.ToString(maxrd));

            }
        }
    }
}
