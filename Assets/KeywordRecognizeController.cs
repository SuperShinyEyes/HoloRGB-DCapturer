using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using RGBDCapturer;

public class KeywordRecognizeController : MonoBehaviour {

    /// <summary>
    /// The depth data created from spatial mapping meshes are not so easily recognizable.
    /// That results in poor UX and we need to provide better scanning experience.
    /// Let's provide two viewing options:
    ///     1. Display spatialmapping meshes only. No depth buffer texture generation.
    ///     2. Don't show anything. Save depth buffer and RGB
    ///     
    /// Change viewing options with voice commands.
    /// </summary>

    /// <summary>
    /// Used for voice commands.
    /// </summary>
    private KeywordRecognizer keywordRecognizer;

    /// <summary>
    /// Use keyboard instead of voice command  when debugging on Unity.
    /// </summary>
    [SerializeField]
    public KeyCode RemoteMappingKey = KeyCode.T;
    public KeyCode ToggleGraphicsKey = KeyCode.G;

    [Tooltip("Keyword for toggling viewing options")]
    [SerializeField]
    public string ToggleKeyword = "Change material";
    public string ToggleGraphicsKeyword = "Toggle graphics";

    /// <summary>
    /// Collection of supported keywords and their associated actions.
    /// </summary>
    private Dictionary<string, System.Action> keywordCollection;


    /// <summary>
    /// Called by keywordRecognizer when a phrase we registered for is heard.
    /// </summary>
    /// <param name="args">Information about the recognition event.</param>
    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        Debug.Log("KeywordRecognizer_OnPhraseRecognized");
        if (keywordCollection.TryGetValue(args.text, out keywordAction))
        {
            Debug.Log("Invoke!");
            keywordAction.Invoke();
        }
    }


    // Use this for initialization
    void Start () {
        SetSpeechRecognizer();

    }

    // Update is called once per frame
    void Update () {
#if UNITY_EDITOR || UNITY_STANDALONE
        // Use the 'network' sourced mesh.  
        if (Input.GetKeyUp(RemoteMappingKey))
        {
            ToggleViewingOptions();
        }
        else if (Input.GetKeyUp(ToggleGraphicsKey))
        {
            ToggleGraphics();
        }
#endif

        }

    private void SetSpeechRecognizer()
    {
        Debug.Log("SetSpeechRecognizer()");
        // Create our keyword collection.
        keywordCollection = new Dictionary<string, System.Action>();
        keywordCollection.Add(ToggleKeyword, () => ToggleViewingOptions());
        keywordCollection.Add(ToggleGraphicsKeyword , () => ToggleGraphics());

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing.
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

    }

    /// <summary>
    /// Switch between situation checking and recording modes
    /// </summary>
    private void ToggleGraphics()
    {
        if(MainController.Instance.PlayMode.Equals(PlayModeEnum.ObservationMode))
        {
            MainController.Instance.PlayMode = PlayModeEnum.RecordMode;
        } else
        {
            MainController.Instance.PlayMode = PlayModeEnum.ObservationMode;
        }
    }

    private void ToggleViewingOptions()
    {

        //var spatialMapping = GameObject.FindGameObjectWithTag("SpatialMapping");
        if (MainController.Instance.ViewMode == ViewModeEnum.WireframeView)
        {
            MainController.Instance.ViewMode = ViewModeEnum.SaveRGBAndDepthButNoView;
            //#if UNITY_WSA && !UNITY_EDITOR
            //            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
            //#endif

            //            Debug.Log("Toggle to SaveRGB!");
        }
        else
        {
            MainController.Instance.ViewMode = ViewModeEnum.WireframeView;
            ////spatialMapping.layer = 0;
            //Debug.Log("Toggle to wireframe");
        }

    }
}
