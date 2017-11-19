using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Core;
using Model;

namespace Framework.Network
{
	public sealed class Session : ASession
	{
        private static uint                         STATIC_ID = 1;

		private static uint                         RpcId            { get; set; }

		private ETClient                            mNetwork;
		private AChannel                            mChannel;
		private List<byte[]>                        mByteses         = new List<byte[]>() {new byte[0], new byte[0]};
        
        public string                               RemoteAddress    => this.mChannel?.RemoteAddress;
        public ChannelType                          ChannelType      => this.mChannel.ChannelType;

		public Session(ETClient rNetwork, AChannel rChannel)
		{
            this.Id = STATIC_ID++;

			this.mNetwork = rNetwork;
			this.mChannel = rChannel;

			this.StartRecv();
		}

		private async void StartRecv()
		{
			while (true)
			{
				if (this.Id == 0)
				{
					return;
				}

				byte[] rMessageBytes;
				try
				{
                    if (mChannel == null) return;

					rMessageBytes = await mChannel.Recv();
					if (this.Id == 0)
					{
						return;
					}
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
                    continue;
				}

				if (rMessageBytes.Length < 3)
				{
					continue;
				}

				ushort nOpcode = BitConverter.ToUInt16(rMessageBytes, 0);
				try
				{
					this.Run(nOpcode, rMessageBytes);
				}
				catch (Exception e)
				{
					Log.Error(e.ToString());
				}
			}
		}

		private void Run(ushort nOpcode, byte[] rMessageBytes)
		{
			int nOffset = 0;
			bool bIsCompressed = (nOpcode & 0x8000) > 0;    // opcode最高位表示是否压缩

            if (bIsCompressed) // 最高位为1,表示有压缩,需要解压缩
			{
				rMessageBytes = ZipHelper.Decompress(rMessageBytes, 2, rMessageBytes.Length - 2);
				nOffset = 0;
			}
			else
			{
				nOffset = 2;
			}
			nOpcode &= 0x7fff;
			this.RunDecompressedBytes(nOpcode, rMessageBytes, nOffset);
		}

		private void RunDecompressedBytes(ushort nOpcode, byte[] rMessageBytes, int nOffset)
		{
            this.mNetwork.MessageDispatcher.Invoke("Dispatch", nOpcode, rMessageBytes, nOffset);
		}

        public void SendMessage(ushort nOpcode, byte[] rMessageBytes)
        {
            if (rMessageBytes.Length > 100)
            {
                byte[] rNewMessageBytes = ZipHelper.Compress(rMessageBytes);
                if (rNewMessageBytes.Length < rMessageBytes.Length)
                {
                    rMessageBytes = rNewMessageBytes;
                    nOpcode |= 0x8000;
                }
            }

            byte[] rOpcodeBytes = BitConverter.GetBytes(nOpcode);

            this.mByteses[0] = rOpcodeBytes;
            this.mByteses[1] = rMessageBytes;

            mChannel.Send(this.mByteses);
        }

        public void Dispose()
		{
			if (this.Id == 0)
			{
				return;
			}

			long nId = this.Id;
            
			this.mChannel.Dispose();
			this.mNetwork.Remove(nId);

            this.Id = 0;
		}
	}
}