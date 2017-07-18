using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VR.WSA.WebCam;

namespace RGBDCapturer
{
    public class PhotoCaptureController : Singleton<PhotoCaptureController>
    //public class PhotoCaptureController: MonoBehaviour
    {

        private PhotoCapture photoCap;
        private int frameIndexBetweenRGBAndZBufferCapture = 0;

        //protected override void Awake()
        //{
        //    base.Awake();

        //}

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (MainController.Instance.PlayMode.Equals(PlayModeEnum.RecordMode) && MainController.Instance.isTimeToCapture )
            {
                TakeScreenshot();
            }

        }

        /// <summary>
        /// Prepare for RGB capture.
        /// Resolution is set to lowest for performance issue.
        /// </summary>
        /// <param name="captureObject"></param>
        void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            photoCap = captureObject;
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();
            CameraParameters c = new CameraParameters();
            c.hologramOpacity = 0.0f;
            c.cameraResolutionWidth = cameraResolution.width;
            c.cameraResolutionHeight = cameraResolution.height;
            c.pixelFormat = CapturePixelFormat.BGRA32;
            captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
        }

        /// <summary>
        /// Dummy callback.
        /// </summary>
        /// <param name="result"></param>
        private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
        {
            if (!result.success)
            {
                Debug.Log("Unable to save photo to disk");
            }
        }


        /// <summary>
        /// Take RGB screenshot from HoloLens forehead camera
        /// </summary>
        /// <param name="imageName"></param>
        internal void TakeScreenshot()
        {
            string imageName = string.Format(@"Hololens-{0}", System.DateTime.Now.ToFileTime());

            string imagePath = Helper.getImageWritePath(imageName, "color");
            photoCap.TakePhotoAsync(
                imagePath,
                PhotoCaptureFileOutputFormat.JPG,
                onCapturedPhotoToDisk
                );
            //photoCap.TakePhotoAsync(
            //    onCapturedPhotoToMemory
            //    );
        }

        void onCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
        {
            Debug.Log(string.Format("Saved Photo to memory! It took {0} frames.", frameIndexBetweenRGBAndZBufferCapture));
            MainController.isCapturing = false;
        }

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            photoCap.Dispose();
            photoCap = null;
        }


        void onCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log(string.Format("Saved Photo to disk! It took {0} frames.", frameIndexBetweenRGBAndZBufferCapture));
            }
            else
            {
                Debug.Log("Failed to save Photo to disk");
            }
            MainController.isCapturing = false;
        }

        internal void createPhotoCapture()
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }

        internal void stopPhotoCapture()
        {
            photoCap.StopPhotoModeAsync(OnStoppedPhotoMode);
        }


    }
}