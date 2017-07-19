using RGBDCapturer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.SpatialMapping
{
    public static class SpatialMappingHelper
    {

        public static void ToggleSurfaceMaterial(ViewModeEnum mode)
        {
            Material m;
            if (mode.Equals(ViewModeEnum.WireframeView))
            {
                //m = (Material)Resources.Load("SpatialMappingWireframe", typeof(Material));
                m = Resources.Load<Material>("Wireframe");
            }
            else
            {
                //m = (Material)Resources.Load("Default-Diffuse", typeof(Material));
                m = Resources.Load<Material>("defaultMat");
            }

            if (m != null)
            {
                Debug.Log(string.Format("Material: {0}", m.name));
                SpatialMappingManager.Instance.SetSurfaceMaterial(m);
            }
            else { Debug.Log("Cannot find material for meshes"); }

        }
    }
}