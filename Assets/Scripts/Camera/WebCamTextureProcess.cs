using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ImageColor;
using UnityEngine.UI;
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif
using OpenCVForUnity;
using OpenCVForUnitySample;

/// <summary>
/// Web cam texture marker based AR sample.
/// </summary>
[RequireComponent(typeof(WebCamTextureToMatHelper))]
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

    public MeshRenderer PlaneRenderer;
    public bool IsCreateCollider = false;

    /// <summary>
    /// The should move AR camera.
    /// </summary>
    [Tooltip("If true, only the first element of markerSettings will be processed.")] public bool
        shouldMoveARCamera;

    /// <summary>
    /// The web cam texture to mat helper.
    /// </summary>
    WebCamTextureToMatHelper webCamTextureHelper;

    // Use this for initialization
    void Start()
    {

        webCamTextureHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
        webCamTextureHelper.Init();

    }

    /// <summary>
    /// Raises the web cam texture to mat helper inited event.
    /// </summary>
    public void OnWebCamTextureHelperInited()
    {
        Debug.Log("OnWebCamTextureToMatHelperInited");
        /*
        var webCamTextureMat = webCamTextureHelper.GetWebCamTexture();

        texture = new Texture2D(webCamTextureMat.width, webCamTextureMat.height, TextureFormat.RGBA32, false);



        gameObject.transform.localScale = new Vector3(webCamTextureMat.width, webCamTextureMat.height, 1);

        Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " +
                  Screen.orientation);

        

        float imageScale = 1.0f;
       
        if (widthScale < heightScale)
        {
            Camera.main.orthographicSize = (width * (float) Screen.height / (float) Screen.width) / 2;
            imageScale = (float) Screen.height / (float) Screen.width;
        }
        else
        {
            Camera.main.orthographicSize = height / 2;
        }

        //    gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        */
        float width = 0;
        float height = 0;

        width = gameObject.transform.localScale.x;
        height = gameObject.transform.localScale.y;
        float widthScale = (float)Screen.width / width;
        float heightScale = (float)Screen.height / height;
        
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

        //init lsd&&processImg
        Lsd = Imgproc.createLineSegmentDetector();

        Mat imgMat = webCamTextureHelper.GetMat();

        int originWidth = imgMat.width();
        int originHeight = imgMat.height();
        ProcessedImg = new Mat(originHeight, originWidth, CvType.CV_8UC3, new Scalar(255, 255, 255));
    }

   // public Texture2D TextureToDraw;
    public LineSegmentDetector Lsd;
    public Mat ProcessedImg;
    /// <summary>
    /// Raises the web cam texture to mat helper disposed event.
    /// </summary>
    public void OnWebCamTextureToMatHelperDisposed()
    {
        Debug.Log("OnWebCamTextureToMatHelperDisposed");

    }

    // Update is called once per frame
    float _calCount = 0;



    void Update()
    {
        if (webCamTextureHelper.isPlaying() && webCamTextureHelper.didUpdateThisFrame())
        {
            _calCount++;
            if (_calCount >= 15)
            {
                //ProcessTextureOpenCV();
                ProcessTexture2DOpenCV();
                //ProcessTexture();
                // SetPhysicsEdge(_lines);
                _calCount = 0;
            }
        }
    }

    /*
    void FixedUpdate()
    {
        if (webCamTextureHelper.isPlaying()) // && webCamTextureHelper.didUpdateThisFrame())
        {

            ProcessTexture();
            _calCount = 0;

        }

    }*/


    void ProcessTexture()
    {
        Profiler.BeginSample("Process Texture");
        var tex = webCamTextureHelper.GetWebCamTexture();
        // Mat cameraTextureMat = new Mat();
        //Utils.webCamTextureToMat(tex, cameraTextureMat);

        var pixels = tex.GetPixels();
        // MainClass.Color[,] colors = GetColor255_2DArray(tex, pixels);
        //var 原灰度 = MainClass.RGBToGray(colors, tex.height, tex.width);
        // var 图像增强 = MainClass.拉普拉斯(colors, tex.height, tex.width);
        //var 拉普拉斯灰度 = MainClass.RGBToGray(图像增强, tex.height, tex.width);
        //var 高斯滤波 = MainClass.Fspecial(colors, tex.height, tex.width);
        //var 高斯滤波灰度 = MainClass.RGBToGray(高斯滤波, tex.height, tex.width);
        var 原灰度 = MainClass.RGBToGray(pixels, tex.height, tex.width);
        int[] reg_img;
        int reg_x;
        int reg_y;
        int n_out = 0;
        Profiler.BeginSample("Call LSD");
        var lsd = LSD.Lsd_scale(ref n_out, 原灰度, tex.width, tex.height, 0.5);
        Profiler.EndSample();

        Texture2D texture = new Texture2D(tex.width, tex.width);

        // List<LineSegment> lines = new List<LineSegment>();
        _lines.Clear();

        // var lsd = LSD.Lsd(ref n_out, 原灰度, tex.width, tex.height);
        Profiler.BeginSample("Draw Line");
        for (int i = 0; i < n_out; ++i)
        {
            int x1, x2, y1, y2;
            //ImageCanvas.scaleFactor
            LSD.GetLineScale(i, lsd, out x1, out y1, out x2, out y2, new Vector2(tex.width, tex.height),
                new Vector2(tex.width, tex.height));
            _lines.Add(new LineSegment(new Vector2(x1, y1), new Vector2(x2, y2)));
            MainClass.DrawLine(texture, x1, y1, x2, y2, Color.blue);
        }
        Profiler.EndSample();

        Profiler.BeginSample("Draw Texture");
        DrawTexture2D(RenderImage, texture);
        Profiler.EndSample();

        if (IsCreateCollider)
        {
           // SetPhysicsEdge(_lines);
        }

        Profiler.EndSample();
    }

    [Range(0,1)]
    public float 裁剪上半部分比例 = 0.233f;
	public float 裁剪下半部分比例 = 0.251f;
	public float 裁剪左半部分比例 = 0f;
	public float 裁剪右半部分比例 = 0f;
	public Scrollbar BarUpward;
	public Scrollbar BarDownward;
	public Scrollbar BarRight;
	public Scrollbar BarLeft;

	public Scrollbar BarEdgeY;
	public Scrollbar BarEdgeX;
	public Text UpText;
	public Text DownText;
	public Text LeftText;
	public Text RightText;
	public Text EdgeYText;
	public Text EdgeXText;
	public void OnUpwardScrollValueChange()
	{
		裁剪上半部分比例 = BarUpward.value;
		UpText.text = 裁剪上半部分比例.ToString ();
	}

	public void OnDownScrollValueChange()
	{
		裁剪下半部分比例 = BarDownward.value;
		DownText.text = 裁剪下半部分比例.ToString ();
	}



	public void OnLeftValueChange()
	{
		裁剪左半部分比例 = BarLeft.value;
		LeftText.text = 裁剪左半部分比例.ToString();
	}

	public void OnRightValueChange()
	{
		裁剪右半部分比例 = BarRight.value;
		RightText.text = 裁剪右半部分比例.ToString();
	}

	public void OnEdgeYPositionValueChange()
	{
		offsetY = (BarEdgeY.value * 2 - 1);
		EdgeYText.text = offsetY.ToString ();
	}

	public void OnEdgeXPositionValueChange()
	{
		offsetX = (BarEdgeX.value * 2 - 1);
		EdgeXText.text = offsetX.ToString ();
	}

   // int curHeight = 0;
	float lineYToAdd = 0f;
    public float ImgScaleFactor = 0.4f;
    void ProcessTexture2DOpenCV()
    {
        Mat imgMat = webCamTextureHelper.GetMat();

       

        
        using (
            var resizedMat = new Mat((int) (imgMat.rows() * ImgScaleFactor), (int) (imgMat.cols() * ImgScaleFactor),
                CvType.CV_8UC4))
        {
            Imgproc.resize(imgMat, resizedMat, new Size(resizedMat.rows(), resizedMat.cols()));

            int originWidth = resizedMat.width();
            int originHeight = resizedMat.height();

            int up = (int)(originHeight * 裁剪上半部分比例);
            int down = (int)(originHeight * 裁剪下半部分比例);
            int left = (int)(originWidth * 裁剪左半部分比例);
            int right = (int)(originWidth * 裁剪右半部分比例);

            resizedMat.adjustROI(-up, -down, -left, -right);

            using (Mat grayMat = new Mat())
            {
                Imgproc.cvtColor(resizedMat, grayMat, Imgproc.COLOR_RGB2GRAY);
                resizedMat.adjustROI(up, down, left, right);
                using (Mat lines = new Mat())
                {
                    Lsd.detect(grayMat, lines);
                    if (lines.height() == 0) return;

                    float[] linesArray = new float[lines.cols() * lines.rows() * lines.channels()];
                    lines.get(0, 0, linesArray);
                    _lines.Clear();
                    ProcessedImg.setTo(Scalar.all(255));
                    for (int i = 0; i < linesArray.Length; i = i + 4)
                    {
                        float fromX = linesArray[i + 0] + left;
                        fromX /= ImgScaleFactor;
                        float fromY = linesArray[i + 1] + up;
                        fromY /= ImgScaleFactor;
                        float toX = linesArray[i + 2] + left;
                        toX /= ImgScaleFactor;
                        float toY = linesArray[i + 3] + up;
                        toY /= ImgScaleFactor;
                        Imgproc.line(ProcessedImg, new Point(fromX, fromY),
                            new Point(toX, toY), new Scalar(255, 86, 0), 2);
                        _lines.Add(new LineSegment(
                            new Vector2((int) fromX, (int) fromY),
                            new Vector2((int) toX, (int) toY)));
                    }
                    var textureToDraw = new Texture2D(ProcessedImg.width(), ProcessedImg.height(), TextureFormat.RGBA32, false);
                    Utils.matToTexture2D(ProcessedImg, textureToDraw);
                    DrawTexture2D(RenderImage, textureToDraw);
                    AdjectTexture2DAndCamera();
                }
            }
        }


        //  for test
        //  SetPhysicsEdge();
    }

    void ProcessTextureOpenCV()
    {
        Profiler.BeginSample("Process Texture CV");
        Profiler.BeginSample("GetMat");
        Mat imgMat = webCamTextureHelper.GetMat();
        Core.flip(imgMat, imgMat, 1);
        Profiler.EndSample();
        Mat grayMat = new Mat();
        Imgproc.cvtColor(imgMat, grayMat, Imgproc.COLOR_RGB2GRAY);
        var lsd = Imgproc.createLineSegmentDetector();


        Profiler.BeginSample("lsd");
        Mat lines = new Mat();
        lsd.detect(grayMat, lines);
        Profiler.EndSample();

        Profiler.BeginSample("drawSegments");
        Mat processedImg = new Mat(imgMat.rows(), imgMat.cols(), CvType.CV_8UC3, new Scalar(255, 255, 255));
      //  lsd.drawSegments(processedImg, lines);
        
		//draw and get lines
        float[] linesArray = new float[lines.cols() * lines.rows() * lines.channels()];
        lines.get(0, 0, linesArray);

        Texture2D texture = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);

        Profiler.EndSample();

        Profiler.BeginSample("DrawTexture");
        // Core.flip(processedImg, processedImg, 1);
        
        //  Utils.matToTexture(processedImg, texture);

        Utils.matToTexture(processedImg, texture);
        DrawTexture(PlaneRenderer, texture);
        SetTextureScaleAndPos();
        
		//GetLines(lines);
        Profiler.EndSample();



        Profiler.EndSample();
    }

    void SetTextureScaleAndPos()
    {
        var mainTexture = PlaneRenderer.material.mainTexture;
        ARCamera.orthographicSize = mainTexture.height / 2;
        var center = ARCamera.transform.position;
        float z = PlaneRenderer.transform.position.z;
        PlaneRenderer.transform.position = new Vector3(center.x, center.y, z);
        PlaneRenderer.transform.localScale = new Vector3(mainTexture.width, mainTexture.height, 1);
    }

    void AdjectTexture2DAndCamera()
    {
        var texture = RenderImage.sprite.texture;
        ARCamera.orthographicSize = (float)texture.height / RenderImage.sprite.pixelsPerUnit/ 2f;
        var center = ARCamera.transform.position;
        float z = RenderImage.transform.position.z;
        RenderImage.transform.position = new Vector3(center.x, center.y, z);
    }

    void GetLines(Mat lines)
    {

    }
    List<LineSegment> _lines = new List<LineSegment>();

    struct LineSegment
    {
        public Vector2 From;
        public Vector2 To;

        public LineSegment(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }
    }
    
    float offsetX = 0.5f;
    float offsetY = 0.5f;

    public void SetPhysicsEdge()
    {
        Profiler.BeginSample("SetPhysicsEdge");

        var edge = RenderImage.GetComponent<EdgeCollider2D>();
        var pos = RenderImage.transform.localPosition;
        float width = RenderImage.sprite.texture.width / RenderImage.sprite.pixelsPerUnit;
        float height = RenderImage.sprite.texture.height / RenderImage.sprite.pixelsPerUnit;
        var lbPos = new Vector3(pos.x - width *offsetX, pos.y + height*offsetY, pos.z);
        float unitPerPixel = 1 / RenderImage.sprite.pixelsPerUnit;
        //    List<Vector2> points = new List<Vector2>();
        CleanPhysicsEdge();
        foreach (var l in _lines)
        {
            float fromX = l.From.x * unitPerPixel + lbPos.x;
            float fromY = -l.From.y * unitPerPixel + lbPos.y;
            float toX = l.To.x * unitPerPixel + lbPos.x;
            float toY = -l.To.y * unitPerPixel + lbPos.y;
            //points.Add(new Vector2(fromX, fromY));
            //points.Add(new Vector2(toX, toY));
            CreateEdgeObject(RenderImage.transform, new Vector2(fromX, fromY), new Vector2(toX, toY));
        }
        Profiler.EndSample();
    }

    public void CleanPhysicsEdge()
    {
        var edges = RenderImage.GetComponentsInChildren<EdgeCollider2D>();
        foreach (var e in edges)
        {
            GameObject.Destroy(e.gameObject);
        }
    }
    void CreateEdgeObject(Transform parent, Vector2 from, Vector2 to)
    {
        GameObject go = new GameObject("line collider");
        var phy = go.AddComponent<EdgeCollider2D>();
        //  go.AddComponent<Rigidbody2D>();
        go.transform.parent = parent;
        go.transform.localPosition = new Vector3(0, 0, 0);
        Vector2[] points = new Vector2[2];
        points[0] = from;
        points[1] = to;
        phy.points = points;
    }

    public SpriteRenderer RenderImage;

    void DrawTexture2D(SpriteRenderer renderer, Texture2D texture)
    {
        if (renderer.sprite)
        {
            DestroyImmediate(renderer.sprite.texture);
            DestroyImmediate(renderer.sprite);
        }

        texture.Apply();
        
        //AfterImage.canvasRenderer.SetTexture(texture);
        //AfterImage.SetNativeSize();
        //AfterImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        //RenderCamera.targetTexture = texture;
        renderer.sprite = Sprite.Create(texture, new UnityEngine.Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));
    }

    void DrawTexture(MeshRenderer renderer, Texture texture)
    {
        renderer.material.mainTexture = texture;
    }

    MainClass.Color[,] GetColor255_2DArray(Texture t, Color[] colors)
    {
        MainClass.Color[,] cs = new MainClass.Color[t.height, t.width];
        for (int i = 0; i < t.height; ++i)
        {
            for (int j = 0; j < t.width; ++j)
            {

                cs[i, j] = new MainClass.Color(colors[i * (t.width) + j]); // * 255f);              
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



