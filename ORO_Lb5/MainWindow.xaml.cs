using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
        readonly static string[] sources = new string[]
        {
            @"C:\Users\Sasha\Desktop\ОРО_Лр_5_видео\Беркут_охота_1.avi",
            @"C:\Users\Sasha\Desktop\ОРО_Лр_5_видео\ДТП Харьков_11.avi",
            @"C:\Users\Sasha\Desktop\ОРО_Лр_5_видео\Идущие_люди_1.avi"
        };
        readonly string source = sources[0];
        bool isInverted = true;

        int[,] gMatrix = new int[,]
        {
            {-1, -1, -1},
            {-1, 8, -1},
            {-1, -1, -1},
        };
        int[,] mMatrix = new int[,]
        {
            {-1, -1, -1},
            {-1, 9, -1},
            {-1, -1, -1},
        };

        bool isProcessing = false;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        async void Button_Click(object sender, RoutedEventArgs e)
        {
            var cap = new VideoCapture(source);
            StartButton.IsEnabled = false;
            int width = cap.Width;
            int heigth = cap.Height;

            Task t = Task.Factory.StartNew(() =>
            {
                while (cap.IsOpened)
                {
                    bool isOpen;
                    Mat temp = new Mat();
                    isOpen = cap.Read(temp);
                    if (!isOpen)
                    {
                        return;
                    }
                    Image<Rgb, byte> frame = temp.ToImage<Rgb, byte>();
                    Dispatcher.BeginInvoke(() => Video.Source = frame.ToBitmapSource());
                    if (!isProcessing)
                    {
                        Task task = new Task(() => Process(frame, width, heigth));
                        task.Start();
                    }
                    Thread.Sleep(30);
                }
            });
            await t;
            StartButton.IsEnabled = true;
        }

        private async void Process(Image<Rgb, byte> frameIn, int width, int heigth)
        {
            isProcessing = true;
            var frame = frameIn.Resize(250, 150, Emgu.CV.CvEnum.Inter.Cubic);
            Task t = new Task(() =>
            {
                var cp = new ContourParser(frame, isInverted, gMatrix);
                var img1 = cp.Result;
                img1 = img1.Resize(width, heigth, Emgu.CV.CvEnum.Inter.Cubic);

                cp = new ContourParser(frame, isInverted, mMatrix);
                var img2 = cp.Result;
                img2 = img2.Resize(width, heigth, Emgu.CV.CvEnum.Inter.Cubic);

                cp = new ContourParser(frame, isInverted);
                var img3 = cp.Result;
                img3 = img3.Resize(width, heigth, Emgu.CV.CvEnum.Inter.Cubic);

                Dispatcher.BeginInvoke(() =>
                {
                    CurrentFrame.Source = frameIn.ToBitmapSource();
                    ContourG.Source = img1.ToBitmapSource();
                    ContourM.Source = img2.ToBitmapSource();
                    ContourS.Source = img3.ToBitmapSource();
                });
            });
            t.Start();
            await t;
            isProcessing = false;
        }
    }
}
