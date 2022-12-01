using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;
    public StarterAssetsInputs starterAssetsInputs;
    public FirstPersonController firstPersonController;
    public GameObject tokenPanel, nftToken;
    public Transform characterObject, characterInitialPos;
    private void Awake() {
        instance=this;
    }
    public void OnOpenScreen()
    {
        starterAssetsInputs.cursorLocked = false;
        starterAssetsInputs.cursorInputForLook = false;
        firstPersonController.enabled = false;
    }
    public void CloseScreenToken()
    {
        starterAssetsInputs.cursorLocked = true;
        starterAssetsInputs.cursorInputForLook = true;
        firstPersonController.enabled = true;
        characterObject.position = characterInitialPos.position;
        tokenPanel.gameObject.SetActive(false);
    }
}
