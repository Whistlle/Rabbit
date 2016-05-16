using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace ImageColor
{
    public static class LSD
    {
#if UNITY_EDITOR
        [DllImport("lsd", EntryPoint = "lsd")]
        extern static IntPtr Intercall_lsd(ref int n_out,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
            int X, int Y);

        [DllImport("lsd", EntryPoint = "lsd_scale_region")]
        extern static IntPtr Intercall_lsd_scale_region(ref int n_out,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
            int X, int Y,
            double scale,
            IntPtr ref_img,
            ref int reg_x, ref int reg_y);

        //double* lsd_scale(int* n_out, double* img, int X, int Y, double scale);
        [DllImport("lsd", EntryPoint = "lsd_scale")]
        extern static IntPtr Intercall_lsd_scale(ref int n_out,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
            int X, int Y,
            double scale);
#elif UNITY_ANDROID
       [DllImport("liblsd_android", EntryPoint = "lsd", CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr Intercall_lsd(ref int n_out,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
            int X, int Y);

        [DllImport("liblsd_android", EntryPoint = "lsd_scale")]
        extern static IntPtr Intercall_lsd_scale(ref int n_out,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
            int X, int Y,
            double scale);

        [DllImport("liblsd_android", EntryPoint = "lsd_scale_region", CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr Intercall_lsd_scale_region(ref int n_out,
            [In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
            int X, int Y,
            double scale,
            IntPtr ref_img,
            ref int reg_x, ref int reg_y);
#elif  UNITY_IOS
		[DllImport("liblsd_android", EntryPoint = "lsd", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr Intercall_lsd(ref int n_out,
			[In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
			int X, int Y);

		[DllImport("liblsd_android", EntryPoint = "lsd_scale")]
		extern static IntPtr Intercall_lsd_scale(ref int n_out,
			[In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
			int X, int Y,
			double scale);

		[DllImport("liblsd_android", EntryPoint = "lsd_scale_region", CallingConvention = CallingConvention.Cdecl)]
		extern static IntPtr Intercall_lsd_scale_region(ref int n_out,
			[In, Out] [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] double[] img,
			int X, int Y,
			double scale,
			IntPtr ref_img,
			ref int reg_x, ref int reg_y);
#endif

        #region Wrapper
        public static double[] Lsd(ref int n_out, double[] img, int X, int Y)
        {
         //   var re = Intercall_lsd(ref n_out, img, X, Y);
            double[] value = new double[7 * n_out];
            //Marshal.Copy(re, value, 0, 7 * n_out);
            return value;
        }

        public static double[] Lsd_scale(ref int n_out, double[] img, int X, int Y, double scale)
        {
         //   var re = Intercall_lsd_scale(ref n_out, img, X, Y, scale);
            double[] value = new double[7 * n_out];
           // Marshal.Copy(re, value, 0, 7 * n_out);
            return value;
        }

        public static double[] Lsd_scale_region(ref int n_out,
            double[] img,
            int X, int Y,
            double scale,
            out int[] ref_img,
            out int reg_x, out int reg_y)
        {

            //IntPtr[] ptrs = new IntPtr[(int)(Y*scale)];
            //IntPtr intptr = Marshal.AllocHGlobal(sizeof(int));
            // var bak = intptr;

            reg_x = reg_y = 0;
          //  var re = Intercall_lsd_scale_region(ref n_out, img, X, Y, 0.8f, intptr, ref reg_x, ref reg_y);
           
            // Marshal.FreeHGlobal(bak);
            ref_img = new int[reg_y* reg_x];
          //  Marshal.Copy(intptr, ref_img, 0, reg_y* reg_x);
          //  Marshal.FreeHGlobal(intptr);
            
          //  ref_img = new int[reg_y, reg_x];
            /*
            for (int i = 0; i < reg_y; ++i)
            {
                int[] intArray = new int[reg_x];
                Marshal.Copy(lsdOut[i], intArray, 0, reg_x);
                for (int j = 0; j < reg_x; ++j)
                {
                    ref_img[i, j] = intArray[j];
                }
            }*/
             double[] value = new double[7 * n_out];
           //    Marshal.Copy(re, value, 0, 7 * n_out);
           //Marshal.FreeHGlobal(re);
              return value;
        }

        #endregion
        public static void GetLine(int n, double[] lines, out int x1, out int y1, out int x2, out int y2)
        {
            int index = 7 * (n);
            x1 = (int)(lines[index+0]);
            y1 = (int)(lines[index+1]);
            x2 = (int)(lines[index+2]);
            y2 = (int)(lines[index+3]);
        }

        public static void GetLineScale(int n, double[] lines, out int x1, out int y1, out int x2, out int y2, Vector2 originScale, Vector2 toScale)
        {
            int index = 7 * (n);
            x1 = (int)(lines[index + 0]);
            y1 = (int)(lines[index + 1]);
            x2 = (int)(lines[index + 2]);
            y2 = (int)(lines[index + 3]);
            var scale = new Vector2(toScale.x / originScale.x,  toScale.y / originScale.y);

            x1 = (int) (x1 * scale.x);
            x2 = (int) (x2 * scale.x);
            y1 = (int) (y1 * scale.y);
            y2 = (int) (y2 * scale.y);
        }

        public static double[] Lsd_scale_region(ref int n_out,
            double[] img,
            int X, int Y,
            double scale)
        {

            IntPtr intptr = new IntPtr();

            int reg_x = 0;
            int reg_y = 0;

            //var re = Intercall_lsd_scale_region(ref n_out, img, X, Y, scale, intptr, ref reg_x, ref reg_y);

            double[] value = new double[7 * n_out];
            //Marshal.Copy(re, value, 0, 7 * n_out);
            //Marshal.FreeHGlobal(re);
            return value;
        }
    }
}
