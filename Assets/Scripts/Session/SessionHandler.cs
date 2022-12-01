using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;

public class SessionHandler : MonoBehaviour
{
    public static SessionHandler instance;
    string sessionAddress;
    BigInteger chainId;
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
    public BigInteger getChainId() { return chainId; }
    public void setChainId(BigInteger _chain) { chainId = _chain; }
}
