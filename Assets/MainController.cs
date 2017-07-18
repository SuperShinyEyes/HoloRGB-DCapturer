using RGBDCapturer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGBDCapturer
{
    public class MainController : Singleton<MainController>
    {

        private PlayModeEnum _playMode = PlayModeEnum.ObservationMode;
        public PlayModeEnum PlayMode
        {
            get { return _playMode;  }
            set
            {
                _playMode = value;

                var spatialMapping = GameObject.FindGameObjectWithTag("SpatialMapping");
                var cam = GetComponent<Camera>();
                frameIndex = 1;

                if (value.Equals(PlayModeEnum.RecordMode))
                {
                    cam.cullingMask = ~(1 << 8);
                    PhotoCaptureController.Instance.createPhotoCapture();
                } else
                {
                    cam.cullingMask |= (1 << 8);
                    PhotoCaptureController.Instance.stopPhotoCapture();
                }
            }
        }

        [SerializeField]
        private ViewModeEnum _viewMode;
        //[Tooltip("Type of the animation parameter to modify.")]
        public ViewModeEnum ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                var spatialMappingManager = HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance;
                //var //spatialMappingPrefab = GameObject.FindGameObjectWithTag("SpatialMapping");

                if (value == ViewModeEnum.SaveRGBAndDepthButNoView)
                {
#if UNITY_WSA && !UNITY_EDITOR
                            //PhotoCaptureController.Instance.createPhotoCapture();
                            //spatialMappingPrefab.layer = 8;
#endif

                    Debug.Log("Toggle to SaveRGB!");
                }
                else
                {
#if UNITY_WSA && !UNITY_EDITOR
                            //PhotoCaptureController.Instance.stopPhotoCapture();
                            //spatialMappingPrefab.layer = 0;
#endif
                    Debug.Log("Toggle to wireframe");
                }
                spatialMappingManager.ToggleSurfaceMaterial(ViewMode);
            }
        }

        public static int CaptureIntervalInNumOfFrames = 20;

        private static int frameIndexBetweenRGBAndZBufferCapture = 0;

        internal int frameIndex = 0;

        private static bool _isCapturing = false;
        public static bool isCapturing
        {
            get
            {
                return _isCapturing;
            }
            set
            {
                _isCapturing = value;
                frameIndexBetweenRGBAndZBufferCapture = 0;
            }
        }


        // Use this for initialization
        void Start()
        {
            //InitializeSpatialMappingMaterial();

        }

        // Update is called once per frame
        void Update()
        {
            //UpdateFPS();
            frameIndex++;
        }

        /// <summary>
        /// 
        /// </summary>
        //private void InitializeSpatialMappingMaterial()
        //{
        //    Debug.Log("Set SpatialMapping mesh material");
        //    HoloToolkit.Unity.SpatialMapping.SpatialMappingManager.Instance.ToggleSurfaceMaterial(ViewMode);
        //}

        [HideInInspector]
        private int _fps = 0;
        private int fps
        {
            get
            {
                return _fps;
            }
            set
            {
                _fps = value;
                frames = 0;
            }
        }
        private int frames = 0;
        private int SecondsPassed = 0;
        void UpdateFPS()
        {
            // This is a super crude method of determining FPS.
            // Basically, we count how many frames we get each second.
            frames++;

            int currentSecond = (int)Time.time;
            if (currentSecond != SecondsPassed)
            {
                fps = frames;
                SecondsPassed = currentSecond;
                // Also show available speech commands.
                Debug.Log(string.Format("fps: {0}", fps));
            }
        }

        internal bool isTimeToCapture
        {
            get { return frameIndex % CaptureIntervalInNumOfFrames == 0; }
        }
    }
}