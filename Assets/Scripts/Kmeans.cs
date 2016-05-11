using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kmeans
{
	public static class Kmeans
	{
		public struct Color
		{
			public float r;
			public float g;
			public float b;

		}

		static int k = 4;
		static float 迭代总次数 = 100;

		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
		}

		//聚类算法
		public static Color Kmeans(Color[,] 处理好后的图,int m,int n)
		{
			List<Color> 聚类中心 = new List<Color>();
			//随机选取k个颜色 放进List里面
			for (int i = 0; i < k; ++i) 
			{
				int 行 = Unity.Random.Range(0,m-1);
				int 列 = Unity.Random.Range(0,n-1);

				聚类中心.Add (处理好后的图[行,列]);
			}
			int 迭代次数 = 0;
			while (迭代次数 < 迭代总次数) {
				//以中心 对所有像素进行归类
				List<List<Color>> 归类结果 = new List<List<Color>> ();
				for (int i = 0; i < k; ++i) {
					归类结果.Add (new List<Color> ());
				}
				for (int i = 0; i < m; ++i) {
					for (int j = 0; j < n; ++j) {
						int 属于哪一类;
						float 最小距离 = float.MaxValue;
						for (int 类别 = 0; 类别 < k; 类别++) {
							float 距离 = 两个颜色之间的距离 (处理好后的图 [i, j], 聚类中心 [类别]);
							if (最小距离 > 距离) {
								属于哪一类 = 类别;
								最小距离 = 距离;
							} 
						}
						归类结果 [属于哪一类].Add (处理好后的图 [i, j]);
					}
				}

				//重新计算每一类的中心
				for (int i = 0; i < k; ++i) {
					if (归类结果 [i].Count > 0) {
						聚类中心 [i] = 计算聚类中心 (归类结果 [i]);
					} else {
						int 行 = Unity.Random.Range (0, m - 1);
						int 列 = Unity.Random.Range (0, n - 1);
						聚类中心 [i] = 处理好后的图 [行, 列];
					}

				}

				迭代次数++;
			}

			//三个值中有一个与其他两个值相关
			float 最小误差 = float.MaxValue;
			Color 中间颜色;
			for (int i = 0; i < k; ++i) 
			{
				Color 可能的中间颜色 = 聚类中心 [i];
				for (int j = 0; j < k; ++j) 
				{
					if (i == j)
						continue;
					Color 可能的第一个母色 = 聚类中心 [j];
					for (int q = 0; q < k; ++q) 
					{
						if (j == q || i == q)
							continue;
						Color 可能的第二个母色 = 聚类中心 [q];
						Color 两个母色的平均值 = (可能的第一个母色 + 可能的第二个母色) / 2;
						float 误差 = 两个颜色之间的距离 (两个母色的平均值, 可能的中间颜色);
						if (误差 < 最小误差) 
						{
							误差 = 最小误差;
							中间颜色 = 可能的中间颜色;
						}
					}
				}
			}

			return 中间颜色;
		}

		//计算一个List<Color>的中心
		public static Color 计算聚类中心(List<Color> 某一类颜色)
		{
			Color 总颜色 = new Color();
			foreach(var 颜色 in 某一类颜色)
			{
				总颜色 += 某一类颜色;
			}
			return 总颜色 / 某一类颜色.Count;
		}

		public static float 两个颜色之间的距离(Color 第一个颜色,Color 第二个颜色)
		{
			return Math.Abs (第一个颜色.r - 第二个颜色.r) + Math.Abs (第一个颜色.g - 第二个颜色.g) + Math.Abs (第一个颜色.b - 第二个颜色.b);
		}

	}
}
