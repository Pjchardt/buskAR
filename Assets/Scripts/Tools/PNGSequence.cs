using UnityEngine;
using System.Collections;
using System.IO;

public class PNGSequence : MonoBehaviour
{
    public static PNGSequence Instance;

    Camera thisCamera;
    public RenderTexture targetTexture;

    bool finished = false;
    int num = 0;

    void Awake()
    {
        Instance = this;
        thisCamera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        if (!finished)
        {
            TakeScreenshot();
            num++;
            if (num > 60 * 4)
                finished = true;
        }
    }

    /*void OnPostRender()
    {
        Texture2D screenshot = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(Rect.MinMaxRect(0, 0, screenshot.width, screenshot.height), 0, 0, false);

        byte[] bytes = screenshot.EncodeToPNG();
        Object.Destroy(screenshot);

        File.WriteAllBytes(Application.dataPath + "/Screenshot" + "1" + ".png", bytes);

        thisCamera.targetTexture = null;
    }*/

    void TakeScreenshot()
    {
        thisCamera.targetTexture = targetTexture;
        thisCamera.Render();
        RenderTexture.active = targetTexture;

        Texture2D screenshot = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(Rect.MinMaxRect(0, 0, screenshot.width, screenshot.height), 0, 0, false);

        byte[] bytes = screenshot.EncodeToPNG();
        Object.Destroy(screenshot);

        File.WriteAllBytes(Application.dataPath + "/Screenshot" + num.ToString() + ".png", bytes);
       
        thisCamera.targetTexture = null;
    }
}
	

