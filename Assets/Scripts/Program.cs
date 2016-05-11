﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace ImageColor
{
	public static class MainClass
	{
	    public struct Color
	    {
	        public float r;
	        public float g;
	        public float b;
	        public float a;

	        public Color(float R, float G, float B, float A)
	        {
	            r = R;
	            g = G;
	            b = B;
	            a = A;
	        }

            public Color(float R, float G, float B)
            {
                r = R;
                g = G;
                b = B;
                a = 255f;
            }
            public Color(UnityEngine.Color c)
	        {
	            r = c.r * 255f;
	            g = c.g * 255f;
	            b = c.b * 255f;
	            a = c.a * 255f;
	        }

            public static implicit operator Vector4(Color c)
            {
                return new Vector4(c.r, c.g, c.b, c.a);
            }

            public static Color operator +(Color a, Color b)
            {
                return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
            }

            public static Color operator -(Color a, Color b)
            {
                return new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
            }

            public static bool operator ==(Color l, Color r)
            {
                return (Vector4)(l) == (Vector4)(r);
            }

            public static bool operator !=(Color lhs, Color rhs)
            {
                return !(lhs == rhs);
            }
            public static Color operator /(Color a, float b)
            {
                return new Color(a.r / b, a.g / b, a.b / b, a.a / b);
            }

            public static Color BLACK = new Color(0f,0f,0f,255f);
            public static Color WHITE = new Color(255f, 255f, 255f, 255f);
        }

	    public static UnityEngine.Color ChangeToUnityColor(this MainClass.Color color)
	    {
	        UnityEngine.Color c = new UnityEngine.Color();
	        c.g = color.g / 255f;
	        c.a = color.a / 255f;
	        c.b = color.b / 255f;
	        c.r = color.r / 255f;
	        return c;
	    }
		public static void Main (string[] args)
		{
			
		}

		//边缘色差结构体
		public struct Chrom{
			public double[,] xchrom;//x方向上的色差
			public double[,] ychrom;//y方向上的色差
			public double[,] chrom;//总色差
		} 

		//我TM第二次意淫编程了，连库都没有   连测试都不能做
		//获取图片连续轮廓灰度矩阵
		public static double[,] ReadOutline(Color[,] image)
		{
			int m = image.GetLength (0);
			int n = image.GetLength (1);

		    int cou = 0;
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (image[i, j].r > 125)
                    {
                        ++cou;
                    }
                }
            }

            //高斯滤波
            Color[,] w = Fspecial(image,m,n);
			//Sobel边缘检测
			Chrom chrom = Sobel (w, m, n);

			//非极大值抑制去除多余边界 寻找像素点局部最大值，将非极大值点所对应的灰度值置为0
			double[,] newEdge = new double[m,n];
			for (int i = 1; i < m - 1; ++i) 
			{
				for (int j = 1; j < n - 1; ++j) 
				{
					double Mx = chrom.xchrom [i, j];
					double My = chrom.ychrom [i, j];
					double o;
					if (Math.Abs (My) > Math.Exp (-6)) 
					{//分母不为0或过小
						//边缘的法线弧度
						o = Math.Atan (Mx / My);
					} else if (Mx > 0) 
					{
						o = Math.PI / 2;
					} else 
					{
						o = -1 * Math.PI / 2; 
					}

					//边缘像素法线一侧求得的两点坐标，插值需要  
					int[] adds = get_coords (o);
					double M1 = My * chrom.chrom [i + adds [1], j + adds [0]] + (Mx - My) * chrom.chrom [i + adds [3], j + adds [2]];
				
					adds = get_coords (o + Math.PI);
					double M2 = My * chrom.chrom [i + adds [1], j + adds [0]] + (Mx - My) * chrom.chrom [i + adds [3], j + adds [2]];
				

					//如果当前点比两边点都大
					bool isBigger = ((Mx*chrom.chrom[i,j]>M1)&&(Mx*chrom.chrom[i,j]>=M2))||((Mx*chrom.chrom[i,j]<M1)&&(Mx*chrom.chrom[i,j]<=M2));

					if (isBigger) 
					{
						newEdge [i, j] = chrom.chrom [i, j];
					}
				}
			}

			//滞后阈值处理
		    double up = 120;//上阈值
			double low = 100;//下阈值
			for(int i=0;i<m;++i)
			{
				for(int j=0;j<n;++j)
				{
					if(newEdge[i,j]>up && newEdge[i,j] != 255) //判断上阈值
					{
						newEdge [i, j] = 255;
						//八连通分析
						newEdge = connect(newEdge,i,j,low);
					}
				}
			}

            //这个之后 newEdge已经是连通的边界了  255就是边界


            //统计255个数
		    int count = 0;
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (newEdge[i, j] == 255)
                    {
                        ++count;
                    }
                }
            }

            return newEdge;

		}
		

		//高斯滤波
		public static Color[,] Fspecial(Color[,] image,int m,int n)
		{
			Color[,] res = new Color[m,n];
			//为了把9种情况统一 边上直接取自己

			for (int i = 0; i < m; ++i)
			{
				for (int j = 0; j < n; ++j)
				{
					if (i == m - 1 || i == 0 || j == 0 || j == n - 1) 
					{
						res [i, j] = image [i, j];
					} else 
					{
						res [i, j].r = (4 * image [i, j].r + 2 * (image [i - 1, j].r + image [i + 1, j].r + image [i, j + 1].r + image [i, j - 1].r)
						+ image [i - 1, j - 1].r + image [i - 1, j + 1].r + image [i + 1, j - 1].r + image [i + 1, j + 1].r) / 16;

						res [i, j].g = (4 * image [i, j].g + 2 * (image [i - 1, j].g + image [i + 1, j].g + image [i, j + 1].g + image [i, j - 1].g)
							+ image [i - 1, j - 1].g + image [i - 1, j + 1].g + image [i + 1, j - 1].g + image [i + 1, j + 1].g) / 16;

						res [i, j].b = (4 * image [i, j].b + 2 * (image [i - 1, j].b + image [i + 1, j].b + image [i, j + 1].b + image [i, j - 1].b)
							+ image [i - 1, j - 1].b + image [i - 1, j + 1].b + image [i + 1, j - 1].b + image [i + 1, j + 1].b) / 16;
					}
				}
			
			}
			return res;

		}

		//Sobel边缘检测
		public static Chrom Sobel(Color[,] image,int m,int n)
		{
			//获取灰度矩阵
			double[,] gray = RGBToGray (image, m, n);

			Chrom res = new Chrom ();
			//为每个像素计算色差
			double[,] xchrom = new double[m,n];//x方向的色差
			double[,] ychrom = new double[m,n];//y方向的色差
			double[,] chrom = new double[m,n];//综合色差
			for (int i = 0; i < m; ++i) 
			{
				for (int j = 0; j < n; ++j) 
				{
					//边缘色差为0
					if ((i == m - 1 || i == 0)&&((j == n-1 || j==0))){
						xchrom [i, j] = 0;
						ychrom [i, j] = 0;
					} else if(i == m - 1 || i == 0) {
						xchrom [i, j] = 0;
					} else if(j == n-1 || j==0){
						ychrom [i, j] = 0;
					} else{
						xchrom [i, j] = -1 * (gray [i - 1, j + 1] + gray [i - 1, j - 1]) + (gray [i + 1, j - 1] + gray [i + 1, j + 1]) - 2 * gray [i - 1, j] + 2 * gray [i + 1, j];
						ychrom [i, j] = -1 * (gray [i + 1, j - 1] + gray [i - 1, j - 1]) + gray [i + 1, j + 1] + gray [i - 1, j + 1] - 2 * gray [i, j - 1] + 2 * gray [i, j + 1];
					}
					chrom [i, j] = Math.Sqrt (xchrom [i, j] * xchrom [i, j] + ychrom [i, j] * ychrom [i, j]);
				}
			}
			res.xchrom = xchrom;
			res.ychrom = ychrom;
			res.chrom = chrom;
			return res;
		}

		//Gray = (R*299 + G*587 + B*114 + 500) / 1000
		//RGB转灰度
		public static double[,] RGBToGray(Color[,] image,int m,int n)
		{
			double[,] gray = new double[m,n];
			for (int i = 0; i < m; ++i) 
			{
				for (int j = 0; j < n; ++j) 
				{
					gray [i, j] = (image [i, j].r * 299 + image [i, j].g * 587 + image [i, j].b * 114 + 500) / 1000;
				}
			}


			return gray;
		}

		//返回法线两点坐标
		private static int[] get_coords(double angle)
		{
			double sigma = 0.000000001;
			int x1 = (int)Math.Ceiling (Math.Cos (angle + Math.PI / 8) * Math.Sqrt (2) - 0.5 - sigma);
			int y1 = (int)Math.Ceiling (-1 * Math.Sin (angle - Math.PI / 8) * Math.Sqrt (2) - 0.5 - sigma);
			int x2 = (int)Math.Ceiling (Math.Cos (angle - Math.PI / 8) * Math.Sqrt (2) - 0.5 - sigma);
			int y2 = (int)Math.Ceiling (-1 * Math.Sin (angle - Math.PI / 8) * Math.Sqrt (2) - 0.5 - sigma);
			int[] res = new int[4] {x1,y1,x2,y2};

			return res;
		}

		//连接轮廓
		private static double[,] connect(double[,] nedge,int y,int x,double low)
		{
			int[,] neighbour = new int[8, 2] {
				{-1,-1},
				{-1,0},
				{-1,1},
				{0,-1},
				{0,1},
				{1,-1},
				{1,0},
				{1,1}
			};
			int m = nedge.GetLength (0);
			int n = nedge.GetLength (1);
			for (int k = 0; k < 8; ++k) 
			{
				int yy = y + neighbour [k, 0];
				int xx = x + neighbour [k, 1];
				if (yy >= 0 && yy < m && xx >= 0 && xx < n) 
				{
					if (nedge[yy, xx] >= low && nedge [yy, xx] != 255) 
					{
						nedge[yy,xx]=255;
                        nedge = connect(nedge, yy, xx, low);
                    }
					
				}
			}
			 
			return nedge;
		}
	}
}
	