using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TransferJson
{
    public string jsonrpc, method;
    public TransferJsonParams parameters;
    public TransferJson() {}
}