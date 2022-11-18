using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Unity.Rpc;

public class BNBInteraction : MonoBehaviour
{
    string url = "https://rpc.ankr.com/bsc_testnet_chapel";
    string account = "0x65EdA2B6E940ccd72dd07b2384DC76f18E90Dacb"; //contract owner
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
        public int Balance { get; set; }
    }
    public static BNBInteraction instance;

    private void Awake() {
        instance = this;
    }
    private void Start() {
        //accountPlayer = SessionHandler.instance.getSessionAddress();
        
        //testing
        accountPlayer = "0x0F257dBb886d4f56BED0ff0Ae0ee229D8818f0Fc";
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
        Debug.Log("balance is");
        Debug.Log(dtoResult.Balance);
    }
}
