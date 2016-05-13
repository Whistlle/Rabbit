using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ImageColor;
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Web cam texture marker based AR sample.
/// </summary>
[RequireComponent(typeof(WebCamTextureHelper))]
public class WebCamTextureProcess : MonoBehaviour
{

    /// <summary>
    /// The colors.
    /// </summary>
    Color32[] colors;

    /// <summary>
    /// The texture.
    /// </summary>
    Texture2D texture;

    /// <summary>
    /// The AR camera.
    /// </summary>
    public Camera ARCamera;


    /// <summary>
    /// The invert Y.
    /// </summary>
    Matrix4x4 invertYM;

    /// <summary>
    /// The transformation m.
    /// </summary>
    Matrix4x4 transformationM;

    /// <summary>
    /// The invert Z.
    /// </summary>
    Matrix4x4 invertZM;

    /// <summary>
    /// The should move AR camera.
    /// </summary>
    [Tooltip("If true, only the first element of markerSettings will be processed.")] public bool
        shouldMoveARCamera;

    /// <summary>
    /// The web cam texture to mat helper.
    /// </summary>
    WebCamTextureHelper webCamTextureHelper;

    // Use this for initialization
    void Start()
    {

        webCamTextureHelper = gameObject.GetComponent<WebCamTextureHelper>();
        webCamTextureHelper.Init();

    }

    /// <summary>
    /// Raises the web cam texture to mat helper inited event.
    /// </summary>
    public void OnWebCamTextureHelperInited()
    {
        Debug.Log("OnWebCamTextureToMatHelperInited");

        var webCamTextureMat = webCamTextureHelper.GetWebCamTexture();

        texture = new Texture2D(webCamTextureMat.width, webCamTextureMat.height, TextureFormat.RGBA32, false);



        gameObject.transform.localScale = new Vector3(webCamTextureMat.width, webCamTextureMat.height, 1);

        Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " +
                  Screen.orientation);

        float width = 0;
        float height = 0;

        width = gameObject.transform.localScale.x;
        height = gameObject.transform.localScale.y;

        float imageScale = 1.0f;
        float widthScale = (float) Screen.width / width;
        float heightScale = (float) Screen.height / height;
        if (widthScale < heightScale)
        {
            Camera.main.orthographicSize = (width * (float) Screen.height / (float) Screen.width) / 2;
            imageScale = (float) Screen.height / (float) Screen.width;
        }
        else
        {
            Camera.main.orthographicSize = height / 2;
        }

        gameObject.GetComponent<Renderer>().material.mainTexture = texture;


        double apertureWidth = 0;
        double apertureHeight = 0;
        double[] fovx = new double[1];
        double[] fovy = new double[1];
        double[] aspectratio = new double[1];

        //Adjust Unity Camera FOV
        if (widthScale < heightScale)
        {
            ARCamera.fieldOfView = (float) fovx[0];
        }
        else
        {
            ARCamera.fieldOfView = (float) fovy[0];
        }
    }

    /// <summary>
    /// Raises the web cam texture to mat helper disposed event.
    /// </summary>
    public void OnWebCamTextureToMatHelperDisposed()
    {
        Debug.Log("OnWebCamTextureToMatHelperDisposed");

    }

    // Update is called once per frame
    float _calCount = 0;
    void LateUpdate()
    {

        if (webCamTextureHelper.isPlaying() && webCamTextureHelper.didUpdateThisFrame())
        {
            _calCount++;
            if (_calCount >= 5)
            {
                ProcessTexture();
                _calCount = 0;
            }
        }

    }

    

    void ProcessTexture()
    {
        var tex = webCamTextureHelper.GetWebCamTexture();
        var pixels = tex.GetPixels();
        MainClass.Color[,] colors = GetColor255_2DArray(tex, pixels);
        var 原灰度 = MainClass.RGBToGray(colors, tex.height, tex.width);
        // var 图像增强 = MainClass.拉普拉斯(colors, tex.height, tex.width);
        //var 拉普拉斯灰度 = MainClass.RGBToGray(图像增强, tex.height, tex.width);
        //var 高斯滤波 = MainClass.Fspecial(colors, tex.height, tex.width);
        //var 高斯滤波灰度 = MainClass.RGBToGray(高斯滤波, tex.height, tex.width);

        int[] reg_img;
        int reg_x;
        int reg_y;
        int n_out = 0;

        var lsd = LSD.Lsd(ref n_out, FlattenDoubleArray(原灰度), tex.width, tex.height);
        //  AfterImage.canvasRenderer.set
        // Texture2D texture = new Texture2D(tex.width, tex.height);
        Texture2D texture = new Texture2D(tex.width, tex.width);
        //   Color[] empty = new Color[tex.width* tex.height];
        //  for (int i = 0; i < empty.Length; ++i)
        //  {
        //y      empty[i] = new Color(0,0,0,0);
        //  }
        //  texture.SetPixels(empty);
        List<LineSegment> lines = new List<LineSegment>();
        for (int i = 0; i < n_out; ++i)
        {
            int x1, x2, y1, y2;
            //ImageCanvas.scaleFactor
            LSD.GetLineScale(i, lsd, out x1, out y1, out x2, out y2, new Vector2(tex.width, tex.height),
                new Vector2(tex.width, tex.height));
            lines.Add(new LineSegment(new Coord(y1, x1), new Coord(y2, x2)));
            MainClass.DrawLine(texture, x1, y1, x2, y2, Color.blue);
        }
        // var finalColor = IntTo01Colors(reg_img);
        //DrawTexture(AfterRenderer, , reg_y, reg_x);
        DrawTexture(RenderImage, texture);
        SetPhysicsEdge(lines);
    }

    struct LineSegment
    {
        public Coord From;
        public Coord To;

        public LineSegment(Coord from, Coord to)
        {
            From = from;
            To = to;
        }
    }

    void SetPhysicsEdge(List<LineSegment> lines)
    {
        var edge = RenderImage.GetComponent<EdgeCollider2D>();
        var pos = RenderImage.transform.localPosition;
        float width = RenderImage.sprite.texture.width / RenderImage.sprite.pixelsPerUnit;
        float height = RenderImage.sprite.texture.height / RenderImage.sprite.pixelsPerUnit;
        var lbPos = new Vector3(pos.x - width / 2, pos.y - height / 2, pos.z);
        float unitPerPixel = 1 / RenderImage.sprite.pixelsPerUnit;
        //    List<Vector2> points = new List<Vector2>();
        var edges = RenderImage.GetComponentsInChildren<EdgeCollider2D>();
        foreach (var e in edges)
        {
            GameObject.Destroy(e.gameObject);
        }
        foreach (var l in lines)
        {
            float fromX = l.From.width * unitPerPixel + lbPos.x;
            float fromY = l.From.height * unitPerPixel + lbPos.y;
            float toX = l.To.width * unitPerPixel + lbPos.x;
            float toY = l.To.height * unitPerPixel + lbPos.y;
            //points.Add(new Vector2(fromX, fromY));
            //points.Add(new Vector2(toX, toY));
            CreateEdgeObject(RenderImage.transform, new Vector2(fromX, fromY), new Vector2(toX, toY));
        }
    }

    void CreateEdgeObject(Transform parent, Vector2 from, Vector2 to)
    {
        GameObject go = new GameObject("line collider");
        var phy = go.AddComponent<EdgeCollider2D>();
        go.transform.parent = parent;
        go.transform.localPosition = new Vector3(0,0,0);
        Vector2[] points = new Vector2[2];
        points[0] = from;
        points[1] = to;
        phy.points = points;
    }
    public SpriteRenderer RenderImage;
    void DrawTexture(SpriteRenderer renderer, Texture2D texture)
    {
        texture.Apply();
        //AfterImage.canvasRenderer.SetTexture(texture);
        //AfterImage.SetNativeSize();
        //AfterImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        //RenderCamera.targetTexture = texture;
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

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
    /// Raises the disable event.
    /// </summary>
    void OnDisable()
    {
        webCamTextureHelper.Dispose();
    }

    /// <summary>
    /// Raises the back button event.
    /// </summary>
    public void OnBackButton()
    {
#if UNITY_5_3
        SceneManager.LoadScene("MarkerBasedARSample");
#else
						Application.LoadLevel ("MarkerBasedARSample");
#endif
    }

    /// <summary>
    /// Raises the play button event.
    /// </summary>
    public void OnPlayButton()
    {
        webCamTextureHelper.Play();
    }

    /// <summary>
    /// Raises the pause button event.
    /// </summary>
    public void OnPauseButton()
    {
        webCamTextureHelper.Pause();
    }

    /// <summary>
    /// Raises the stop button event.
    /// </summary>
    public void OnStopButton()
    {
        webCamTextureHelper.Stop();
    }

    /// <summary>
    /// Raises the change camera button event.
    /// </summary>
    public void OnChangeCameraButton()
    {
        webCamTextureHelper.Init(null, webCamTextureHelper.requestWidth,
            webCamTextureHelper.requestHeight, !webCamTextureHelper.requestIsFrontFacing);
    }

}



