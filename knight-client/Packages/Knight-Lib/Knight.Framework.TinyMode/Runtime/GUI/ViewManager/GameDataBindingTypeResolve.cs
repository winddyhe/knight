//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Knight.Core;

namespace Knight.Framework.TinyMode.UI
{
    public class GameDataBindingTypeResolve
    {
        public static DataBindingProperty MakeViewModelDataBindingProperty(string rViewModelPath)
        {
            if (string.IsNullOrEmpty(rViewModelPath)) return null;

            var rViewModelPathStrs = rViewModelPath.Split('/');
            if (rViewModelPathStrs.Length < 2) return null;

            var rViewModelClass = rViewModelPathStrs[0].Trim();
            var rViewModelClassStrs = rViewModelClass.Split('@');
            if (rViewModelClassStrs.Length < 1) return null;

            var rViewModelClassKey = rViewModelClassStrs[0].Trim();
            var rViewModelClassName = rViewModelClassStrs[1].Trim();

            var rViewModelProp = rViewModelPathStrs[1].Trim();

            var rViewModelPropStrs = rViewModelProp.Split(':');
            if (rViewModelPropStrs.Length < 1) return null;

            var rViewModelPropName = rViewModelPropStrs[0].Trim();
            var rViewModelProperty = new DataBindingProperty(null, rViewModelClassKey, rViewModelPropName);
            rViewModelProperty.Property = TypeResolveManager.Instance.GetType(rViewModelClassName).GetProperty(rViewModelPropName);
            return rViewModelProperty;
        }
        
        public static bool MakeViewModelDataBindingEvent(ViewController rViewController, EventBinding rEventBinding)
        {
            if (string.IsNullOrEmpty(rEventBinding.ViewModelMethod)) return false;

            var rViewModelMethods = rEventBinding.ViewModelMethod.Split('/');
            if (rViewModelMethods.Length < 2) return false;

            var rViewModelEventClass = rViewModelMethods[0];
            var rViewModelEventName = rViewModelMethods[1];

            var rBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
            var rMethodInfo = rViewController.GetType().GetMethods(rBindingFlags)
                .Where(method => method.Name.Equals(rViewModelEventName)
            ).FirstOrDefault();

            if (rMethodInfo != null)
            {
                Action<EventArg> rActionDelegate = (rEventArg) => { rMethodInfo.Invoke(rViewController, new object[] { rEventArg }); };
                rEventBinding.InitEventWatcher(rActionDelegate);
            }
            else
            {
                Debug.LogErrorFormat("Can not find Method: {0} in ViewController.", rEventBinding.ViewModelMethod);
                return false;
            }
            return true;
        }

        public static bool MakeListViewModelDataBindingEvent(ViewController rViewController, EventBinding rEventBinding, int nIndex)
        {
            if (string.IsNullOrEmpty(rEventBinding.ViewModelMethod)) return false;

            var rViewModelMethods = rEventBinding.ViewModelMethod.Split('/');
            if (rViewModelMethods.Length < 2) return false;

            var rViewModelEventClass = rViewModelMethods[0];
            var rViewModelEventName = rViewModelMethods[1];

            var rBindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod;
            var rMethodInfo = rViewController.GetType().GetMethods(rBindingFlags)
                .Where(method => method.Name.Equals(rViewModelEventName)
            ).FirstOrDefault();

            if (rMethodInfo != null)
            {
                Action<EventArg> rActionDelegate = (rEventArg) => { rMethodInfo.Invoke(rViewController, new object[] { nIndex, rEventArg }); };
                rEventBinding.InitEventWatcher(rActionDelegate);
            }
            else
            {
                Debug.LogErrorFormat("Can not find Method: {0} in ViewController.", rEventBinding.ViewModelMethod);
                return false;
            }

            return true;
        }
    }
}
