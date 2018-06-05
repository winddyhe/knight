using System;
using System.Collections.Generic;
using Knight.Core;

namespace Knight.Framework.Net
{
    internal enum ParserState
    {
        PacketSize,
        PacketBody
    }

    public class Packet
    {
        public const int MinSize = 3;
        public const int MaxSize = 60000;
        public const int FlagIndex = 0;
        public const int OpcodeIndex = 1;
        public const int Index = 3;

        /// <summary>
        /// 只读，不允许修改
        /// </summary>
        public byte[]   Bytes   { get;      }
        public ushort   Length  { get; set; }
        public byte     Flag    { get; set; }
        public ushort   Opcode  { get; set; }

        public Packet(int length)
        {
            this.Length = 0;
            this.Bytes = new byte[length];
        }

        public Packet(byte[] bytes)
        {
            this.Bytes = bytes;
            this.Length = (ushort)bytes.Length;
        }
    }

    internal class PacketParser
    {
        private readonly CircularBuffer mBuffer;
        private ushort                  mPacketSize;
        private ParserState             mState;
        private readonly Packet         mPacket = new Packet(ushort.MaxValue);
        private bool                    mIsOK;

        public PacketParser(CircularBuffer buffer)
        {
            this.mBuffer = buffer;
        }

        public bool Parse()
        {
            if (this.mIsOK)
            {
                return true;
            }

            bool bFinish = false;
            while (!bFinish)
            {
                switch (this.mState)
                {
                    case ParserState.PacketSize:
                        if (this.mBuffer.Length < 2)
                        {
                            bFinish = true;
                        }
                        else
                        {
                            this.mBuffer.Read(this.mPacket.Bytes, 0, 2);
                            this.mPacketSize = BitConverter.ToUInt16(this.mPacket.Bytes, 0);
                            if (mPacketSize < Packet.MinSize || mPacketSize > Packet.MaxSize)
                            {
                                throw new Exception($"packet size error: {this.mPacketSize}");
                            }
                            this.mState = ParserState.PacketBody;
                        }
                        break;
                    case ParserState.PacketBody:
                        if (this.mBuffer.Length < this.mPacketSize)
                        {
                            bFinish = true;
                        }
                        else
                        {
                            this.mBuffer.Read(this.mPacket.Bytes, 0, this.mPacketSize);
                            this.mPacket.Length = this.mPacketSize;
                            this.mPacket.Flag = this.mPacket.Bytes[0];
                            this.mPacket.Opcode = BitConverter.ToUInt16(this.mPacket.Bytes, Packet.OpcodeIndex);
                            this.mIsOK = true;
                            this.mState = ParserState.PacketSize;
                            bFinish = true;
                        }
                        break;
                }
            }
            return this.mIsOK;
        }

        public Packet GetPacket()
        {
            this.mIsOK = false;
            return this.mPacket;
        }
    }
}
