using System.Collections.Generic;
using UnityEngine;

namespace ImageColor
{
    /// <summary>
    /// 预处理
    /// </summary>
    public static class BeforeProcess
    {
        public static void 找四个角(MainClass.Color[,] image)
        {
            int lastlastBlackCount = -1;
            int lastBlackCount = -1;
            
            List<Coord> 四个角 = new List<Coord>(4);

            //下到上，左至右
            for (int h = 0; h < image.GetLength(0); ++h)
            {
                int blackCount = 0;
                Coord? blackLineBegin = null;
                bool lastPointBlack = false;
                for (int w = 0; w < image.GetLength(1); ++w)
                {
                    //扫每行获取黑色数值
                    if (IsBlack(image[h, w]))
                    {
                        if (blackCount == 0)
                        {
                            ++blackCount;
                            lastPointBlack = true;
                            if (blackLineBegin == null)
                            {
                                blackLineBegin = new Coord(h, w);
                            }
                        }
                        else if (lastPointBlack)
                        {
                            ++blackCount;
                        }                                          
                    }
                    else
                    {
                        blackCount = 0;
                        blackLineBegin = null;
                        lastPointBlack = false;
                    }
                }
                if (blackCount == 0) continue;
                if (lastBlackCount < 0 || lastlastBlackCount < 0)
                {
                    continue;
                }
                
                //中间那行比上下两行都大
                if (lastBlackCount >= lastlastBlackCount && lastBlackCount > blackCount)
                {
                    if (blackLineBegin.HasValue)
                    {
                        四个角.Add(blackLineBegin.Value);
                        return;
                    }
                }
                lastlastBlackCount = lastBlackCount;
                lastBlackCount = blackCount;
            }

            //上到下，左至右
            for (int h = image.GetLength(0)-1; h >=0; --h)
            {
                int blackCount = 0;
                Coord? blackLineBegin = null;
                bool lastPointBlack = false;
                for (int w = 0; w < image.GetLength(1); ++w)
                {
                    //扫每行获取黑色数值
                    if (IsBlack(image[h, w]))
                    {
                        if (blackCount == 0)
                        {
                            ++blackCount;
                            lastPointBlack = true;
                            if (blackLineBegin == null)
                            {
                                blackLineBegin = new Coord(h, w);
                            }
                        }
                        else if (lastPointBlack)
                        {
                            ++blackCount;
                        }
                    }
                    else
                    {
                        blackCount = 0;
                        blackLineBegin = null;
                        lastPointBlack = false;
                    }
                }
                if (blackCount == 0) continue;
                if (lastBlackCount < 0 || lastlastBlackCount < 0)
                {
                    continue;
                }

                //中间那行比上下两行都大
                if (lastBlackCount >= lastlastBlackCount && lastBlackCount > blackCount)
                {
                    if (blackLineBegin.HasValue)
                    {
                        四个角.Add(blackLineBegin.Value);
                        return;
                    }
                }
                lastlastBlackCount = lastBlackCount;
                lastBlackCount = blackCount;
            }

            //下到上，右至左
            for (int h = 0; h < image.GetLength(0); ++h)
            {
                int blackCount = 0;
                Coord? blackLineBegin = null;
                bool lastPointBlack = false;
                for (int w = 0; w < image.GetLength(1); ++w)
                {
                    //扫每行获取黑色数值
                    if (IsBlack(image[h, w]))
                    {
                        if (blackCount == 0)
                        {
                            ++blackCount;
                            lastPointBlack = true;
                            if (blackLineBegin == null)
                            {
                                blackLineBegin = new Coord(h, w);
                            }
                        }
                        else if (lastPointBlack)
                        {
                            ++blackCount;
                        }
                    }
                    else
                    {
                        blackCount = 0;
                        blackLineBegin = null;
                        lastPointBlack = false;
                    }
                }
                if (blackCount == 0) continue;
                if (lastBlackCount < 0 || lastlastBlackCount < 0)
                {
                    continue;
                }

                //中间那行比上下两行都大
                if (lastBlackCount >= lastlastBlackCount && lastBlackCount > blackCount)
                {
                    if (blackLineBegin.HasValue)
                    {
                        四个角.Add(blackLineBegin.Value);
                        return;
                    }
                }
                lastlastBlackCount = lastBlackCount;
                lastBlackCount = blackCount;
            }
            //上到下 右至左
            for (int h = image.GetLength(0) - 1; h >= 0; --h)
            {
                int blackCount = 0;
                Coord? blackLineBegin = null;
                bool lastPointBlack = false;
                for (int w = 0; w < image.GetLength(1); ++w)
                {
                    //扫每行获取黑色数值
                    if (IsBlack(image[h, w]))
                    {
                        if (blackCount == 0)
                        {
                            ++blackCount;
                            lastPointBlack = true;
                            if (blackLineBegin == null)
                            {
                                blackLineBegin = new Coord(h, w);
                            }
                        }
                        else if (lastPointBlack)
                        {
                            ++blackCount;
                        }
                    }
                    else
                    {
                        blackCount = 0;
                        blackLineBegin = null;
                        lastPointBlack = false;
                    }
                }
                if (blackCount == 0) continue;
                if (lastBlackCount < 0 || lastlastBlackCount < 0)
                {
                    continue;
                }

                //中间那行比上下两行都大
                if (lastBlackCount >= lastlastBlackCount && lastBlackCount > blackCount)
                {
                    if (blackLineBegin.HasValue)
                    {
                        四个角.Add(blackLineBegin.Value);
                        return;
                    }
                }
                lastlastBlackCount = lastBlackCount;
                lastBlackCount = blackCount;
            }
            if(四个角.Count!= 4)
                Debug.LogErrorFormat("找四个角: Error! 输出了{0}个坐标", 四个角.Count);
            for (int i = 0; i < 四个角.Count; ++i)
            {
                Debug.LogFormat("{0}: x:{1} y:{2}", i, 四个角[i].width, 四个角[i].height);
            }
        }

        /// <summary>
        /// Gray = (R*299 + G*587 + B*114 + 500) / 1000
		/// RGB转灰度
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static bool IsBlack(MainClass.Color color)
        {
            if (color == MainClass.Color.BLACK) return true;
            float gray = 灰度值(color);
            Vector4 line0_255 = new Vector4(0f,0f,0f,0f) - new Vector4(255f,255f,255f);
            float cos = Vector4.Dot(line0_255, (Vector4) color) / (line0_255.magnitude * ((Vector4) color).magnitude);
            float sin = Mathf.Sqrt(1 - cos * cos);
            float distance = sin * ((Vector4) color).magnitude;
           // float distance = Vector4.Distance((Vector4) MainClass.Color.WHITE, (Vector4) color);
            if (distance < 15 && gray < 111)
            {
                return true;
            }
            return false;
        }

        public static float 灰度值(MainClass.Color color)
        {
            return (color.r * 299f + color.g * 587f + color.b * 114f) / 1000f;
        }
    }
}
