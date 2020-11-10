using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace demo
{
    /// <summary>
    /// PicturesPage.xaml 的交互逻辑
    /// </summary>
    public partial class PicturesPage : Page
    {
        private RelationStruct[] texts = new RelationStruct[3];

        public PicturesPage()
        {
            InitializeComponent();
            SetData();
        }

        public void SetNumber(string name)
        {
            EE_Number_TextBlock.Text = name;
        }

        //获取编号
        public string GetNumber()
        {
            return EE_Number_TextBlock.Text;
        }

        //设置编号
        private void SetData()
        {
            texts[0].obj = new List<FrameworkElement>();
            texts[1].obj = new List<FrameworkElement>();
            texts[2].obj = new List<FrameworkElement>();
            //设置字符串对应的TextBox控件
            texts[0].name = "EE_Picture_Change_Button";
            texts[0].obj.Add(EE_Number_TextBlock);
            texts[0].obj.Add(EE_Piceture_Image);
            texts[1].name = "Overprint_Picture_Change_Button";
            texts[1].obj.Add(Overprint_Number_TextBlock);
            texts[1].obj.Add(Overprint_Picture_Image);
            texts[2].name = "Preview_Picture_Change_Button";
            texts[2].obj.Add(Preview_Number_TextBlock);
            texts[2].obj.Add(Preview_Picture_Image);
        }

        private void PictureChangeButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string path = FileTools.OpenFile("*.png|*.png|*.jpg|*.jpg|*.jepg|*.jepg", ".png");
            
            if (path.Length > 4)
            {
                string name = FileTools.GetFileShortName(path);
                foreach (RelationStruct keyValue in texts)
                {
                    if (keyValue.name == button.Name)
                    {
                        BitmapImage imagesouce = new BitmapImage();
                        imagesouce = new BitmapImage(new Uri(path));
                        ((Image)keyValue.obj[1]).Source = imagesouce.Clone();
                        ((TextBox)keyValue.obj[0]).Text = name.Split(new char[] { '.' })[0];
                        break;
                    }
                }
            }
        }

        public void HideControl(bool state)
        {
            if (state) //自动
            {
                EE_Picture_Change_Button.IsEnabled = false;
                Overprint_Picture_Change_Button.IsEnabled = false;
                Preview_Picture_Change_Button.IsEnabled = false;
                EE_Number_TextBlock.IsReadOnly = false;
            } else //手动
            {
                EE_Picture_Change_Button.IsEnabled = true;
                Overprint_Picture_Change_Button.IsEnabled = true;
                Preview_Picture_Change_Button.IsEnabled = true;
                EE_Number_TextBlock.IsReadOnly = false;
            }
        }

        public string LoadPicture()
        {
            string number = EE_Number_TextBlock.Text;
            if (number == "")
                return "请输入能效编号";
            BitmapImage[] imagesouce = new BitmapImage[3];
            if (File.Exists(FileTools.pictureDirPath + "\\" + number + ".png"))
            {
                EE_Piceture_Image.Source = new BitmapImage(new Uri(FileTools.pictureDirPath + "\\" + number + ".png")).Clone();
                EE_Number_TextBlock.Text = number;
            }
            if (File.Exists(FileTools.pictureDirPath + "\\" + number + "-AAA" + ".png"))
            {
                Overprint_Picture_Image.Source = new BitmapImage(new Uri(FileTools.pictureDirPath + "\\" + number + "-AAA" + ".png")).Clone();
                Overprint_Number_TextBlock.Text = number + "-AAA";
            }
            if (File.Exists(FileTools.pictureDirPath + "\\" + number + "-BBB" + ".png"))
            {
                Preview_Picture_Image.Source = new BitmapImage(new Uri(FileTools.pictureDirPath + "\\" + number + "-BBB" + ".png")).Clone();
                Preview_Number_TextBlock.Text = number + "-BBB";
            }

            return "";
        }
    }
}
