using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SVRMenu : MonoBehaviour
{
    [Header("General")]
    [HideInInspector] public SVRManager svrManager;
    [SerializeField] private GameObject[] canvasArray = null;

    [Header("Fly Config")]
    public Sprite[] flyModeImg;
    public Button flyButton;

    [Header("Tsv Config")]
    [SerializeField] private GameObject btnTemplate = null;
    [HideInInspector] public List<GameObject> tsvGameObjects = new List<GameObject>();
    private bool _tsvMode = false;

    [Header("Screenshot Camera Config")]
    [SerializeField] private GameObject screenCamPref = null;
    [SerializeField] private GameObject screenCamButton = null;
    private bool _screenCamEnabled = false;
    private GameObject _screenCamInst;

    [Header("Config Config")]
    [SerializeField] private GameObject configButton = null;
    [SerializeField] private GameObject configCanvas = null;
    private bool _showConfig;

    [Header("Config Config")]
    [SerializeField] private GameObject playerSpeedButton = null;
    [SerializeField] private GameObject playerSpeedCanvas = null;
    private bool _showPlayerSpeed;

    private Text _configCanvasText;
    private ConfigScript _configScript;

    private Text _playerSpeedCanvasText;
    private SVRNavigationController _svrNavigationController;

    private void Start()
    {
        svrManager = FindObjectOfType<SVRManager>();

        TSVInit();

        canvasArray[0].SetActive(true);
        canvasArray[1].SetActive(false);
        canvasArray[2].SetActive(false);
        canvasArray[3].SetActive(false);

        // Determine si le bouton tsv doit etre desactiver ou non
        if (tsvGameObjects.Count < 1)
        {
            GameObject.Find("TSVButton").GetComponent<Button>().interactable = false;
            GameObject.Find("TSVButton").GetComponentsInChildren<Image>()[1].color = Color.gray;
        }

        // Si l'user n'a pas le droit de changer de config le boutton est desactive
        if (!GameObject.Find("SolutionVRManager").GetComponent<SVRManager>().userCanUseConfig)
        {
            configButton.GetComponentsInChildren<Image>()[1].color = Color.gray;
            configButton.GetComponentInChildren<Text>().text = "Config disabled";
        }

        // Vide la liste car inutile desormais
        tsvGameObjects.Clear();

        _configCanvasText = configCanvas.GetComponentsInChildren<Text>()[1];
        _configScript = GameObject.Find("SolutionVRManager").GetComponent<ConfigScript>();

        _playerSpeedCanvasText = playerSpeedCanvas.GetComponentsInChildren<Text>()[1];
        _svrNavigationController = FindObjectOfType<SVRNavigationController>();
    }

    private void Update()
    {
        if (!svrManager)
            return;

        // Modification des textes
        if (_showConfig)
            _configCanvasText.text = _configScript.condition.ToString();

        if (_showPlayerSpeed)
            _playerSpeedCanvasText.text = "X " + _svrNavigationController.SpeedMultiplier;
    }

    // Toggle fly mode et changement du texte
    public void ToggleFlyMode()
    {
        svrManager.flyingMode = !svrManager.flyingMode;
        if (!svrManager.flyingMode)
        {
            flyButton.GetComponentInChildren<Text>().text = "Walking";
            flyButton.GetComponentsInChildren<Image>()[1].sprite = flyModeImg[0];
        }
        else
        {
            flyButton.GetComponentInChildren<Text>().text = "Flying";
            flyButton.GetComponentsInChildren<Image>()[1].sprite = flyModeImg[1];
        }
    }

    // Toggle le mode tsv preview pour pouvoir ce teleporter
    public void ToggleTSVMode()
    {
        _tsvMode = !_tsvMode;
        if(_tsvMode)
        {
            canvasArray[0].SetActive(false);
            canvasArray[1].SetActive(true);
        }
        else
        {
            canvasArray[0].SetActive(true);
            canvasArray[1].SetActive(false);
        }
    }

    // Active ou desactive la camera
    public void ToggleScreenshotCam()
    {
        _screenCamEnabled = !_screenCamEnabled;
        if (_screenCamEnabled)
        {
            _screenCamInst = Instantiate(screenCamPref);
            _screenCamInst.transform.SetPositionAndRotation(new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z) , transform.rotation);
            screenCamButton.GetComponentsInChildren<Image>()[1].color = Color.white;                
            screenCamButton.GetComponentInChildren<Text>().text = "Disable Camera";
        }
        else if(_screenCamInst != null)
        {
            Destroy(_screenCamInst);
            screenCamButton.GetComponentsInChildren<Image>()[1].color = Color.gray;
            screenCamButton.GetComponentInChildren<Text>().text = "Enable Camera";
        }
    }

    // Toggle l'affichage des configs
    public void ToggleConfigGUI()
    {

        _showConfig = !_showConfig;
        if (_showConfig)
        {
            canvasArray[2].SetActive(true);
            configButton.GetComponentsInChildren<Image>()[1].color = Color.white;
            configButton.GetComponentInChildren<Text>().text = "Hide Config";
        }
        else
        {
            canvasArray[2].SetActive(false);
            configButton.GetComponentsInChildren<Image>()[1].color = Color.gray;
            configButton.GetComponentInChildren<Text>().text = "Show Config";
        }
    }

    // Toggle l'affichage des infos du joueur (vitesse)
    public void TogglePlayerSpeedGUI()
    {

        _showPlayerSpeed = !_showPlayerSpeed;
        if (_showPlayerSpeed)
        {
            canvasArray[3].SetActive(true);
            playerSpeedButton.GetComponentsInChildren<Image>()[1].color = Color.white;
        }
        else
        {
            canvasArray[3].SetActive(false);
            playerSpeedButton.GetComponentsInChildren<Image>()[1].color = Color.gray;
        }
    }

    // Recharge la scene
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Cette fonction capture les camera des TSV
    // Et ajoute l'image correspondante dans le Menu
    private void TSVInit()
    {
        for (int i = 0; i < FindObjectsOfType<TSVFlag>().Length; i++)
        {
            tsvGameObjects.Add(FindObjectsOfType<TSVFlag>()[i].gameObject);
        }

        if (tsvGameObjects == null)
            return;

        int y = 0;
        for (int i = 0; i < tsvGameObjects.Count; i++)
        {
            // On prend le screenshot de la camera du tsv[i]
            int mWidth = 1280, mHeight = 720;
            Rect rect = new Rect(0, 0, mWidth, mHeight);
            RenderTexture renderTexture = new RenderTexture(mWidth, mHeight, 24);
            Texture2D screenShot = new Texture2D(mWidth, mHeight, TextureFormat.RGBA32, false);

            tsvGameObjects[i].GetComponent<Camera>().targetTexture = renderTexture;
            tsvGameObjects[i].GetComponent<Camera>().Render();

            RenderTexture.active = renderTexture;
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            tsvGameObjects[i].GetComponent<Camera>().targetTexture = null;
            RenderTexture.active = null;

            Destroy(renderTexture);

            // Instanciation de l'objet tsvPreview sur le menu
            GameObject tsvPreview = Instantiate(btnTemplate, GameObject.Find("TSVContainer").transform);

            // On definit l'emplacement de l'image et l'on modifie de maniere dynamique le container
            if (i == 0)
            {
                GameObject.Find("TSVContainer").GetComponent<RectTransform>().sizeDelta = new Vector2(0, y*75);
                tsvPreview.GetComponent<RectTransform>().localPosition = new Vector3(50, 75, -0.501f);
                y++;
            }

            if (i % 2 == 0 && i > 0)
            {
                GameObject.Find("TSVContainer").GetComponent<RectTransform>().sizeDelta = new Vector2(0,y*75);
                tsvPreview.GetComponent<RectTransform>().localPosition = new Vector3(50, i + 75, -0.501f);
                y++;
            }
            if(i % 2 != 0 && i > 0)
                tsvPreview.GetComponent<RectTransform>().localPosition = new Vector3(130, (i - 1) + 75, -0.501f);

            //Attribution des valeurs dans les scripts de notre tsvPreview
            tsvPreview.GetComponent<TSVNavigation>().tsvIndex = i;
            tsvPreview.GetComponentInChildren<RawImage>().texture = screenShot;
            if(tsvGameObjects[i].GetComponent<TSVFlag>().locationName.Length > 0)
                tsvPreview.GetComponentInChildren<Text>().text = tsvGameObjects[i].GetComponent<TSVFlag>().locationName;
            else
                tsvPreview.GetComponentInChildren<Text>().text = "Tsv n° " + i;
        }
        //On agrandit le content de la scrollbar afin que tous les tsv entrent dedans
        GameObject.Find("TSVContainer").GetComponent<RectTransform>().sizeDelta += new Vector2(0, 150);
    }
}
