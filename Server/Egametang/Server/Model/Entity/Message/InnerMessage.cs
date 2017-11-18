
#region __SERIALIZER__PARTIAL_
//using Core.Serializer;
//using System.Collections.Generic;

//// 服务器内部消息 Opcode从10000开始
//namespace Model
//{
//	public abstract partial class AActorMessage : AMessage
//	{
//	}

//	public abstract partial class AActorRequest : ARequest
//	{
//	}

//	public abstract partial class AActorResponse : AResponse
//	{
//	}

//	/// <summary>
//	/// 用来包装actor消息
//	/// </summary>
//    [Message(Opcode.ActorRequest)]
//	public partial class ActorRequest : ARequest
//	{
//		public long Id { get; set; }

//        [SBDynamic]
//		public AMessage AMessage { get; set; }
//	}

//	/// <summary>
//	/// actor RPC消息响应
//	/// </summary>
//	[Message(Opcode.ActorResponse)]
//	public partial class ActorResponse : AResponse
//	{
//	}

//	/// <summary>
//	/// 用来包装actor消息
//	/// </summary>
//	[Message(Opcode.ActorRpcRequest)]
//	public partial class ActorRpcRequest : ActorRequest
//	{
//	}

//	/// <summary>
//	/// actor RPC消息响应带回应
//	/// </summary>
//    [Message(Opcode.ActorRpcResponse)]
//	public partial class ActorRpcResponse : ActorResponse
//	{
//		public AMessage AMessage { get; set; }
//	}


//	/// <summary>
//	/// 传送unit
//	/// </summary>
//    [Message(Opcode.M2M_TrasferUnitRequest)]
//	public partial class M2M_TrasferUnitRequest : ARequest
//	{
//        [SBIgnore]
//		public Unit Unit;
//	}

//	[Message(Opcode.M2M_TrasferUnitResponse)]
//	public partial class M2M_TrasferUnitResponse : AResponse
//	{
//	}



//	[Message(Opcode.M2A_Reload)]
//	public partial class M2A_Reload : ARequest
//	{
//	}


//	[Message(Opcode.A2M_Reload)]
//	public partial class A2M_Reload : AResponse
//	{
//	}


//	[Message(Opcode.G2G_LockRequest)]
//	public partial class G2G_LockRequest : ARequest
//	{
//		public long Id;
//		public string Address;
//	}


//	[Message(Opcode.G2G_LockResponse)]
//	public partial class G2G_LockResponse : AResponse
//	{
//	}

//	[Message(Opcode.G2G_LockReleaseRequest)]
//	public partial class G2G_LockReleaseRequest : ARequest
//	{
//		public long Id;
//		public string Address;
//	}


//	[Message(Opcode.G2G_LockReleaseResponse)]
//	public partial class G2G_LockReleaseResponse : AResponse
//	{
//	}

//	[Message(Opcode.DBSaveRequest)]
//	public partial class DBSaveRequest : ARequest
//	{
//		public bool NeedCache = true;

//		public string CollectionName = "";
//        [SBIgnore]
//		public Entity Entity;
//	}



//	[Message(Opcode.DBSaveBatchResponse)]
//	public partial class DBSaveBatchResponse : AResponse
//	{
//	}


//	[Message(Opcode.DBSaveBatchRequest)]
//	public partial class DBSaveBatchRequest : ARequest
//	{
//		public bool NeedCache = true;
//		public string CollectionName = "";
//        [SBIgnore]
//		public List<Entity> Entitys = new List<Entity>();
//	}

//	[Message(Opcode.DBSaveResponse)]
//	public partial class DBSaveResponse : AResponse
//	{
//	}


//	[Message(Opcode.DBQueryRequest)]
//	public partial class DBQueryRequest : ARequest
//	{
//		public long Id;
//		public string CollectionName { get; set; }
//		public bool NeedCache = true;
//	}


//	[Message(Opcode.DBQueryResponse)]
//	public partial class DBQueryResponse : AResponse
//	{
//        [SBIgnore]
//		public Entity Entity;
//	}


//	[Message(Opcode.DBQueryBatchRequest)]
//	public partial class DBQueryBatchRequest : ARequest
//	{
//		public string CollectionName { get; set; }
//		public List<long> IdList { get; set; }
//		public bool NeedCache = true;
//	}

//	[Message(Opcode.DBQueryBatchResponse)]
//	public partial class DBQueryBatchResponse : AResponse
//	{
//        [SBIgnore]
//		public List<Entity> Entitys;
//	}


//	[Message(Opcode.DBQueryJsonRequest)]
//	public partial class DBQueryJsonRequest : ARequest
//	{
//		public string CollectionName { get; set; }
//		public string Json { get; set; }
//		public bool NeedCache = true;
//	}

//	[Message(Opcode.DBQueryJsonResponse)]
//	public partial class DBQueryJsonResponse : AResponse
//	{
//        [SBIgnore]
//		public List<Entity> Entitys;
//	}

//	[Message(Opcode.ObjectAddRequest)]
//	public partial class ObjectAddRequest : ARequest
//	{
//		public long Key { get; set; }
//		public int AppId { get; set; }
//	}

//	[Message(Opcode.ObjectAddResponse)]
//	public partial class ObjectAddResponse : AResponse
//	{
//	}

//	[Message(Opcode.ObjectRemoveRequest)]
//	public partial class ObjectRemoveRequest : ARequest
//	{
//		public long Key { get; set; }
//	}

//	[Message(Opcode.ObjectRemoveResponse)]
//	public partial class ObjectRemoveResponse : AResponse
//	{
//	}

//	[Message(Opcode.ObjectLockRequest)]
//	public partial class ObjectLockRequest : ARequest
//	{
//		public long Key { get; set; }
//		public int LockAppId { get; set; }
//		public int Time { get; set; }
//	}

//	[Message(Opcode.ObjectLockResponse)]
//	public partial class ObjectLockResponse : AResponse
//	{
//	}

//	[Message(Opcode.ObjectUnLockRequest)]
//	public partial class ObjectUnLockRequest : ARequest
//	{
//		public long Key { get; set; }
//		public int UnLockAppId { get; set; }
//		public int AppId { get; set; }
//	}

//	[Message(Opcode.ObjectUnLockResponse)]
//	public partial class ObjectUnLockResponse : AResponse
//	{
//	}

//	[Message(Opcode.ObjectGetRequest)]
//	public partial class ObjectGetRequest : ARequest
//	{
//		public long Key { get; set; }
//	}

//	[Message(Opcode.ObjectGetResponse)]
//	public partial class ObjectGetResponse : AResponse
//	{
//		public int AppId { get; set; }
//	}


//	[Message(Opcode.R2G_GetLoginKey)]
//	public partial class R2G_GetLoginKey : ARequest
//	{
//		public string Account;
//	}


//	[Message(Opcode.G2R_GetLoginKey)]
//	public partial class G2R_GetLoginKey : AResponse
//	{
//		public long Key;

//		public G2R_GetLoginKey()
//		{
//		}

//		public G2R_GetLoginKey(long key)
//		{
//			this.Key = key;
//		}
//	}


//	[Message(Opcode.G2M_CreateUnit)]
//	public partial class G2M_CreateUnit : ARequest
//	{
//		public long PlayerId;
//		public long GateSessionId;
//	}


//	[Message(Opcode.M2G_CreateUnit)]
//	public partial class M2G_CreateUnit : AResponse
//	{
//		public long UnitId;
//		public int Count;
//	}
//}

#endregion //__SERIALIZER__PARTIAL_

using System.Collections.Generic;using MongoDB.Bson.Serialization.Attributes;

// 服务器内部消息 Opcode从10000开始
namespace Model{
    public abstract partial class AMessage
    {
    }

    public abstract partial class ARequest : AMessage
    {
    }

    public abstract partial class AResponse : AMessage
    {
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
    
    /// <summary>    /// 用来包装actor消息    /// </summary>    [Message(Opcode.ActorRequest)]
    [BsonIgnoreExtraElements]
    public partial class ActorRequest : ARequest
    {
        public long Id { get; set; }

        public AMessage AMessage { get; set; }
    }
    
    /// <summary>    /// actor RPC消息响应    /// </summary>    [Message(Opcode.ActorResponse)]
    [BsonIgnoreExtraElements]
    public partial class ActorResponse : AResponse
    {
    }
    
    /// <summary>    /// 用来包装actor消息    /// </summary>    [Message(Opcode.ActorRpcRequest)]
    [BsonIgnoreExtraElements]
    public partial class ActorRpcRequest : ActorRequest
    {
    }

    /// <summary>    /// actor RPC消息响应带回应    /// </summary>    [Message(Opcode.ActorRpcResponse)]
    [BsonIgnoreExtraElements]
    public partial class ActorRpcResponse : ActorResponse
    {
        public AMessage AMessage { get; set; }
    }
    
    /// <summary>    /// 传送unit    /// </summary>    [Message(Opcode.M2M_TrasferUnitRequest)]
    [BsonIgnoreExtraElements]
    public partial class M2M_TrasferUnitRequest : ARequest
    {
        public Unit Unit;
    }

    [Message(Opcode.M2M_TrasferUnitResponse)]
    [BsonIgnoreExtraElements]
    public partial class M2M_TrasferUnitResponse : AResponse
    {
    }
    
    [Message(Opcode.M2A_Reload)]
    [BsonIgnoreExtraElements]
    public partial class M2A_Reload : ARequest
    {
    }
    
    [Message(Opcode.A2M_Reload)]
    [BsonIgnoreExtraElements]
    public partial class A2M_Reload : AResponse
    {
    }


    [Message(Opcode.G2G_LockRequest)]
    [BsonIgnoreExtraElements]
    public partial class G2G_LockRequest : ARequest
    {
        public long Id;
        public string Address;
    }


    [Message(Opcode.G2G_LockResponse)]
    [BsonIgnoreExtraElements]
    public partial class G2G_LockResponse : AResponse
    {
    }

    [Message(Opcode.G2G_LockReleaseRequest)]
    [BsonIgnoreExtraElements]
    public partial class G2G_LockReleaseRequest : ARequest
    {
        public long Id;
        public string Address;
    }


    [Message(Opcode.G2G_LockReleaseResponse)]
    [BsonIgnoreExtraElements]
    public partial class G2G_LockReleaseResponse : AResponse
    {
    }

    [Message(Opcode.DBSaveRequest)]
    [BsonIgnoreExtraElements]
    public partial class DBSaveRequest : ARequest
    {
        public bool NeedCache = true;

        public string CollectionName = "";

        public Entity Entity;
    }



    [Message(Opcode.DBSaveBatchResponse)]
    [BsonIgnoreExtraElements]
    public partial class DBSaveBatchResponse : AResponse
    {
    }


    [Message(Opcode.DBSaveBatchRequest)]
    [BsonIgnoreExtraElements]
    public partial class DBSaveBatchRequest : ARequest
    {
        public bool NeedCache = true;
        public string CollectionName = "";
        public List<Entity> Entitys = new List<Entity>();
    }

    [Message(Opcode.DBSaveResponse)]
    [BsonIgnoreExtraElements]
    public partial class DBSaveResponse : AResponse
    {
    }


    [Message(Opcode.DBQueryRequest)]
    [BsonIgnoreExtraElements]
    public partial class DBQueryRequest : ARequest
    {
        public long Id;
        public string CollectionName { get; set; }
        public bool NeedCache = true;
    }


    [Message(Opcode.DBQueryResponse)]
    [BsonIgnoreExtraElements]
    public partial class DBQueryResponse : AResponse
    {
        public Entity Entity;
    }


    [Message(Opcode.DBQueryBatchRequest)]
    [BsonIgnoreExtraElements]
    public partial class DBQueryBatchRequest : ARequest
    {
        public string CollectionName { get; set; }
        public List<long> IdList { get; set; }
        public bool NeedCache = true;
    }

    [Message(Opcode.DBQueryBatchResponse)]
    [BsonIgnoreExtraElements]
    public partial class DBQueryBatchResponse : AResponse
    {
        public List<Entity> Entitys;
    }


    [Message(Opcode.DBQueryJsonRequest)]
    [BsonIgnoreExtraElements]
    public partial class DBQueryJsonRequest : ARequest
    {
        public string CollectionName { get; set; }
        public string Json { get; set; }
        public bool NeedCache = true;
    }

    [Message(Opcode.DBQueryJsonResponse)]
    [BsonIgnoreExtraElements]
    public partial class DBQueryJsonResponse : AResponse
    {
        public List<Entity> Entitys;
    }

    [Message(Opcode.ObjectAddRequest)]
    [BsonIgnoreExtraElements]
    public partial class ObjectAddRequest : ARequest
    {
        public long Key { get; set; }
        public int AppId { get; set; }
    }

    [Message(Opcode.ObjectAddResponse)]
    [BsonIgnoreExtraElements]
    public partial class ObjectAddResponse : AResponse
    {
    }

    [Message(Opcode.ObjectRemoveRequest)]
    [BsonIgnoreExtraElements]
    public partial class ObjectRemoveRequest : ARequest
    {
        public long Key { get; set; }
    }

    [Message(Opcode.ObjectRemoveResponse)]
    [BsonIgnoreExtraElements]
    public partial class ObjectRemoveResponse : AResponse
    {
    }

    [Message(Opcode.ObjectLockRequest)]
    [BsonIgnoreExtraElements]
    public partial class ObjectLockRequest : ARequest
    {
        public long Key { get; set; }
        public int LockAppId { get; set; }
        public int Time { get; set; }
    }

    [Message(Opcode.ObjectLockResponse)]
    [BsonIgnoreExtraElements]
    public partial class ObjectLockResponse : AResponse
    {
    }

    [Message(Opcode.ObjectUnLockRequest)]
    [BsonIgnoreExtraElements]
    public partial class ObjectUnLockRequest : ARequest
    {
        public long Key { get; set; }
        public int UnLockAppId { get; set; }
        public int AppId { get; set; }
    }

    [Message(Opcode.ObjectUnLockResponse)]
    [BsonIgnoreExtraElements]
    public partial class ObjectUnLockResponse : AResponse
    {
    }

    [Message(Opcode.ObjectGetRequest)]
    [BsonIgnoreExtraElements]
    public partial class ObjectGetRequest : ARequest
    {
        public long Key { get; set; }
    }

    [Message(Opcode.ObjectGetResponse)]
    [BsonIgnoreExtraElements]
    public partial class ObjectGetResponse : AResponse
    {
        public int AppId { get; set; }
    }


    [Message(Opcode.R2G_GetLoginKey)]
    [BsonIgnoreExtraElements]
    public partial class R2G_GetLoginKey : ARequest
    {
        public string Account;
    }


    [Message(Opcode.G2R_GetLoginKey)]
    [BsonIgnoreExtraElements]
    public partial class G2R_GetLoginKey : AResponse
    {
        public long Key;

        public G2R_GetLoginKey()
        {
        }

        public G2R_GetLoginKey(long key)
        {
            this.Key = key;
        }
    }


    [Message(Opcode.G2M_CreateUnit)]
    [BsonIgnoreExtraElements]
    public partial class G2M_CreateUnit : ARequest
    {
        public long PlayerId;
        public long GateSessionId;
    }


    [Message(Opcode.M2G_CreateUnit)]
    [BsonIgnoreExtraElements]
    public partial class M2G_CreateUnit : AResponse
    {
        public long UnitId;
        public int Count;
    }}