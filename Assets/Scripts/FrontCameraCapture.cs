using UnityEngine;
using System.Collections;
using ImageColor;
using UnityEngine.UI;

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
            tex = new WebCamTexture(deviceName, 400, 300, 12);
            tex.Play(); //开始摄像
        }
    }

    public SpriteRenderer Renderer_;

    IEnumerator getTexture()
    {
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
        var outColors = ChangeTo01Colors(gray);
        var averageColor = AfterProcess.在轮廓中取色(GrayToTo255Colors(gray), colors);
        Debug.LogFormat("平均色：R{0} G{1} B{2}", averageColor.r, averageColor.g, averageColor.b);
        Texture2D texture = new Texture2D(t.width, t.height);
        texture.SetPixels(outColors);
        texture.Apply();
        AfterRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
    }
    void OnGUI()
    {
        //开始按钮
        if (GUI.Button(new Rect(0, 0, 100, 100), "click"))
        {
            //调用启动那个协程，开启摄像头
            StartCoroutine(test());
        }
        //暂停
        if (GUI.Button(new Rect(0, 200, 100, 30), "pause"))
        {
            tex.Pause();
            //这个方法就是保存图片
            StopAllCoroutines();
        }

        //重启开始
        if (GUI.Button(new Rect(0, 300, 100, 30), "restart"))
        {
            tex.Play();
        }
        if (GUI.Button(new Rect(100, 0, 100, 30), "摄像"))
        {
            //开始摄像，摄像就是一系列的图片集合
           StartCoroutine(getTexture2d());
            //StartCoroutine(getTexture());
        }
        if (tex != null)
            GUI.DrawTexture(new Rect(200, 200, 200, 180), tex);
      //  GUI.DrawTexture(new Rect(400, 300, 400, 300), tex, ScaleMode.ScaleToFit);
    }

    /// <summary>
    /// 获取摄像头截取的图片，这里也是一个协程
    /// </summary>
    /// <returns></returns>
    IEnumerator getTexture2d()
    {
        yield return new WaitForEndOfFrame();
        // while (true)
        // {
        // Texture2D t = new Texture2D(200, 180); //要保存图片的大小
        //截取的区域
        // tex.
        // t.ReadPixels(new Rect(200, 320, 200, 180), 0, 0, false);
        // t.Apply();
        var pixels = tex.GetPixels();
        //BeforeRenderer.sprite = Sprite.Create()
        MainClass.Color[,] colors = GetColor255_2DArray(tex, pixels);
        DrawTexture(BeforeRenderer, pixels);
		var gray = MainClass.ReadOutline(colors);
		EmptyClass.获取轮廓坐标 (gray, gray.GetLength (0), gray.GetLength (1));
        var outColors = ChangeTo01Colors(gray);
        //BeforeProcess.找四个角(colors);
        //var averageColor = AfterProcess.在轮廓中取色(GrayToTo255Colors(gray), colors);
        //Debug.LogFormat("平均色：R{0} G{1} B{2}", averageColor.r, averageColor.g, averageColor.b);
        //SetTextureColor(averageColor.ChangeToUnityColor());
        //AfterRenderer.sprite
        Texture2D texture = new Texture2D(tex.width, tex.height);
        texture.SetPixels(outColors);
        texture.Apply();
        AfterRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 200, 180), new Vector2(0.5f, 0.5f));
    }

    void DrawTexture(SpriteRenderer renderer, Color[] colors)
    {
        Texture2D texture = new Texture2D(tex.width, tex.height);
        texture.SetPixels(colors);
        texture.Apply();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, 200, 180), new Vector2(0.5f, 0.5f));
    }

    public SpriteRenderer CenterColor;
    void SetTextureColor(Color color)
    {
        CenterColor.color = color;
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

                cs[i, j] = new MainClass.Color(colors[i * (t.width) + j]);// * 255f;
                
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

    MainClass.Color[,] GrayToTo255Colors(double[,] gray)
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
}

