using UnityEngine;
using UnityEngine.SceneManagement;

// Ce script est un model qui permet d'avoir plusieurs
// configurations via un Switch case
public class ConfigScript : MonoBehaviour
{
    public int condition;
    [SerializeField] private KeyCode decrementerValeur = KeyCode.Alpha1;
    [SerializeField] private KeyCode incrementerValeur = KeyCode.Alpha2;

    void Start()
    {
        condition = 0;
    }

    void Update()
    {
        GetInputs();

        switch(condition)
        {
            case 1:
                //Script
                break;
            case 2:
                if(SceneManager.GetActiveScene().name != "Scene2")
                    SceneManager.LoadSceneAsync("Scene2");
                else
                    SceneManager.LoadSceneAsync(0);
                break;
            default:
                // Ne s'effectue que si la condition n'active aucun des 'case'
                condition = 1;
                break;
        }
    }

    // Cette fonction gère ce qui ce passe apres avoir
    // pressé l'une des touches mises en paramètres
    private void GetInputs()
    {
        if(Input.GetKeyDown(incrementerValeur))
            condition++;
        if (Input.GetKeyDown(decrementerValeur))
            condition--;

    }

    // Fonctions liee a l'interface de config
    public void IncrementVal()
    {
        condition++;
    }
    public void DecrementVal()
    {
        condition--;
    }
}
