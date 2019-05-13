using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            System_Object_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            Knight_Core_ICustomAttributeProviderExpand_Binding.Register(app);
            System_Reflection_CustomAttributeExtensions_Binding.Register(app);
            System_String_Binding.Register(app);
            Knight_Core_UtilTool_Binding.Register(app);
            System_IO_File_Binding.Register(app);
            Knight_Core_WindJson_JsonParser_Binding.Register(app);
            System_Reflection_FieldInfo_Binding.Register(app);
            Knight_Core_WindJson_JsonNode_Binding.Register(app);
            System_Reflection_PropertyInfo_Binding.Register(app);
            System_IO_FileStream_Binding.Register(app);
            System_IO_BinaryWriter_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncTaskMethodBuilder_Binding.Register(app);
            Knight_Core_AssetLoader_Binding.Register(app);
            Knight_Core_IAssetLoader_Binding.Register(app);
            Knight_Core_DictExpand_Binding.Register(app);
            Knight_Core_TSingleton_1_ABPlatform_Binding.Register(app);
            Knight_Framework_AssetBundles_ABPlatform_Binding.Register(app);
            UnityFx_Async_AsyncExtensions_Binding.Register(app);
            UnityFx_Async_CompilerServices_AsyncAwaiter_1_AssetLoaderRequest_Binding.Register(app);
            Knight_Core_AssetLoaderRequest_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_TextAsset_Binding.Register(app);
            System_IO_MemoryStream_Binding.Register(app);
            System_IO_BinaryReader_Binding.Register(app);
            Knight_Core_TSingleton_1_HotfixEventManager_Binding.Register(app);
            Knight_Framework_Hotfix_HotfixEventManager_Binding.Register(app);
            System_Threading_Tasks_Task_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Int32_Binding.Register(app);
            Knight_Core_TSingleton_1_UIAtlasManager_Binding.Register(app);
            UnityEngine_UI_UIAtlasManager_Binding.Register(app);
            System_Threading_Tasks_Task_1_ILTypeInstance_Binding.Register(app);
            System_Runtime_CompilerServices_TaskAwaiter_1_ILTypeInstance_Binding.Register(app);
            Knight_Core_GameLoading_Binding.Register(app);
            Knight_Core_WaitAsync_Binding.Register(app);
            UnityFx_Async_CompilerServices_AsyncAwaiter_1_Knight_Core_WaitAsync_Binding_WaitForSecondsRequest_Binding.Register(app);
            Knight_Core_Dict_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_CKeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            Knight_Core_CKeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Single_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            System_Collections_Generic_Stack_1_ILTypeInstance_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncTaskMethodBuilder_1_ILTypeInstance_Binding.Register(app);
            Knight_Core_EventArg_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            Knight_Core_ObservableList_1_ILTypeInstance_Binding.Register(app);
            System_Runtime_CompilerServices_AsyncTaskMethodBuilder_1_String_Binding.Register(app);
            Knight_Core_TSingleton_1_EventManager_Binding.Register(app);
            Knight_Core_EventManager_Binding.Register(app);
            Knight_Core_WindJson_JsonArray_Binding.Register(app);
            System_Collections_IEnumerable_Binding.Register(app);
            System_Array_Binding.Register(app);
            Knight_Core_WindJson_JsonClass_Binding.Register(app);
            System_Collections_IDictionary_Binding.Register(app);
            Knight_Core_IDict_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_Object_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_Object_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Object_Object_Binding.Register(app);
            Knight_Core_WindJson_JsonData_Binding.Register(app);
            Knight_Core_ReflectionAssist_Binding.Register(app);
            System_Reflection_ConstructorInfo_Binding.Register(app);
            System_Convert_Binding.Register(app);
            System_Reflection_ICustomAttributeProvider_Binding.Register(app);
            System_Collections_Hashtable_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding.Register(app);
            System_Collections_Generic_List_1_KeyValuePair_2_Type_Type_Binding.Register(app);
            Knight_Core_TSingleton_1_TypeResolveManager_Binding.Register(app);
            Knight_Core_TypeResolveManager_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Type_Type_Binding.Register(app);
            System_Collections_Generic_List_1_KeyValuePair_2_Type_Type_Binding_Enumerator_Binding.Register(app);
            UnityFx_Async_CompilerServices_AsyncAwaiter_1_Knight_Core_WaitAsync_Binding_WaitForEndOfFrameRequest_Binding.Register(app);
            System_Func_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_CKeyValuePair_2_Int32_ILTypeInstance_Binding.Register(app);
            System_Char_Binding.Register(app);
            UnityEngine_UI_DataBindingProperty_Binding.Register(app);
            UnityEngine_UI_EventBinding_Binding.Register(app);
            System_Linq_Enumerable_Binding.Register(app);
            System_Reflection_MethodInfo_Binding.Register(app);
            System_Reflection_MethodBase_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_UI_ViewControllerContainer_Binding.Register(app);
            UnityEngine_UI_MemberBindingAbstract_Binding.Register(app);
            System_Collections_Generic_List_1_ViewModelDataSource_Binding.Register(app);
            UnityEngine_UI_ViewModelDataSource_Binding.Register(app);
            System_Collections_Generic_List_1_EventBinding_Binding.Register(app);
            System_Collections_Generic_List_1_MemberBindingAbstract_Binding.Register(app);
            UnityEngine_UI_DataBindingTypeResolve_Binding.Register(app);
            UnityEngine_UI_DataBindingPropertyWatcher_Binding.Register(app);
            UnityEngine_UI_MemberBindingTwoWay_Binding.Register(app);
            System_Collections_Generic_List_1_ViewModelDataSourceList_Binding.Register(app);
            UnityEngine_UI_ViewModelDataSourceTemplate_Binding.Register(app);
            Knight_Core_IObservableEvent_Binding.Register(app);
            System_Collections_ICollection_Binding.Register(app);
            UnityEngine_UI_ViewModelDataSourceList_Binding.Register(app);
            UnityEngine_UI_LoopScrollRect_Binding.Register(app);
            System_Collections_IList_Binding.Register(app);
            UnityEngine_UI_ViewModelDataSourceArray_Binding.Register(app);
            UnityEngine_UI_UITool_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_UI_ViewModelDataSourceTab_Binding.Register(app);
            System_Collections_Generic_List_1_TabButton_Binding.Register(app);
            UnityEngine_UI_TabView_Binding.Register(app);
            Knight_Core_ObjectExpand_Binding.Register(app);
            UnityEngine_UI_Toggle_Binding.Register(app);
            UnityEngine_UI_TabButton_Binding.Register(app);
            Knight_Core_Dict_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_CKeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            Knight_Core_CKeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            UnityEngine_UI_UIRoot_Binding.Register(app);
            Knight_Core_IndexedDict_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_List_1_String_Binding.Register(app);
            Knight_Core_IndexedDictExpand_Binding.Register(app);
            Knight_Core_TSingleton_1_UIAssetLoader_Binding.Register(app);
            UnityEngine_UI_UIAssetLoader_Binding.Register(app);
            UnityEngine_UI_UIAssetLoader_Binding_LoaderRequest_Binding.Register(app);
            System_Guid_Binding.Register(app);
            UnityEngine_UI_DataBindingRelatedAttribute_Binding.Register(app);
            Knight_Core_Dict_2_String_List_1_String_Binding.Register(app);
            System_Action_1_String_Binding.Register(app);
            Knight_Framework_Net_Packet_Binding.Register(app);
            Knight_Framework_Net_NetworkSession_Binding.Register(app);
            Knight_Framework_Net_NetworkClient_Binding.Register(app);
            Knight_Framework_Net_NetworkOpcodeTypes_Binding.Register(app);
            System_Exception_Binding.Register(app);
            System_Action_1_ILTypeInstance_Binding.Register(app);
            System_Threading_Tasks_TaskCompletionSource_1_ILTypeInstance_Binding.Register(app);
            Knight_Core_Dict_2_Int32_Action_1_ILTypeInstance_Binding.Register(app);
            System_Threading_CancellationToken_Binding.Register(app);
            System_Collections_Generic_List_1_Byte_Array_Binding.Register(app);
            System_Byte_Binding.Register(app);
            Knight_Framework_Net_RpcException_Binding.Register(app);
            Microsoft_IO_RecyclableMemoryStreamManager_Binding.Register(app);
            System_ComponentModel_ISupportInitialize_Binding.Register(app);
            System_Collections_Generic_List_1_MemberInfo_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_InvalidOperationException_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Collections_Generic_List_1_UnityObject_Binding.Register(app);
            Knight_Framework_Hotfix_UnityObject_Binding.Register(app);
            System_Collections_Generic_List_1_Type_Binding_Enumerator_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
