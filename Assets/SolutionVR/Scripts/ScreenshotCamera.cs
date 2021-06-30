using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;

public class ScreenshotCamera : MonoBehaviour
{
    [SerializeField] private RenderTexture targetTexture = null;
    [SerializeField] private int mWidth = 1920, mHeight = 1080;
    [SerializeField] private uint wandButtonIndex = 1;
    private Text _cameraText = null;
    private string _sessionPath;
    private bool _cooldownComplete;
    private Camera _cam;

    private void Start()
    {
        _sessionPath = GameObject.Find("SolutionVRManager").GetComponent<SVRManager>().sessionPath;
        _cooldownComplete = true;
        _cam = GetComponent<Camera>();
        _cameraText = transform.GetChild(0).gameObject.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (!_cooldownComplete)
            return;
        
        if (MiddleVR.VRDeviceMgr.IsWandButtonToggled(wandButtonIndex))
            TakeScreenshot();
    }
    private string ScreenShotName()
    {
        // Creation d'un fichier qui stockera les photos prise lors de la session
        if (!Directory.Exists(Application.dataPath + "/../" +_sessionPath + "/screenshots/" + System.DateTime.Now.ToString("dd-MM-yyyy")))
        {
            Directory.CreateDirectory(Application.dataPath + "/../" +_sessionPath + "/screenshots/" + System.DateTime.Now.ToString("dd-MM-yyyy"));
        }

        return string.Format("{0}/screenshots/" + System.DateTime.Now.ToString("dd-MM-yyyy") + "/screen_{1}x{2}_{3}.png",
                             Application.dataPath + "/../" +_sessionPath,
                             mWidth, mHeight,
                             System.DateTime.Now.ToString("HH-mm-ss"));
    }

    private void TakeScreenshot()
    {

        Rect rect = new Rect(0, 0, mWidth, mHeight);
        RenderTexture renderTexture = new RenderTexture(mWidth, mHeight, 24);
        Texture2D screenShot = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);

        _cam.targetTexture = renderTexture;
        _cam.Render();

        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();

        _cam.targetTexture = targetTexture;
        RenderTexture.active = null;

        Destroy(renderTexture);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName();
        File.WriteAllBytes(filename, bytes);
        // Debug.Log(string.Format("Took screenshot to: {0}", filename));

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        _cooldownComplete = false;
        _cameraText.text = "Screenshot taken, now wait";
        yield return new WaitForSeconds(1.5f);
        _cameraText.text = "Press the first left button";
        _cooldownComplete = true;
    }
}
