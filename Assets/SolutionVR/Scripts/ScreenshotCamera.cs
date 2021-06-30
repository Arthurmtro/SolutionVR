using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ScreenshotCamera : MonoBehaviour
{
    [SerializeField] private Text Text = null;
    [SerializeField] private RenderTexture targetTexture = null;
    [SerializeField] private int mWidth = 1920, mHeight = 1080;
    private string sessionPath;
    private bool CooldownComplete;

    void Start()
    {
        sessionPath = GameObject.Find("SolutionVRManager").GetComponent<SVRManager>().SessionPath;
        CooldownComplete = true;
    }

    void Update()
    {
        if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(1))
            TakeScreenshot();
    }
    public string ScreenShotName()
    {
        // Creation d'un fichier qui stockera les photos prise lors de la session
        if (!Directory.Exists(Application.dataPath + "/../" + sessionPath + "/screenshots/" + System.DateTime.Now.ToString("dd-MM-yyyy")))
        {
            Directory.CreateDirectory(Application.dataPath + "/../" + sessionPath + "/screenshots/" + System.DateTime.Now.ToString("dd-MM-yyyy"));
        }

        return string.Format("{0}/screenshots/" + System.DateTime.Now.ToString("dd-MM-yyyy") + "/screen_{1}x{2}_{3}.png",
                             Application.dataPath + "/../" + sessionPath,
                             mWidth, mHeight,
                             System.DateTime.Now.ToString("HH-mm-ss"));
    }

    private void TakeScreenshot()
    {
        if (!CooldownComplete)
            return;

        Rect rect = new Rect(0, 0, mWidth, mHeight);
        RenderTexture renderTexture = new RenderTexture(mWidth, mHeight, 24);
        Texture2D screenShot = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);

        GetComponent<Camera>().targetTexture = renderTexture;
        GetComponent<Camera>().Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        GetComponent<Camera>().targetTexture = targetTexture;
        RenderTexture.active = null;

        Destroy(renderTexture);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName();
        File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));

        StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        CooldownComplete = false;
        Text.text = "Screenshot taken, now wait";
        yield return new WaitForSeconds(1.5f);
        Text.text = "Press the first left button";
        CooldownComplete = true;
    }
}
