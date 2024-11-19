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
	public class PlayerProtoViewModel : ViewModel
	{
		[DataBinding]
		public System.Int64 ID { get; set; }

		[DataBinding]
		public List<RoleProtoViewModel> Roles { get; set; }

		public override void SyncData(IMessage rMessage)
		{
			if (rMessage == null)
			{
				this.ResetData();
				return;
			}
			var rPlayer = rMessage as Player;
			if (rPlayer == null)
			{
				LogManager.LogError("SyncData error: message is not {rClassName}");
				return;
			}

			if (this.ID != rPlayer.ID)
			{
				this.ID = rPlayer.ID;
			}
			if (rPlayer.Roles != null && rPlayer.Roles.Count > 0)
			{
				for (int i = 0; i < rPlayer.Roles.Count; i++)
				{
					if (i >= this.Roles.Count)
					{
						this.Roles.Add(new RoleProtoViewModel());
					}
					this.Roles[i].SyncData(rPlayer.Roles[i]);
				}
				if (rPlayer.Roles.Count < this.Roles.Count)
				{
					for (int i = rPlayer.Roles.Count; i < this.Roles.Count; i++)
					{
						this.Roles.RemoveAt(i);
					}
				}
			}
			else
			{
				this.Roles.Clear();
			}
		}

		public void ResetData()
		{
			this.ID = default(System.Int64);
			if (this.Roles != null)
			{
				this.Roles.Clear();
			}
			else
			{
				this.Roles = new List<RoleProtoViewModel>();
			}
		}
	}
}
