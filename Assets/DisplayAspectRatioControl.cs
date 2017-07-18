using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAspectRatioControl : MonoBehaviour {

    public float scale = 1.0f;

    public void SetDisplay(Texture2D tex)
    {
        Debug.Log("Set quad ratio");
        float aspectRatio = (float)tex.height / tex.width;
        Vector3 size = scale * (new Vector3(1, aspectRatio, 1));
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = tex;
        transform.localScale = size;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
