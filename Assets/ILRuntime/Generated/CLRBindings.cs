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
            System_Int32_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Int64_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Array_Binding.Register(app);
            UnityEngine_Color_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_ILTypeInstance_ILTypeInstance_Binding.Register(app);
            Core_Dict_2_ILTypeInstance_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Object_Object_Binding.Register(app);
            Core_Dict_2_Object_Object_Binding.Register(app);
            Core_WindJson_JsonParser_Binding.Register(app);
            Core_WindJson_JsonArray_Binding.Register(app);
            Core_WindJson_JsonClass_Binding.Register(app);
            Core_WindJson_JsonData_Binding.Register(app);
            Framework_Hotfix_HotfixEventManager_Binding.Register(app);
            System_Delegate_Binding.Register(app);
        }
    }
}
