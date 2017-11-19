using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WindHotfix.Core;
using Model;
using Core;
using System;
using System.Reflection;

namespace WindHotfix.Net
{
    [HotfixSBGroupInerited("Protocol")]
    public abstract partial class AMessage : HotfixSerializerBinary
    {
        public override string ToString()
        {
            var rJsonNode = HotfixJsonParser.ToJsonNode(this);
            return rJsonNode?.ToString();
        }
    }

    public abstract partial class ARequest : AMessage
    {
        public uint RpcId;
    }

    /// <summary>
    /// 服务端回的RPC消息需要继承这个抽象类
    /// </summary>
    public abstract partial class AResponse : AMessage
    {
        public uint RpcId;
        public int Error = 0;
        public string Message = "";
    }

    public abstract partial class AActorMessage : AMessage
    {
    }

    public abstract partial class AActorRequest : ARequest
    {
    }

    public abstract partial class AActorResponse : AResponse
    {
    }

    /// <summary>
    /// 帧消息，继承这个类的消息会经过服务端转发
    /// </summary>
    public abstract partial class AFrameMessage : AActorMessage
    {
    }
}