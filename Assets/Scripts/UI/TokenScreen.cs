using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TokenScreen : MonoBehaviour
{
    [Header("Token Transfer")]
    [SerializeField] TMP_Text currentBalance;
    [SerializeField] TMP_Text yourAddress;
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
}
