using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AvatarInteraction : MonoBehaviour
{
    public GameObject getTokenPanel, buyLandPanel;
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "token")
        {
            if (!getTokenPanel.activeSelf)
            {
                getTokenPanel.SetActive(true);
                BNBInteraction.instance.ReadBalance();
            }
        } 
        else if (gameObject.tag == "land")
        {
            if(!buyLandPanel.activeSelf)
            {
                buyLandPanel.SetActive(true);
                TokenScreen.instance.OnLandPanel();
                BNBInteraction.instance.BuyLand(0);
            }
        }
    }
}
