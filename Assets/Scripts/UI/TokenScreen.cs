using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TokenScreen : MonoBehaviour
{
    [Header("Token Transfer")]
    [SerializeField] TMP_Text currentBalance;
    [SerializeField] TMP_Text yourAddress;
    [Header("Land purchase")]
    [SerializeField] TMP_Text currentBalance2;
    [SerializeField] TMP_Text yourAddress2;
    public static TokenScreen instance;
    private void Awake()
    {
        instance = this;
        yourAddress.text = SessionHandler.instance?.getSessionAddress();
    }
    public void WriteInfo(string _balance)
    {
        currentBalance.text = _balance;
    }
    public void OnLandPanel()
    {
        currentBalance2.text = BNBInteraction.instance.GetBalance().ToString();
        yourAddress2.text = SessionHandler.instance.getSessionAddress();
    }
    public void WriteTx(string _tx)
    {
        yourAddress2.text = _tx;
    }
}
