using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Unity.Rpc;
using System.Numerics;

public class BNBInteraction : MonoBehaviour
{
    public static BNBInteraction instance;
    string url = "https://rpc.ankr.com/bsc_testnet_chapel";
    string account = "0x65EdA2B6E940ccd72dd07b2384DC76f18E90Dacb"; //contract owner
    string privateKey = "d3212873f98d90f81fcef815f57d982317fe601f6fa0f2f18218ca449f22c292"; //contract owner key
    string ContractAddress = "0x5A5E585F79915F7c6122999446644Da79Cf006b3"; //token contract
    string accountPlayer; //playeraddress
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

    public partial class TransferFunction : TransferFunctionBase
    {

    }
    private void Awake() {
        instance = this;
    }
    private void Start() {
        accountPlayer = SessionHandler.instance.getSessionAddress();

        // //testing
        // accountPlayer = "0x0F257dBb886d4f56BED0ff0Ae0ee229D8818f0Fc";
    }
    public void ReadBalance() 
    {
        StartCoroutine(Balance());
    }
    IEnumerator Balance( bool last = false)
    {
        var queryRequest = new QueryUnityRequest<BalanceOfFunction, BalanceOfFunctionOutput>(url, account);
        yield return queryRequest.Query(new BalanceOfFunction() { Owner = accountPlayer }, ContractAddress);
        //Getting the dto response already decoded
        var dtoResult = queryRequest.Result;
        // Debug.Log("balance is");
        // Debug.Log(dtoResult.Balance);
        TokenScreen.instance.WriteInfo((queryRequest.Result.Balance/1000000000000000000).ToString());
    }
    IEnumerator Transfer()
    {
        var transactionTransferRequest = new TransactionSignedUnityRequest(url, privateKey);
        transactionTransferRequest.UseLegacyAsDefault = true;

        var newAddress = accountPlayer;

        var transactionMessage = new TransferFunction
        {
            FromAddress = account,
            To = newAddress,
            Value = 7,
        };
        yield return transactionTransferRequest.SignAndSendTransaction(transactionMessage, ContractAddress);
        var transactionTransferHash = transactionTransferRequest.Result;
        Debug.Log("Transfer txn hash:" + transactionTransferHash);
        yield return new WaitForSeconds(3f);
        StartCoroutine(Balance(true));
    }
}
