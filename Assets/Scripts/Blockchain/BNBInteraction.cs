using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Unity.Rpc;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Unity.Metamask;
using Nethereum.Util;

public class BNBInteraction : MonoBehaviour
{
    public static BNBInteraction instance;
    ///===>>> Token contract information
    string url = "https://rpc.ankr.com/bsc_testnet_chapel";
    string account = "0x65EdA2B6E940ccd72dd07b2384DC76f18E90Dacb"; //contract owner testing purposes
    string privateKey = "d3212873f98d90f81fcef815f57d982317fe601f6fa0f2f18218ca449f22c292"; //contract owner key testing purposes
    string ContractAddress = "0x5A5E585F79915F7c6122999446644Da79Cf006b3"; //token contract
    string accountPlayer; //playeraddress
    decimal playerTokenBalance;
    ///===>>> NFT contract information
    string NFTContractAddress = "0x752905D58F7Ea5Ce26175920d1E0cfA02C19b13a"; //NFT contract

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; }
    }
    [FunctionOutput]
    public class BalanceOfFunctionOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)]
        public BigInteger Balance { get; set; }
    }
    [Function("transfer", "bool")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public string To { get; set; }
        [Parameter("uint256", "_value", 2)]
        public BigInteger Value { get; set; }
    }

    public partial class TransferFunction : TransferFunctionBase { }
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //accountPlayer = SessionHandler.instance.getSessionAddress();

        //testing
        accountPlayer = "0x1f882709A1B1E00b449Afc215B3F13213E7Dc8eD";
    }
    public bool IsWebGL()
    {
#if UNITY_WEBGL
        return true;
#else
      return false;
#endif
    }
    public decimal GetBalance() { return playerTokenBalance; }
    public void ReadBalance()
    {
        StartCoroutine(Balance());
    }
    IEnumerator Balance()
    {
        var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfFunctionOutput>(url, account);
        yield return queryRequest.Query(new BalanceOfFunction() { Owner = accountPlayer }, ContractAddress);
        //Getting the dto response already decoded
        var dtoResult = queryRequest.Result;
        TokenScreen.instance.WriteInfo(UnitConversion.Convert.FromWei(queryRequest.Result.Balance).ToString());
        playerTokenBalance = UnitConversion.Convert.FromWei(queryRequest.Result.Balance);
    }
    public void GetTokens() { StartCoroutine(Transfer()); }
    IEnumerator Transfer()
    {
        TokenScreen.instance.WriteInfo("transfering");
        //read balance
        var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfFunctionOutput>(url, account);
        yield return queryRequest.Query(new BalanceOfFunction() { Owner = accountPlayer }, ContractAddress);
        //Getting the dto response already decoded
        var dtoResult = queryRequest.Result;
        playerTokenBalance = UnitConversion.Convert.FromWei(queryRequest.Result.Balance);
        //transfer tokens
        var transactionTransferRequest = new TransactionSignedUnityRequest(url, privateKey, 97);
        transactionTransferRequest.UseLegacyAsDefault = true;
        Debug.Log(playerTokenBalance);
        Debug.Log("uinit to transfer");
        Debug.Log(UnitConversion.Convert.ToWei(10));
        var transactionMessage = new TransferFunction
        {
            FromAddress = account,
            To = accountPlayer,
            Value = (playerTokenBalance == 0) ? UnitConversion.Convert.ToWei(10) : UnitConversion.Convert.ToWei(playerTokenBalance * 5),
        };
        yield return transactionTransferRequest.SignAndSendTransaction(transactionMessage, ContractAddress);
        if (transactionTransferRequest.Exception != null)
        {
            Debug.Log("Error transfering: " + transactionTransferRequest.Exception.Message);
        }
        else
        {
            var transactionTransferHash = transactionTransferRequest.Result;
            Debug.Log("Transfer txn hash:" + transactionTransferHash);
            yield return new WaitForSeconds(3f);
            StartCoroutine(Balance());
        }

    }

    public void BuyLand(int _index)
    {
        StartCoroutine(SendToken());
    }
    public IEnumerator SendToken()
    {
        var transferRequest = new MetamaskTransactionUnityRequest(accountPlayer, GetUnityRpcRequestClientFactory());
        var transactionMessage = new TransferFunction
        {
            FromAddress = accountPlayer,
            To = account,
            Value = 100000,
        };
        yield return transferRequest.SignAndSendTransaction(transactionMessage, ContractAddress);
        var transactionTransferHash = transferRequest.Result;
        Debug.Log("Transfer txn hash:" + transactionTransferHash);
        TokenScreen.instance.WriteTx(transactionTransferHash);
    }
    public IEnumerator SignMessage()
    {
#if UNITY_WEBGL
        var personalSignRequest = new EthPersonalSignUnityRequest(GetUnityRpcRequestClientFactory());
        yield return personalSignRequest.SendRequest(new HexUTF8String("text sample"));
        if (personalSignRequest.Exception != null)
        {
            Debug.Log("Error signing message");
            Debug.Log(personalSignRequest.Exception.Message);
            yield break;
        }
        Debug.Log(personalSignRequest.Result);
#endif
    }
    public IUnityRpcRequestClientFactory GetUnityRpcRequestClientFactory()
    {
#if UNITY_WEBGL
        if (IsWebGL())
        {
            if (MetamaskInterop.IsMetamaskAvailable())
            {
                return new MetamaskRequestRpcClientFactory(accountPlayer, null, 60000);
            }
            else
            {
                // DisplayError("Metamask is not available, please install it");
                return null;
            }
        }
        else
        {
            return null;
        }
#endif
    }
}
