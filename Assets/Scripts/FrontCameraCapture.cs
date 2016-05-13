using UnityEngine;
using System.Collections;
using ImageColor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;

public class FrontCameraCapture : MonoBehaviour
{
    public string deviceName;
    WebCamTexture tex; //接收返回的图片数据

    /// <summary>
    /// 实现IEnumerator接口，这里使用了一个协程，相当于多线程。
    /// 这里是调用摄像头的方法。
    /// </summary>
    /// <returns></returns>
    IEnumerator test()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam); //授权
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            deviceName = devices[0].name;
            //设置摄像机摄像的区域
            _start = true;
            tex = new WebCamTexture(deviceName,
                                   ImageCanvas.worldCamera.pixelWidth, ImageCanvas.worldCamera.pixelHeight, 12);
            //planeT.renderer.material.mainTexture = tex;
        }
    }

    public SpriteRenderer Renderer_;

    public RawImage AfterImage;
    public Canvas ImageCanvas;
    IEnumerator getTexture()
    {
        var c = GetComponent<EdgeCollider2D>();
       
        yield return new WaitForEndOfFrame();
        // while (true)
        // {
        // Texture2D t = new Texture2D(200, 180); //要保存图片的大小
        //截取的区域
        // tex.
        // t.ReadPixels(new Rect(200, 320, 200, 180), 0, 0, false);
        // t.Apply();
        var t = Renderer_.sprite.texture;
        var pixels = t.GetPixels();
        //BeforeRenderer.sprite = Sprite.Create()
        MainClass.Color[,] colors = GetColor255_2DArray(t, pixels);
        
        var gray = MainClass.ReadOutline(colors);
        
        var out01Colors = ChangeTo01Colors(gray);
        var out255Colors = GrayTo255Colors(gray);

        var averageColor = AfterProcess.在轮廓中取色(GrayTo255Colors(gray), colors);
        Debug.LogFormat("平均色：R{0} G{1} B{2}", averageColor.r, averageColor.g, averageColor.b);
        Texture2D texture = new Texture2D(t.width, t.height);
        texture.SetPixels(out01Colors);
        texture.Apply();
        
       // AfterRenderer.sprite = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), new Vector2(0.5f, 0.5f));
    }
    public float DrawTextureX = 400f;
    public float DrawTextureY = 400f;
    public float DrawTextureWidth = 400f;
    public float DrawTextureHeight = 300f;
    
    void Start()
    {
       // StartCoroutine(test());
       // test();
    }
    void OnGUI()
    {
        //开始按钮
        if (GUI.Button(new Rect(0, 0, 100, 100), "click"))
        {
            //调用启动那个协程，开启摄像头
            StartCoroutine(test());
        }
        if (GUI.Button(new Rect(100, 0, 100, 30), "摄像"))
        {
            //开始摄像，摄像就是一系列的图片集合
          // StartCoroutine(getTexture2d());
            //StartCoroutine(getTexture());
        }
      //  if (tex != null)
           // GUI.DrawTexture(new Rect(DrawTextureX, DrawTextureY, tex.width, tex.height), tex);
      //  GUI.DrawTexture(new Rect(400, 300, 400, 300), tex, ScaleMode.ScaleToFit);
    }

    bool _start = false;
    int _count = 0;
    void LateUpdate()
    {
        if (_start)
        {
            _count++;
            if (_count >= 5)
            {
                //StartCoroutine(getTexture2d());
                ProcessTexture();
                _count = 0;
            }           
        }
        
    }

    public SpriteRenderer Renderer1;
    public SpriteRenderer Renderer2;
    public SpriteRenderer Renderer3;

   // public Camera RenderCamera;
    void ProcessTexture()
    {
        var pixels = tex.GetPixels();
        MainClass.Color[,] colors = GetColor255_2DArray(tex, pixels);
        var 原灰度 = MainClass.RGBToGray(colors, tex.height, tex.width);
        // var 图像增强 = MainClass.拉普拉斯(colors, tex.height, tex.width);
        //var 拉普拉斯灰度 = MainClass.RGBToGray(图像增强, tex.height, tex.width);
        //var 高斯滤波 = MainClass.Fspecial(colors, tex.height, tex.width);
        //var 高斯滤波灰度 = MainClass.RGBToGray(高斯滤波, tex.height, tex.width);

        // DrawTexture(Renderer1, ChangeTo01Colors(原灰度), tex.height, tex.width);
        //  DrawTexture(Renderer2, ChangeTo01Colors(拉普拉斯灰度), tex.height, tex.width);
        //  DrawTexture(Renderer3, ChangeTo01Colors(高斯滤波灰度), tex.height, tex.width);
        int[] reg_img;
        int reg_x;
        int reg_y;
        int n_out = 0;

        //var lsd = LSD.Lsd_scale_region(ref n_out, FlattenDoubleArray(拉普拉斯灰度), tex.width, tex.height,
        //   0.5f, out reg_img, out reg_x, out reg_y);
        //   var scale = ImageCanvas.GetComponent<CanvasScaler>().
        var lsd = LSD.Lsd(ref n_out, FlattenDoubleArray(原灰度), tex.width, tex.height);
        //  AfterImage.canvasRenderer.set
        Texture2D texture = new Texture2D(tex.width, tex.height);
        Color[] empty = new Color[tex.width * tex.height];
        for (int i = 0; i < empty.Length; ++i)
        {
            empty[i] = new Color(0, 0, 0, 0);
        }
        texture.SetPixels(empty);
        for (int i = 0; i < n_out; ++i)
        {
            int x1, x2, y1, y2;
            //ImageCanvas.scaleFactor
            LSD.GetLineScale(i, lsd, out x1, out y1, out x2, out y2, new Vector2(tex.width, tex.height),
                new Vector2(tex.width, tex.height));

            MainClass.DrawLine(texture, x1, y1, x2, y2, Color.blue);
        }
        // var finalColor = IntTo01Colors(reg_img);
        //DrawTexture(AfterRenderer, , reg_y, reg_x);
        DrawTexture(AfterRenderer, texture);

    }

    //void SetColliderEdge(int x1, int y1, int x2, int y2)
    Color[] IntTo01Colors(int[] gray)
    {
        Color[] colors = new Color[gray.Length];
        for (int i = 0; i < gray.Length; ++i)
        {
            var g = (float) gray[i]; ///255f;
            colors[i] = new Color(g, g, g);

        }
        return colors;
    }

    /*
    Color[] ChangeTo01Colors(double[] gray)
    {
        Color[] colors = new Color[gray.Length];
        for (int height = 0; height < gray.GetLength(0); ++height)
        {
            for (int width = 0; width < gray.GetLength(1); ++width)
            {
                var g = (float)gray[height, width] / 255f;
                colors[height * (gray.GetLength(1)) + width] = new Color(g, g, g);
            }
        }
        return colors;
    }*/

    double[] FlattenDoubleArray(double[,] img)
    {
        double[] i = new double[img.Length];

        for (int height = 0; height < img.GetLength(0); ++height)
        {
            for (int width = 0; width < img.GetLength(1); ++width)
            {
                i[height * (img.GetLength(1)) + width] = img[height, width];
            }
        }
        return i;
    }
    /// <summary>
    /// 获取摄像头截取的图片，这里也是一个协程
    /// </summary>
    /// <returns></returns>
    void  getTexture2d()
    {
      //  yield return new WaitForEndOfFrame();
        // while (true)
        // {
        // Texture2D t = new Texture2D(200, 180); //要保存图片的大小
        //截取的区域
        // tex.
        // t.ReadPixels(new Rect(200, 320, 200, 180), 0, 0, false);
        // t.Apply();
        var pixels = tex.GetPixels();
      //  DrawTexture(AfterRenderer, pixels);
        
        MainClass.Color[,] colors = GetColor255_2DArray(tex, pixels);
        //DrawTexture(BeforeRenderer, pixels);
        var gray = MainClass.ReadOutline(colors);
        var point = MainClass.获取轮廓坐标 (gray, gray.GetLength (0), gray.GetLength (1));
        //var outColors = ChangeTo01Colors(gray);
         //DrawTexture(AfterRenderer, AddCoordToTexture(point, gray.GetLength(0), gray.GetLength(1)));

        /*
        //BeforeProcess.找四个角(colors);
        var averageColor = AfterProcess.在轮廓中取色(GrayTo255Colors(gray), colors);
        Debug.LogFormat("平均色：R{0} G{1} B{2}", averageColor.r, averageColor.g, averageColor.b);
        SetTextureColor(averageColor.ChangeToUnityColor());
        //AfterRenderer.sprite      
        /*
        Texture2D texture = new Texture2D(tex.width, tex.height);
        texture.SetPixels(outColors);
        texture.Apply();
        var texture1 = AddGrayColorToTexture(gray);
        AfterRenderer.sprite = Sprite.Create(texture1, new Rect(0, 0, texture1.width, texture1.height), new Vector2(0.5f, 0.5f));
   */
    }

    void DrawTexture(SpriteRenderer renderer, Texture2D texture)
    {
       texture.Apply();
        AfterImage.canvasRenderer.SetTexture(texture);
        AfterImage.SetNativeSize();
        AfterImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        //RenderCamera.targetTexture = texture;
       //renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    void DrawTexture(SpriteRenderer renderer, Color[] colors, int height, int width)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(colors);
        texture.Apply();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    public SpriteRenderer BeforeRenderer;
    public SpriteRenderer AfterRenderer;
    MainClass.Color[,] GetColor255_2DArray(Texture t, Color[] colors)
    {
        MainClass.Color[,] cs = new MainClass.Color[t.height, t.width];
        for (int i = 0; i < t.height; ++i)
        {
            for (int j = 0; j < t.width; ++j)
            {

                cs[i, j] = new MainClass.Color(colors[i * (t.width) + j]);// * 255f);              
            }
        }
        return cs;
    }

    public Color GreyColor = new Color(133, 133, 133);

    Color[] ChangeTo01Colors(double[,] grey)
    {
        Color[] colors = new Color[grey.Length];
        for (int height = 0; height < grey.GetLength(0); ++height)
        {
            for (int width = 0; width < grey.GetLength(1); ++width)
            {
                var g = (float) grey[height, width]/255f;
                colors[height * (grey.GetLength(1)) + width] = new Color(g, g, g);
            }
        }
        return colors;
    }

    MainClass.Color[,] GrayTo255Colors(double[,] gray)
    {
        MainClass.Color[,] colors = new MainClass.Color[gray.GetLength(0), gray.GetLength(1)];
        for (int height = 0; height < gray.GetLength(0); ++height)
        {
            for (int width = 0; width < gray.GetLength(1); ++width)
            {
                var g = (float)gray[height, width];
                colors[height, width] = new MainClass.Color(g,g,g);
            }
        }
        return colors;
    }

    Color[] AddGrayColorToCameraTexture(double[,] gray)
    {
        var colors = tex.GetPixels();
        for (int height = 0; height < gray.GetLength(0); ++height)
        {
            for (int width = 0; width < gray.GetLength(1); ++width)
            {
                if (gray[height, width] > 200)
                {
                    colors[height * (gray.GetLength(1)) + width] = Color.blue;
                 //   tex.(height, width, Color.blue);
                }
            }
        }
        return colors;
    }

    Color[] AddCoordToCameraTexture(List<List<Coord>> coords, int heightLength, int widthLength)
    {
        var colors = tex.GetPixels();
        int count = 0;
        foreach (var line in coords)
        {
            
            if (line.Count <= 3) continue;
            foreach (var pos in line)
            {
                colors[pos.height * (widthLength) + pos.width] = Color.blue;
            }
            ++count;
        }
        Debug.LogFormat("Line Count:{0}", count);
        return colors;
    }

    Color[] AddCoordToTexture(List<List<Coord>> coords,int heightLength, int widthLength)
    {
        Color[] colors = new Color[heightLength * widthLength];
        int count = 0;
        foreach (var line in coords)
        {

            if (line.Count < 4) continue;
            foreach (var pos in line)
            {
                colors[pos.height * (widthLength) + pos.width] = Color.blue;
            }
            ++count;
        }
       // Debug.LogFormat("Line Count:{0}", count);
        return colors;
    }
}

