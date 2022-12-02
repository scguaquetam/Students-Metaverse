using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Unity.Rpc;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Unity.Metamask;
using Nethereum.Util;
using Nethereum.Unity.Contracts.Standards.ERC721;
using Nethereum.Contracts.Standards.ERC721;
using Nethereum.Unity.Utils.Drawing;
using Nethereum.Unity.Contracts;

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
    string NFTContractAddress = "0x92caF4F46e1F661A94cc515feE80a7Dd269d2427"; //NFT contract
    int userNftsCount;
    public SpriteRenderer stageInWorld;

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
    public partial class MintFunction : MintFunctionBase { }
    [Function("mint")]
    public class MintFunctionBase : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public virtual string To { get; set; }
    }
    public partial class SafeMint : SafeMintFunctionBase { }
    [Function("safeMint")]
    public class SafeMintFunctionBase : FunctionMessage
    {
        [Parameter("address", "to", 1)]
        public virtual string To { get; set; }
        [Parameter("string", "uri", 2)]
        public virtual string Uri { get; set; }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////start functions ====>
    private void Awake()
    {
        instance = this;
        Debug.Log(accountPlayer);
        Debug.Log(SessionHandler.instance.getSessionAddress().ToString());
        Debug.Log(SessionHandler.instance.getSessionAddress());
        string address = SessionHandler.instance.getSessionAddress();
        Debug.Log(address);
        accountPlayer = address;
        Debug.Log(accountPlayer);
    }
    private void Start()
    {
        //ReadNftStatus();
        // //testing
        // accountPlayer = "0x0F257dBb886d4f56BED0ff0Ae0ee229D8818f0Fc";
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
        Debug.Log(accountPlayer);
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
    public IEnumerator PayWithZToken(BigInteger _value)
    {
        var transferRequest = new MetamaskTransactionUnityRequest(accountPlayer, GetUnityRpcRequestClientFactory());
        var transactionMessage = new TransferFunction
        {
            FromAddress = accountPlayer,
            To = account,
            Value = _value,
        };
        yield return transferRequest.SignAndSendTransaction(transactionMessage, ContractAddress);
        if(transferRequest.Exception != null)
        {
            PurchaseScreen.instance.loadingPanel.SetActive(false);
            PurchaseScreen.instance.ShowError(transferRequest.Exception.Message);
            yield break;
        }
        if (transferRequest.Result != null)
        {
            InteractionManager.instance.HideDoors();
            PurchaseScreen.instance.loadingPanel.SetActive(false);
            var transactionTransferHash = transferRequest.Result;
            PurchaseScreen.instance.ShowAnswer("Payment correct, hash: " + transactionTransferHash);
        }
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
        print("1111");
#if UNITY_WEBGL
        if (IsWebGL())
        {
            print("2222");
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
#endif
        else
        {
            print("3333");
            return new UnityWebRequestRpcClientFactory(url);
        }


        // /testing
        // return new UnityWebRequestRpcClientFactory(url);
    }

    #region NFT
    public void ReadNftStatus()
    {
        StartCoroutine(GetNFTsMetadata());
    }
    public IEnumerator GetNFTsMetadata()
    {
        var nftsOfUser = new NFTsOfUserUnityRequest(accountPlayer, GetUnityRpcRequestClientFactory());
        yield return nftsOfUser.GetAllMetadataUrls(NFTContractAddress, accountPlayer);
        if (nftsOfUser.Exception != null)
        {
            PurchaseScreen.instance.ShowError(nftsOfUser.Exception.Message);
            yield break;
        }
        if (nftsOfUser.Result != null)
        {
            Debug.Log("22");
            Debug.Log(nftsOfUser.Result.Count);
            userNftsCount = nftsOfUser.Result.Count;
            Debug.Log("221");
            if (nftsOfUser.Result.Count <= 0)
            {
                Debug.Log("no NFT");
                PurchaseScreen.instance.LoadInfoStage(0, true);
                Debug.Log("no NFT 1");
                stageInWorld.sprite = PurchaseScreen.instance.GetSprite(0);
                Debug.Log("no NFT 2");
                PurchaseScreen.instance.loadingPanel.SetActive(false);
            }
            else
            {
                Debug.Log("more than 1");
                var metadataUnityRequest = new NftMetadataUnityRequest<NftMetadata>();
                Debug.Log("b1");
                yield return metadataUnityRequest.GetAllMetadata(nftsOfUser.Result);
                Debug.Log("b2");
                Debug.Log(metadataUnityRequest);
                PurchaseScreen.instance.ShowAnswer(metadataUnityRequest.ToString());
                if (metadataUnityRequest.Exception != null)
                {
                    PurchaseScreen.instance.ShowError(metadataUnityRequest.Exception.Message);
                    yield break;
                }
                Debug.Log("b3");
                if (metadataUnityRequest.Result != null)
                {
                    Debug.Log("b4");
                    var image = new Image();
                    //_lstViewNFTs.hierarchy.Add(image);
                    Debug.LogWarning(metadataUnityRequest.Result[0].Image);
                    Debug.LogWarning(metadataUnityRequest.Result[0].Name);
                    Debug.LogWarning(metadataUnityRequest.Result[0].Description);
                    Debug.LogWarning(metadataUnityRequest.Result[0].ExternalUrl);
                    // StartCoroutine(new ImageDownloaderTextureAssigner().DownloadAndSetImageTexture(metadataUnityRequest.Result[0].Image, image));
                    // Debug.Log("b5");
                    // PurchaseScreen.instance.SetNFTImage(image.sprite);
                    // Debug.Log("b6");
                    PurchaseScreen.instance.loadingPanel.SetActive(false);
                }
            }
        }
    }
    public void OnPurchaseButton(int _index, string _price, bool _hasNoNFT)
    {
        if(_index == 0)
        {
            if (_hasNoNFT) // if does not have NFT purchase stage 0
            {
                //StartCoroutine(MintNFT());
                BigInteger priceInToken = int.Parse(_price);
                StartCoroutine(PayWithZToken(priceInToken));
            }
            else
            {

            }
        }
    }
    public IEnumerator MintNFT()
    {
        var contractTransactionUnityRequest = GetContractTransactionUnityRequest();
        Debug.Log("3");
        if (contractTransactionUnityRequest != null)
        {
            Debug.Log("4");
            Debug.Log(accountPlayer);
            var mintFunction = new MintFunction() { To = accountPlayer };
            Debug.Log("4 1");
            yield return contractTransactionUnityRequest.SignAndSendTransaction<MintFunction>(mintFunction, NFTContractAddress);
            Debug.Log("5");
            if (contractTransactionUnityRequest.Exception == null)
            {
                Debug.Log("6");
                PurchaseScreen.instance.ShowAnswer(contractTransactionUnityRequest.Result);
                StartCoroutine(GetNFTsMetadata());
            }
            else
            {
                Debug.Log("7");
                PurchaseScreen.instance.ShowError(contractTransactionUnityRequest.Exception.Message);
            }
        }
    }
    public IContractTransactionUnityRequest GetContractTransactionUnityRequest()
    {
        var transactionTransferRequest = new TransactionSignedUnityRequest(url, privateKey, 97);
        transactionTransferRequest.UseLegacyAsDefault = true;
        return transactionTransferRequest;
    }
    #endregion
}
