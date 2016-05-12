using System;
using System.Collections.Generic;

namespace ImageColor
{
	public class EmptyClass
	{
		public static List<List<Coord>> 获取轮廓坐标(double[,] 原轮廓,int m,int n)
		{
			double[,] 轮廓 = new double[m,n];
			for (int i = 0; i < m; ++i) 
			{
				for (int j = 0; j < n; ++j) 
				{
					轮廓 [i, j] = 原轮廓 [i, j];
				}
			}
			List<List<Coord>> resList = new List<List<Coord>> ();
			for (int i = 0; i < m; ++i) 
			{
				for (int j = 0; j < n; ++j) 
				{
					if (轮廓 [i, j] == 255) 
					{//是轮廓了
						//一个方向上找到最尾端
						List<Coord> 连续的点 = new List<Coord>();
						连续的点.Add (new Coord(i,j));
						轮廓 [i, j] = 0;
						Coord 下一个连续的点 = 计算下一个连续的点 (轮廓,m,n,i,j);
						while (下一个连续的点.width >= 0) 
						{
							连续的点.Add (下一个连续的点);
							轮廓 [下一个连续的点.height, 下一个连续的点.width] = 0;
							下一个连续的点 = 计算下一个连续的点 (轮廓,m,n,下一个连续的点.height, 下一个连续的点.width);
						}
						//木有新的连续点了 然后倒序 然后取最后一个坐标继续  到那边也木有
						连续的点.Reverse();
						Coord 第一个点 = 连续的点 [连续的点.Count - 1];
						下一个连续的点 = 计算下一个连续的点 (轮廓,m,n,第一个点.height,第一个点.width);
						while (下一个连续的点.width >= 0) 
						{
							连续的点.Add (下一个连续的点);
							轮廓 [下一个连续的点.height,下一个连续的点.width] = 0;
							下一个连续的点 = 计算下一个连续的点 (轮廓,m,n,下一个连续的点.height,下一个连续的点.width);
						}
						resList.Add (连续的点);
					}
				}
			}
			return resList;

		}

		public static Coord 计算下一个连续的点(double[,] 轮廓,int m,int n,int i,int j)
		{
			//先上下左右  然后斜着
			int [,] temp = {
				{-1,0},{-1,1},{0,1},{1,1},{1,0},{1,-1},{0,-1},{-1,-1},{-2,0},{-2,1},{-2,2},{-1,2},
				{0,2},{1,2},{2,2},{2,1},{2,0},{2,-1},{2,-2},{1,-2},{0,-2},{-1,-2},{-2,-2},{-2,-1},
				{-3,0},{-3,1},{-3,2},{-3,3},{-2,3},{-1,3},{0,3},{1,3},{2,3},{3,3},{3,2},{3,1},
				{3,0},{3,-1},{3,-2},{3,-3},{2,-3},{1,-3},{0,-3},{-1,-3},{-2,-3},{-3,-3},{-3,-2},{-3,-1}
			};
			for (int k = 0; k < temp.GetLength (0); ++k) {
				if (i + temp [k, 0] < m && i + temp [k, 0] >= 0 && j + temp [k, 1] >= 0 && j + temp [k, 1] < n
					&& 轮廓 [i + temp [k, 0], j + temp [k, 1]] == 255) 
				{
					return new Coord (i + temp [k, 0],j + temp [k, 1]);
				}
			}

			return new Coord(-1,-1);
		}
	}
}

