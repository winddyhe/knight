using Game.Proto;
using Google.Protobuf;
using Knight.Core;
using Knight.Framework.UI;
using System.Collections.Generic;
using System;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	[DataBinding]
	public class BagItemProtoViewModel : ViewModel
	{
		[DataBinding]
		public System.Int32 ID { get; set; }

		[DataBinding]
		public System.Int32 Count { get; set; }

		public override void SyncData(IMessage rMessage)
		{
			if (rMessage == null)
			{
				this.ResetData();
				return;
			}
			var rBagItem = rMessage as BagItem;
			if (rBagItem == null)
			{
				LogManager.LogError("SyncData error: message is not {rClassName}");
				return;
			}

			if (this.ID != rBagItem.ID)
			{
				this.ID = rBagItem.ID;
			}
			if (this.Count != rBagItem.Count)
			{
				this.Count = rBagItem.Count;
			}
		}

		public void ResetData()
		{
			this.ID = default(System.Int32);
			this.Count = default(System.Int32);
		}
	}
}
