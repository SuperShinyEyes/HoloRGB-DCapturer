using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RGBDCapturer
{

    public enum PlayModeEnum
    {
        RecordMode,
        ObservationMode
    }

    /// <summary>
    /// Enum for wireframe mesh materials
    /// </summary>
    public enum ViewModeEnum
    {
        WireframeView,
        SaveRGBAndDepthButNoView
    }

    public static class Helper
    {

        /// <summary>
        /// Helper method for SaveDepthAndRGB()
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="flag"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        public static string getImageWritePath(string imageName, string flag, string imageFormat = ".jpg")
        {
            return Path.Combine(Application.persistentDataPath, imageName + "-" + flag + imageFormat);

        }
    }

    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                //if (instance == null)
                //{
                //    instance = (T) this;
                //}

                return instance;
            }
        }

        /// <summary>
        /// Returns whether the instance has been initialized or not.
        /// </summary>
        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        /// <summary>
        /// Base awake method that sets the singleton's unique instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}", GetType().Name);
            }
            else
            {
                instance = (T)this;
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}