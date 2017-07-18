using UnityEngine;
using UnityEngine.VR.WSA.WebCam;
using System.Collections;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using RGBDCapturer;

[ExecuteInEditMode]
//[RequireComponent(typeof(PhotoCaptureController))]
public class DepthCaptureController : MonoBehaviour
{
    [Range(0f, 3f)]
    public float depthLevel = 0.5f;

    [SerializeField]
    public GameObject quad;
    public RenderTexture tex;
    public Shader shader;

    private Camera cam;
    //private bool isTestMode = false;
    private bool isTestMode = true;
    private int frameIndex = 1;

    private string imageName;

    private Material _material;
    private Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }
    

    private void Start()
    {
        /// <summary>
        /// Prepare for depthbuffer capturing
        /// </summary>
        cam = GetComponent<Camera>();
        //tex = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 32);
        //Debug.Log(string.Format("{0} x {1}", Camera.main.pixelWidth, Camera.main.pixelHeight));
        //tex = new RenderTexture(720, 504, 32);

        /// <summary>
        /// Usually cameras render directly to screen, but for some effects it is useful 
        /// to make a camera render into a texture. This is done by creating a RenderTexture 
        /// object and setting it as targetTexture on the camera. The camera will then render 
        /// into that texture.
        /// </summary>
        //cam.targetTexture = tex;
        cam.SetTargetBuffers(tex.colorBuffer, tex.depthBuffer);
        if (!SystemInfo.supportsImageEffects)
        {
            print("System doesn't support image effects");
            enabled = false;
            return;
        }
        if (shader == null || !shader.isSupported)
        {
            enabled = false;
            print("Shader " + shader.name + " is not supported");
            return;
        }

        // turn on depth rendering for the camera so that the shader can access it via _CameraDepthTexture
        cam.depthTextureMode = DepthTextureMode.Depth;

        /// <summary>
        /// For debugging purpose, don't save but display z-buffer.
        /// </summary>
        if (isTestMode)
        {
            //DisplayAspectRatioControl darc;
            //darc = quad.GetComponent<DisplayAspectRatioControl>();
            //Texture2D t = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            //darc.SetDisplay(t);
        }
        else
        {
        }

    }





    /// <summary>
    /// Dispatch asynchronous RGB capture and then save depth synchronously.
    /// </summary>
    /// <param name="t"></param>
    private void SaveDepthAndRGB(Texture2D t)
    {
        Debug.Log("@SaveDepth");
        byte[] bytes = t.EncodeToJPG();
        string imageName = string.Format(@"Hololens-{0}", System.DateTime.Now.ToFileTime());


        string imagePath = Helper.getImageWritePath(imageName, "depth");
        FileStream file = File.Open(imagePath, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Dispose();
        /*
         * Usually cameras render directly to screen, but for some effects it is useful to 
         * make a camera render into a texture. This is done by creating a RenderTexture 
         * object and setting it as targetTexture on the camera. The camera will then render 
         * into that texture.
         */
        //RenderTexture.active = null;
        Debug.Log("@SaveDepth exits");

    }



    /// <summary>
    /// Get z-buffer from a secondary camera and return it as a Texture2D
    /// </summary>
    /// <returns></returns>
    private Texture2D captureDepthBufferAsTexture2D()
    {
        material.SetFloat("_DepthLevel", depthLevel);

        //Debug.Log(Camera.main.depthTextureMode);

        // Render textures are textures that can be rendered to
        RenderTexture.active = tex;

        /* Render the camera manually.
         * This will render the camera. It will use the camera's clear flags, target 
         * texture and all other settings.
         * The camera will send OnPreCull, OnPreRender and OnPostRender to any scripts 
         * attached, and render any eventual image filters.
         * This is used for taking precise control of render order. To make use of this 
         * feature, create a camera and disable it. Then call Render on it.
         */
        cam.Render();
        /* Copies source texture into destination render texture with a shader.
         * This is mostly used for implementing post-processing effects.
         */
        Graphics.Blit(tex, tex, material);
        Texture2D t = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        // Read pixels from screen into the saved texture data.
        t.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

        // Actually apply all previous SetPixel and SetPixels changes.
        t.Apply();    // Unnecessary?
        return t;
    }

    private void Update()
    {
        //if (!isTestMode && isTimeToCaptureDepthAfterRGB)
        //{

        //}
        //if (_photoCap == null)
        //{
        //    Debug.Log("_photoCap is null");
        //}

        if (++frameIndex % 20 == 0 && !MainController.isCapturing)
        {
            if (isTestMode)
            {
                Texture2D t = captureDepthBufferAsTexture2D();
                quad.GetComponent<MeshRenderer>().sharedMaterial.mainTexture = t;
            }
            else
            {
                MainController.isCapturing = true;

                /// Unity editor can neither capture rgb nor save to disk.
#if UNITY_WSA && !UNITY_EDITOR
                Texture2D t = captureDepthBufferAsTexture2D();
                SaveDepthAndRGB(t);
#endif
            }
            RenderTexture.active = null;

        }

        
    }


    private void OnDisable()
    {
        if (_material != null)
            DestroyImmediate(_material);
    }

    //void Awake()
    //{
    //    DontDestroyOnLoad(transform.gameObject);
    //}

}
