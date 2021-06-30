//Entreprise: Solution & Co
//Date de creation: juin 2021

using UnityEngine;
using System.IO;

/*Script gestion de SolutionVr
 * Ce script contient :
 * - fonction d'initialisation des outils
 *      -Interactions
 *      -Teleportation
 * -Fonctions de gestions (mode vol, tablette, etc..)
*/

public class SVRManager : MonoBehaviour
{
    [Header("Configuration")]
    public string SessionPath = "1-SRV-Images";
    public bool UserCanUseConfig;
    
    [Header("WandCube")]
    [SerializeField] private Mesh wandCubeMesh = null;
    [SerializeField] private Material wandMat = null;

    [Header("SceneOptions")]
    public bool flyingMode;
    private SVRNavigationController srvNavController;

    private void Awake()
    {
        // Creation d'un fichier qui stockera les photos prise lors de la session
        if (!Directory.Exists(Application.dataPath + "/../" + SessionPath))
        {
            Directory.CreateDirectory(Application.dataPath + "/../" + SessionPath);
        }

        // On attribue notre Menu personnaliser au component de MiddleVR
        GetComponentInChildren<SVRMenuManager>().WandCubeMesh = wandCubeMesh;
        GetComponentInChildren<SVRMenuManager>().WandMat = wandMat;
        Destroy(GetComponentInChildren<VRMenuManager>());
    }

    private void Start()
    {
        srvNavController = GameObject.Find("PlayerBody").GetComponent<SVRNavigationController>();
    }

    private void Update()
    {
        if (flyingMode)
        {
            srvNavController.GravityMultiplier = 0f;
            srvNavController.isFlying = true;
        }
        else
        {
            srvNavController.GravityMultiplier = 9.81f;
            srvNavController.isFlying = false;
        }

        GetComponentInChildren<VRManagerScript>().Fly = flyingMode;
    }
}


