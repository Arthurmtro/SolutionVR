using UnityEngine;
using System.Collections.Generic;


/*Script de gestion des tsv de SolutionVr
 * Ce script ce gere pendant la creation du projet
 * dans l'editeur de Unity
 * Il recupere les TSV et le gere afin de pouvoir
 * les attribuer au menu de SVR
*/

[ExecuteInEditMode]
public class TSVManager : MonoBehaviour
{
    [Header("Informations sur les TSV")]
    [SerializeField] private GameObject tsvPrefab = null;
    [SerializeField] int nbrTsv = 0;
    [HideInInspector] public List<GameObject> tsvGameObjects = new List<GameObject>();

    void Update()
    {
        tsvGameObjects.Clear();
        for (int i = 0; i< FindObjectsOfType<TSVFlag>().Length; i++)
        {
            tsvGameObjects.Add(FindObjectsOfType<TSVFlag>()[i].gameObject);
        }

        while (nbrTsv > FindObjectsOfType<TSVFlag>().Length)
        {
            GameObject tsv = Instantiate(tsvPrefab, transform);
            tsvGameObjects.Add(tsv);
        }

        while (nbrTsv < FindObjectsOfType<TSVFlag>().Length)
        {
            DestroyImmediate(tsvGameObjects[tsvGameObjects.Count - 1]);
            tsvGameObjects.RemoveAt(tsvGameObjects.Count - 1 );
        }

        if (FindObjectsOfType<TSVFlag>().Length < 1)
            return;

        for (int i = 0; i > tsvGameObjects.Count; i++)
        {
            if (tsvGameObjects[i] == null)
                tsvGameObjects.RemoveAt(i);
        }
    }
}
