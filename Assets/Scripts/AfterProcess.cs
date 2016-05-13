using System;
using System.Collections.Generic;
using UnityEngine;

namespace ImageColor
{
    /// <summary>
    /// 对预处理后的识别出轮廓的图像进行处理
    /// </summary>
    public struct Coord
    {
        public int height;
        public int width;

        public Coord(int Height, int Width)
        {
            height = Height;
            width = Width;
        }

        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.height + b.height, a.width + b.width);
        }

        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.height - b.height, a.width - b.width);
        }

        /*
    public static Coord operator *(Coord a, Coord b)
    {
        return new Coord(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
    }
    */

        public static Coord operator /(Coord a, int b)
        {
            return new Coord(a.height / b, a.width / b);
        }
    }

    public static class AfterProcess
    {
        public static MainClass.Color 在轮廓中取色(MainClass.Color[,] image, MainClass.Color[,] 原图)
        {
            int firstHeight = 0;
            int firstWidth = 0;
            第一个边缘的像素(image, out firstHeight, out firstWidth);
            if (firstWidth == -1 || firstHeight == -1) Debug.LogError("第一个黑色的像素 出错");
            递归查找边缘像素(firstHeight, firstWidth, image);
            var 几何平均处 = 连续边缘的几何平均处();
            Debug.LogFormat("中间坐标{0}, {1}", 几何平均处.width, 几何平均处.height);
            return 范围内取平均色(原图, 几何平均处);
        }


        static void 第一个边缘的像素(MainClass.Color[,] image, out int height, out int width)
        {
            for (int i = 0; i < image.GetLength(0); ++i)
            {
                for (int j = 0; j < image.GetLength(1); ++j)
                {
                    if (image[i, j] == MainClass.Color.WHITE)
                    {
                        height = i;
                        width = j;
                        return;
                    }
                }
            }
            height = -1;
            width = -1;
        }



        static List<Coord> 连续边缘 = new List<Coord>();

        static int _scanRange = 2; //5*5的区域内

        static void 递归查找边缘像素(int centerHeight, int centerWidth, MainClass.Color[,] image)
        {
            if (centerHeight == -1 || centerWidth == -1) return;
            for (int h = centerHeight - 取色范围; h <= centerHeight + 取色范围; ++h)
            {
                for (int w = centerWidth - 取色范围; w <= centerWidth + 取色范围; ++w)
                {
                    //越界检测
                    if (h < 0 || h >= image.GetLength(0)) continue;
                    if (w < 0 || w >= image.GetLength(1)) continue;
                    if (image[h, w] == MainClass.Color.WHITE)
                    {
                        记录边缘并清除(h, w, image);
                        递归查找边缘像素(h, w, image);
                    }
                }
            }
        }

        static void 记录边缘并清除(int height, int width, MainClass.Color[,] image)
        {
            连续边缘.Add(new Coord(height, width));
            image[height, width] = MainClass.Color.BLACK;
        }

        static Coord 连续边缘的几何平均处()
        {
            Coord pos = new Coord();
            foreach (var c in 连续边缘)
            {
                pos += c;
            }
            pos /= 连续边缘.Count;
            连续边缘.Clear();
            return pos;
        }

        const int 取色范围 = 6;

        static MainClass.Color 范围内取平均色(MainClass.Color[,] image, Coord center)
        {
            MainClass.Color averageColor = new MainClass.Color();
            for (int h = center.height - 取色范围; h <= center.height + 取色范围; ++h)
            {
                for (int w = center.width - 取色范围; w <= center.width + 取色范围; ++w)
                {
                    //越界检测
                    if (h < 0 || h >= image.GetLength(0)) continue;
                    if (w < 0 || w >= image.GetLength(1)) continue;
                    averageColor += image[h, w];
                }              
            }
           // averageColor -= image[center.height, center.width];
            averageColor /= (2*取色范围+1) * (2*取色范围+1);
            return averageColor;
        }

        public static List<Coord> 从灰度数组中提取轮廓坐标(double[,] gray)
        {
            List<Coord> coords = new List<Coord>();
            for (int h = 0; h < gray.GetLength(0); ++h)
            {
                for (int w = 0; w < gray.GetLength(1); ++w)
                {
                    if (Math.Abs(gray[h, w] - 255) < double.Epsilon)
                    {
                        continue;
                    }
                    else
                    {
                        coords.Add(new Coord(h, w));
                    }
                }
            }
            return coords;
        }
    }
}


