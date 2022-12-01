using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Nethereum.Unity.Rpc;
using Debug = UnityEngine.Debug;
#if UNITY_WEBGL
using Nethereum.Unity.Metamask;
#endif
using Nethereum.Unity.FeeSuggestions;
using Nethereum.Unity.Contracts;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using TMPro;
using UnityEngine.SceneManagement;
public class MetamaskConnection : MonoBehaviour
{
    private bool _isMetamaskInitialised = false;
    private string _selectedAccountAddress;
    public BigInteger ChainId = 444444444500;
    [SerializeField] TMP_Text LblError, addressTxt, chainTxt;
    [SerializeField] Button continueBtn;
     
    public bool IsWebGL()
    {
#if UNITY_WEBGL
        return true;
#else
      return false;
#endif
    }
    public void MetamaskConnect()
    {
#if UNITY_WEBGL
        if (IsWebGL())
        {
            if (MetamaskInterop.IsMetamaskAvailable())
            {
                MetamaskInterop.EnableEthereum(gameObject.name, nameof(EthereumEnabled), nameof(DisplayError));
                continueBtn.interactable = true;
            }
            else
            {
                DisplayError("Metamask is not available, please install it");
            }
        }
#endif

    }
    public void EthereumEnabled(string addressSelected)
    {
#if UNITY_WEBGL
        if (IsWebGL())
        {
            if (!_isMetamaskInitialised)
            {
                MetamaskInterop.EthereumInit(gameObject.name, nameof(NewAccountSelected), nameof(ChainChanged));
                MetamaskInterop.GetChainId(gameObject.name, nameof(ChainChanged), nameof(DisplayError));
                _isMetamaskInitialised = true;
            }
            NewAccountSelected(addressSelected);
        }
#endif
    }
    public void NewAccountSelected(string accountAddress)
    {
        _selectedAccountAddress = accountAddress;
        addressTxt.text = accountAddress;
        SessionHandler.instance.setSessionAddress(accountAddress);
        
    }
    public void ChainChanged(string chainId)
    {
        print(chainId);
        ChainId = new HexBigInteger(chainId).Value;
        SessionHandler.instance.setChainId(ChainId);
        chainTxt.text = chainId;
        //InputChainId.text = ChainId.ToString();
    }
    public void DisplayError(string errorMessage)
    {
        LblError.text = errorMessage;
    }
    public void ContinueBtn()
    {
        SceneManager.LoadScene("Planet1");
    }
}
