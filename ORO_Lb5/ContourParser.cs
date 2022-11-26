using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Img = Emgu.CV.Image<Emgu.CV.Structure.Rgb, byte>;
using Emgu.CV.ML;

namespace ORO_Lb5;
public class ContourParser
{
    int[,] mask;
    bool isReverse;
    Img source;
    Image<Gray, byte> result;

    public Image<Gray, byte> Result { get => result;}

    public ContourParser(Img source, bool isReverse, int[,] mask = null)
    {
        this.isReverse = isReverse;
        this.mask = mask;
        this.source = source.Clone() as Img;
        source = source.SmoothGaussian(11);

        result = new Image<Gray, byte>(this.source.Width, this.source.Height);
        Parse();
    }

    private double Average(int x, int y)
    {
        var pixel = source[x, y];
        return Math.Sqrt((Math.Pow(pixel.Red, 2) + Math.Pow(pixel.Green, 2) + Math.Pow(pixel.Blue, 2)) / 3);
    }

    private void Parse()
    {
        if (mask is null)
        {
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    Sobel(i, j);
                }
            }
        }
        else
        {
            for (int i = 0; i < source.Width; i++)
            {
                for (int j = 0; j < source.Height; j++)
                {
                    UseMask(i, j);
                }
            }
        }
        
    }

    private byte DoubleToByteBounds(double number)
    {
        byte result = 0;
        if(number < 0)
        {
            result = 0;
        }
        else if(number > 255)
        {
            result = 255;
        }
        else
        {
            result = (byte) number;
        }

        return result;
    }

    private void Sobel(int x, int y)
    {
        int[,] vertMask = new int[,]
        {
                {1, 0, -1},
                {2, 0, -2},
                {1, 0, -1}
        };

        int[,] horMask = new int[,]
        {
                {1, 2, 1},
                {0, 0, 0},
                {-1, -2, -1}
        };

        double Gx = 0;
        double Gy = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (x - 1 + i >= 0 && x - 1 + i < source.Width &&
                    y - 1 + j >= 0 && y - 1 + j < source.Height)
                {
                    var avg = Average(x - 1 + i, y - 1 + j); ;
                    Gx += horMask[i, j] * avg;
                    Gy += vertMask[i, j] * avg;
                }
            }
        }

        double res = Math.Sqrt(Math.Pow(Gx, 2) + Math.Pow(Gy, 2));

        if (isReverse)
        {
            res = Math.Abs(DoubleToByteBounds(res) - 255);
        }
        else
        {
            res = DoubleToByteBounds(res);
        }

        result[x, y] = new Gray(res);
    }

    private void UseMask(int x, int y)
    {
        double sum = 0;
        for (int i = 0; i < mask.GetLength(0); i++)
        {
            for (int j = 0; j < mask.GetLength(1); j++)
            {
                if (x - 1 + i >= 0 && x - 1 + i < source.Width &&
                    y - 1 + j >= 0 && y - 1 + j < source.Height)
                {
                    sum += mask[i, j] * Average(x - 1 + i, y - 1 + j);
                }
            }
        }

        if (isReverse)
        {
            sum = Math.Abs(DoubleToByteBounds(sum) - 255);
        }
        else
        {
            sum = DoubleToByteBounds(sum);
        }

        result[x, y] = new Gray(sum);
    }
}
