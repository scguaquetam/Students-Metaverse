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
    [Header("scene")]
    public GameObject[] doors;
    private void Awake() {
        instance=this;
    }
    private void Start() {
        //PurchaseScreen.instance.LoadInfoStage(0);
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
        tokenPanel.SetActive(false);
    }
    public void CloseScreenNFT()
    {
        starterAssetsInputs.cursorLocked = true;
        starterAssetsInputs.cursorInputForLook = true;
        firstPersonController.enabled = true;
        characterObject.position = characterInitialPos.position;
        nftToken.SetActive(false);
    }
    public void HideDoors()
    {
        foreach(GameObject g in doors) {g.SetActive(false);}
    }
}
