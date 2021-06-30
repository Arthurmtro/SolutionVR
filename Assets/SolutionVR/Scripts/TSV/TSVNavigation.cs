using UnityEngine;
using System.Collections;

// Cette fonction gere la teleportation vers
// les TSVplayerBody


public class TSVNavigation : MonoBehaviour
{
    [HideInInspector] public int tsvIndex;

    public void TeleportToView()
    {
        TSVFlag tsvFlag = FindObjectsOfType<TSVFlag>()[FindObjectsOfType<TSVFlag>().Length - 1 - tsvIndex];
        GameObject playerBody = GameObject.Find("PlayerBody");
        StartCoroutine(Teleport(tsvFlag, playerBody));
    }

    IEnumerator Teleport(TSVFlag tsvFlag, GameObject playerBody)
    {
        playerBody.GetComponent<CharacterController>().enabled = false;
        playerBody.GetComponent<SVRNavigationController>().enabled = false;
        yield return null;
        playerBody.transform.SetPositionAndRotation(tsvFlag.transform.position, tsvFlag.transform.rotation);
        yield return null;
        playerBody.GetComponent<CharacterController>().enabled = true;
        playerBody.GetComponent<SVRNavigationController>().enabled = true;
    }
}
