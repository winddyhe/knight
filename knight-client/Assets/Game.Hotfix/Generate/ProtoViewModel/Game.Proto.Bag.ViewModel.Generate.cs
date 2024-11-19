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
	public class BagProtoViewModel : ViewModel
	{
		[DataBinding]
		public List<BagItemProtoViewModel> Items { get; set; }

		public override void SyncData(IMessage rMessage)
		{
			if (rMessage == null)
			{
				this.ResetData();
				return;
			}
			var rBag = rMessage as Bag;
			if (rBag == null)
			{
				LogManager.LogError("SyncData error: message is not {rClassName}");
				return;
			}

			if (rBag.Items != null && rBag.Items.Count > 0)
			{
				for (int i = 0; i < rBag.Items.Count; i++)
				{
					if (i >= this.Items.Count)
					{
						this.Items.Add(new BagItemProtoViewModel());
					}
					this.Items[i].SyncData(rBag.Items[i]);
				}
				if (rBag.Items.Count < this.Items.Count)
				{
					for (int i = rBag.Items.Count; i < this.Items.Count; i++)
					{
						this.Items.RemoveAt(i);
					}
				}
			}
			else
			{
				this.Items.Clear();
			}
		}

		public void ResetData()
		{
			if (this.Items != null)
			{
				this.Items.Clear();
			}
			else
			{
				this.Items = new List<BagItemProtoViewModel>();
			}
		}
	}
}
