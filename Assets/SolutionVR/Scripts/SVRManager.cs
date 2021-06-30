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
    private VRManagerScript _vrManagerScript;
    
    [Header("Configuration")]
    public string sessionPath = "1-SRV-Images";
    public bool userCanUseConfig;
    
    [Header("WandCube")]
    [SerializeField] private Mesh wandCubeMesh = null;
    [SerializeField] private Material wandMat = null;

    [Header("SceneOptions")]
    public bool flyingMode;
    private SVRNavigationController _srvNavController;

    private void Awake()
    {
        // Creation d'un fichier qui stockera les photos prise lors de la session
        if (!Directory.Exists(Application.dataPath + "/../" + sessionPath))
            Directory.CreateDirectory(Application.dataPath + "/../" + sessionPath);

        // On attribue notre Menu personnaliser au component de MiddleVR
        GetComponentInChildren<SVRMenuManager>().WandCubeMesh = wandCubeMesh;
        GetComponentInChildren<SVRMenuManager>().WandMat = wandMat;
        
        _srvNavController = GameObject.Find("PlayerBody").GetComponent<SVRNavigationController>();
        _vrManagerScript = GetComponentInChildren<VRManagerScript>();
    }

    private void Update()
    {
        if (flyingMode)
        {
            _srvNavController.GravityMultiplier = 0f;
            _srvNavController.isFlying = true;
        }
        else
        {
            _srvNavController.GravityMultiplier = 9.81f;
            _srvNavController.isFlying = false;
        }

        _vrManagerScript.Fly = flyingMode;
    }
}


