//-----------------------------------------------------------------------
// Based on "HelloARController.cs". 
//-----------------------------------------------------------------------

using GoogleARCore;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls for viewer.
/// </summary>
public class ARCore_ViewerController : MonoBehaviour
{
    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera FirstPersonCamera;

    /// <summary>
    /// Prefab for where music viz will be placed in the world
    /// </summary>
    public GameObject PlacementVisualizationPrefab;
    private GameObject placementVisualizationObject;

    /// <summary>
    /// Music visualization prefab to place in the world
    /// </summary>
    public GameObject MusicVisualizationPrefab;
    private GameObject musicVisualizationObject;

    public Button PlaceVisualizationButton;
    public Button RemoveVisualizationButton;

    /// <summary>
    /// Variables for on hover events
    /// </summary>
    public SingleUnityLayer InteractableLayer;
    InteractableObject hoverObject = null;
    InteractableObject prevObject = null;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;

    /// <summary>
    /// Set to true when viewer places music visualization in the world
    /// </summary>
    private bool visualizationInitialized = false;

    private void Start()
    {
        PlaceVisualizationButton.onClick.AddListener(OnButtonPressed_SetVisualization);
        RemoveVisualizationButton.onClick.AddListener(OnButtonPressed_RemoveVisualization);
        RemoveVisualizationButton.gameObject.SetActive(false);
        placementVisualizationObject = Instantiate(PlacementVisualizationPrefab);
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        _QuitOnConnectionErrors();

        // Check that motion tracking is tracking.
        if (Frame.TrackingState != TrackingState.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (!visualizationInitialized)
        {
            Vector3 target = FirstPersonCamera.transform.position + FirstPersonCamera.transform.forward * .5f;
            placementVisualizationObject.transform.position = Vector3.Lerp(placementVisualizationObject.transform.position, target, Time.deltaTime * 4f);
            return;
        }

        if (Input.touchCount > 0)
        {
            foreach (Touch t in Input.touches)
            {
                if (t.phase == TouchPhase.Began)
                {
                    Ray ray = FirstPersonCamera.ScreenPointToRay(t.position);
                    RaycastHit hit;

                    if (Physics.SphereCast(ray, .025f, out hit, Mathf.Infinity, InteractableLayer.Mask))
                    {
                        InteractableObject temp = hit.collider.gameObject.GetComponent<InteractableObject>();

                        if (temp != null)
                            temp.OnClick();
                    }
                }
            }
        }
    }

    public void OnButtonPressed_SetVisualization()
    {
        musicVisualizationObject = Instantiate(MusicVisualizationPrefab);
        musicVisualizationObject.transform.position = placementVisualizationObject.transform.position;
        DataObject.Instance.ARObjectToTrack = musicVisualizationObject;
        placementVisualizationObject.SetActive(false);
        PlaceVisualizationButton.gameObject.SetActive(false);
        RemoveVisualizationButton.gameObject.SetActive(true);
        visualizationInitialized = true;
    }

    public void OnButtonPressed_RemoveVisualization()
    {
        Destroy(musicVisualizationObject);
        DataObject.Instance.ARObjectToTrack = null;
        placementVisualizationObject.SetActive(true);
        PlaceVisualizationButton.gameObject.SetActive(true);
        RemoveVisualizationButton.gameObject.SetActive(false);
        visualizationInitialized = false;
    }

    /// <summary>
    /// Quit the application if there was a connection error for the ARCore session.
    /// </summary>
    private void _QuitOnConnectionErrors()
    {
        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
        else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
