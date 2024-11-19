using System;
using TouchSocket.Core;

namespace Knight.Framework.Network
{
    public struct NetworkPackageHeader
    {
        public const int HeaderSize = 12;
        public const int MaxDataLen = 1024 * 1024 - 12;

        public const int DataLenPos = 0;
        public const int ErrorCodePos = 4;
        public const int CmdPos = 6;
        public const int ActPos = 7;
        public const int IndexPos = 8;
        public const int FlagsPos = 10;
        public const int BccPos = 11;
        public const int BodyPos = 12;

        public UInt32 DataLen;
        public UInt16 ErrorCode;
        public Byte Cmd;
        public Byte Act;
        public UInt16 Index;
        public Byte Flags;
        public Byte Bcc;
    }

    public class NetworkPackage_FixedHeaderRequestInfo : IFixedHeaderRequestInfo
    {
        public NetworkPackageHeader PackageHeader;

        public int BodyLength => (int)this.PackageHeader.DataLen;

        public bool OnParsingHeader(byte[] header)
        {
            return true;
        }

        public bool OnParsingHeader(ReadOnlySpan<byte> rHeader)
        {
            if (rHeader.Length == NetworkPackageHeader.HeaderSize)
            {
                this.PackageHeader.DataLen = BitConverter.ToUInt32(rHeader.Slice(NetworkPackageHeader.DataLenPos, sizeof(uint)));
                this.PackageHeader.ErrorCode = BitConverter.ToUInt16(rHeader.Slice(NetworkPackageHeader.ErrorCodePos, sizeof(ushort)));
                this.PackageHeader.Cmd = rHeader[NetworkPackageHeader.CmdPos];
                this.PackageHeader.Act = rHeader[NetworkPackageHeader.ActPos];
                this.PackageHeader.Index = BitConverter.ToUInt16(rHeader.Slice(NetworkPackageHeader.IndexPos, sizeof(ushort)));
                this.PackageHeader.Flags = rHeader[NetworkPackageHeader.FlagsPos];
                this.PackageHeader.Bcc = rHeader[NetworkPackageHeader.BccPos];
                return true;
            }
            return false;
        }

        public bool OnParsingBody(ReadOnlySpan<byte> rBody)
        {
            if (rBody.Length == this.BodyLength)
            {
                return true;
            }
            return false;
        }
    }

    public class NetworkPackage_CustomFixedHeaderDataHandlingAdapter : CustomFixedHeaderDataHandlingAdapter<NetworkPackage_FixedHeaderRequestInfo>
    {
        public override int HeaderLength => NetworkPackageHeader.HeaderSize;

        protected override NetworkPackage_FixedHeaderRequestInfo GetInstance()
        {
            return new NetworkPackage_FixedHeaderRequestInfo();
        }
    }
}
