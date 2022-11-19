using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TokenScreen : MonoBehaviour
{
    [SerializeField] TMP_Text currentBalance, yourAddress;
    public static TokenScreen instance;
    private void Awake() {
        instance = this;
        //yourAddress.text = SessionHandler.instance.getSessionAddress();
    }
    public void WriteInfo(string _balance)
    {
        currentBalance.text = _balance;
    }
}
