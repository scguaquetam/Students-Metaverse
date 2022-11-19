using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AvatarInteraction : MonoBehaviour
{
    public GameObject getTokenPanel;
    private void OnTriggerEnter(Collider other) {
        if (!getTokenPanel.activeSelf){
            getTokenPanel.SetActive(true);
            BNBInteraction.instance.ReadBalance();
        }
    }
}
