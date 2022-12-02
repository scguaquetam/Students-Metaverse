using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PurchaseScreen : MonoBehaviour
{
    public static PurchaseScreen instance;
    int stageIndex;
    public TMP_Text stageName, stageFeatures, stagePrice;
    public Button prev, next;
    public List<StageInfo> stages = new List<StageInfo>();
    public Image stageImg;
    public GameObject loadingPanel;
    bool hasNoNFT;
    [Header("error")]
    public TMP_Text errorTxt;
    public TMP_Text msgTxt;
    [Header ("my profile")]
    public Image myNft;
    private void Awake() {
        instance = this;
    }
    private void Start() {
        errorTxt.gameObject.SetActive(false);
        // prev.interactable = false;
        // next.interactable = false;
        // // testing /////==>
        // stageIndex = 0;
        //checkButtons();
        // /////==>
        
    }
    public void checkButtons()
    {
        if(stageIndex == 0)
        {
            prev.interactable = false;
            next.interactable = true;
        }
        else if (stageIndex == stages.Count-1)
        {
            next.interactable = false;
            prev.interactable = true;
        }
        else 
        {
            next.interactable = true;
            prev.interactable = true;
        }
    }
    public void OnNextBtn()
    {
        if(stageIndex != stages.Count-1)
        {
            stageIndex ++;
            LoadInfoStage(stageIndex);
        }
    }
    public void OnPrevBtn()
    {
        if(stageIndex != 0)
        {
            stageIndex --;
            LoadInfoStage(stageIndex);
        }
    }
    public void ShowError(string _error)
    {
        msgTxt.gameObject.SetActive(false);
        errorTxt.text = _error;
        errorTxt.gameObject.SetActive(true);
    }
    public void ShowAnswer(string _msg)
    {
        errorTxt.gameObject.SetActive(false);
        msgTxt.text = _msg;
        msgTxt.gameObject.SetActive(true);
    }
    public void SetNFTImage(Sprite _sprite)
    {
        myNft.sprite = _sprite;
    }
    public void LoadInfoStage(int _index, bool _hasNoNFT = false)
    {
        stageIndex = _index;
        checkButtons();
        stageName.text = stages[stageIndex].stageName;
        stageFeatures.text = stages[stageIndex].stageFeatures;
        stagePrice.text = stages[stageIndex].price;
        stageImg.sprite = stages[stageIndex].stageImage;
        loadingPanel.SetActive(false);
        if(_hasNoNFT)
        {
            hasNoNFT = _hasNoNFT;
        }
    }
    public void OnPurchaseButton()
    {
        loadingPanel.SetActive(true);
        BNBInteraction.instance.OnPurchaseButton(stageIndex, stages[stageIndex].price, hasNoNFT);
    }
    public Sprite GetSprite(int _index)
    {
        return stages[_index].stageImage;
    }
}
[Serializable]
public class StageInfo 
{
    public string stageName, stageFeatures, price;
    public Sprite stageImage;
    StageInfo() {}
}

