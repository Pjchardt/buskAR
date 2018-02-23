using UnityEngine;
using System.Collections;
using System.IO;

public class MonoscopicScreencapture : MonoBehaviour
{
    public static MonoscopicScreencapture Instance;

    Camera thisCamera;
    public RenderTexture targetTexture;

    void Awake()
    {
        Instance = this;
    }

    void TakePhoto()
    {
        Debug.Log("Taking monoscopic photo");
        if (thisCamera == null)
            thisCamera = GetComponent<Camera>();
        thisCamera.targetTexture = targetTexture;
        float savedFOV = thisCamera.fieldOfView;
        thisCamera.fieldOfView = 60f;
        thisCamera.Render();
        RenderTexture.active = targetTexture;

        Texture2D screenshot = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(Rect.MinMaxRect(0, 0, screenshot.width, screenshot.height), 0, 0, false);

        byte[] bytes = screenshot.EncodeToPNG();
        Object.Destroy(screenshot);
        
#if UNITY_EDITOR
        File.WriteAllBytes("C:/RalphVR/VRExplorations/Media" + "/Screenshot" + System.DateTime.Now.ToFileTime() + ".png", bytes);
#else
        File.WriteAllBytes(Application.persistentDataPath + "/Screenshot" + System.DateTime.Now.ToFileTime() + ".png", bytes);
#endif

        thisCamera.fieldOfView = savedFOV;
        thisCamera.targetTexture = null;
    }

    public void TryToTakePhoto()
    {
            TakePhoto();
    }
}


