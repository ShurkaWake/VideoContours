using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ORO_Lb5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Image<Rgb, byte> img = new Image<Rgb, byte>(@"C:\Users\Sasha\Desktop\Pictures\photo_2022-11-09_13-39-47.jpg");
            var width = img.Width;
            var heigth = img.Height;

            img.Resize(200, 200, Emgu.CV.CvEnum.Inter.Cubic);
            var cp = new ContourParser(img, true);
            var res = cp.Result;
            res.Resize(width, heigth, Emgu.CV.CvEnum.Inter.Cubic);
            res.Save(@"C:\Users\Sasha\Desktop\Pictures\result1.jpg");
        }
    }
}
