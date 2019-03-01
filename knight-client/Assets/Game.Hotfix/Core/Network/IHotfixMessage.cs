using System;
using System.Collections.Generic;

namespace Knight.Hotfix.Core
{
    public interface IHotfixMessage
    {
    }

    public interface IHotfixRequest : IHotfixMessage
    {
        int             RpcId   { get; set; }
    }

    public interface IHotfixResponse : IHotfixMessage
    {
        int             Error   { get; set; }
        string          Message { get; set; }
        int             RpcId   { get; set; }
    }
}
