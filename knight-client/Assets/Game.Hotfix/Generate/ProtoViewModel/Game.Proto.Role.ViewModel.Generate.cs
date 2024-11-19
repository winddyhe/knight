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
	public class RoleProtoViewModel : ViewModel
	{
		[DataBinding]
		public System.Int32 ID { get; set; }

		[DataBinding]
		public System.String Name { get; set; }

		[DataBinding]
		public System.String HeadIcon { get; set; }

		[DataBinding]
		public Game.Proto.Role.Types.ProfessionalType Professional { get; set; }

		[DataBinding]
		public BagProtoViewModel ItemBag { get; set; }

		[DataBinding]
		public Google.Protobuf.WellKnownTypes.Timestamp CreateTime { get; set; }

		public override void SyncData(IMessage rMessage)
		{
			if (rMessage == null)
			{
				this.ResetData();
				return;
			}
			var rRole = rMessage as Role;
			if (rRole == null)
			{
				LogManager.LogError("SyncData error: message is not {rClassName}");
				return;
			}

			if (this.ID != rRole.ID)
			{
				this.ID = rRole.ID;
			}
			if (this.Name != rRole.Name)
			{
				this.Name = rRole.Name;
			}
			if (this.HeadIcon != rRole.HeadIcon)
			{
				this.HeadIcon = rRole.HeadIcon;
			}
			if (this.Professional != rRole.Professional)
			{
				this.Professional = rRole.Professional;
			}
			if (this.ItemBag == null)
			{
				this.ItemBag = new BagProtoViewModel();
			}
			this.ItemBag.SyncData(rRole.ItemBag);
			if (this.CreateTime != rRole.CreateTime)
			{
				this.CreateTime.MergeFrom(rRole.CreateTime);
			}
		}

		public void ResetData()
		{
			this.ID = default(System.Int32);
			this.Name = default(System.String);
			this.HeadIcon = default(System.String);
			this.Professional = default(Game.Proto.Role.Types.ProfessionalType);
			if (this.ItemBag != null)
			{
				this.ItemBag.ResetData();
			}
			else
			{
				this.ItemBag = new BagProtoViewModel();
			}
			this.CreateTime = new Google.Protobuf.WellKnownTypes.Timestamp();
		}
	}
}
