using System;

namespace Knight.Framework.Net
{
	/// <summary>
	/// RPC异常,带ErrorCode
	/// </summary>
	[Serializable]
	public class RpcException: Exception
	{
		public int Error { get; private set; }

		public RpcException(int nError, string rMessage): base($"Error: {nError} Message: {rMessage}")
		{
			this.Error = nError;
		}

		public RpcException(int nError, string rMessage, Exception rException): base($"Error: {nError} Message: {rMessage}", rException)
		{
			this.Error = nError;
		}
	}
}