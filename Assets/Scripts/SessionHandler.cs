using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionHandler : MonoBehaviour
{
    public static SessionHandler instance;
    string sessionAddress, chainId;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public string getSessionAddress() { return sessionAddress; }
    public void setSessionAddress(string _address) { sessionAddress = _address; }
    public string getChainId() { return chainId; }
    public void setChainId(string _chain) { chainId = _chain; }
}
