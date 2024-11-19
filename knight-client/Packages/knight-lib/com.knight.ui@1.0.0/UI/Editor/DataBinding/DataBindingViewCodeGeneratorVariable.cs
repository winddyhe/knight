using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.UI.Editor
{
    public class DataBindingCodeGeneratorProp
    {
        public Component PropComp;

        public List<DataBindingOneWay> OneWays;
        public List<DataBindingTwoWay> TwoWays;
        public List<DataBindingEvent> Events;
        public List<ViewModelDataSourceListTemplate> ListTemplates;

        public DataBindingCodeGeneratorProp(Component rPropComp)
        {
            this.PropComp = rPropComp;
            this.OneWays = new List<DataBindingOneWay>();
            this.TwoWays = new List<DataBindingTwoWay>();
            this.Events = new List<DataBindingEvent>();
            this.ListTemplates = new List<ViewModelDataSourceListTemplate>();
        }

        public int GetPropCount()
        {
            return this.OneWays.Count + this.TwoWays.Count + this.Events.Count;
        }

        public void AddOneWay(DataBindingOneWay rOneWay)
        {
            this.OneWays.Add(rOneWay);
        }

        public void AddTwoWay(DataBindingTwoWay rTwoWay)
        {
            this.TwoWays.Add(rTwoWay);
        }

        public void AddEvent(DataBindingEvent rEvent)
        {
            this.Events.Add(rEvent);
        }

        public void AddListTemplate(ViewModelDataSourceListTemplate rListTemplate)
        {
            this.ListTemplates.Add(rListTemplate);
        }
    }

    public class DataBindingCodeGeneratorVariable
    {
        public bool IsListTemlate;
        public string ListTemplateName;

        public string VariableName;
        public string VariableType;

        public string GenericArg0;
        public string GenericArg1;

        public DataBindingCodeGeneratorViewVariable ViewVariable;
        public DataBindingCodeGeneratorViewModelVariable ViewModelVariable;
        public DataBindingCodeGeneratorViewEvent ViewEvent;
    }

    public class DataBindingCodeGeneratorViewModel
    {
        public string Key;
        public string ViewModelTypeName;
        public string ViewModelName;
    }

    public class DataBindingCodeGeneratorViewModelVariable
    {
        public DataBindingCodeGeneratorViewModel ViewModel;
        public string PropName;
        public string PropType;
        public bool IsReleated;
        public List<string> RelatedProps;
        public List<string> RelatedTypes;
    }

    public class DataBindingCodeGeneratorViewVariable
    {
        public Component ViewComp;
        public string PropName;
        public string PropType;
    }

    public class DataBindingCodeGenertorEvent
    {
        public Component ViewComp;
        public string EventName;
        public string ViewVariableName;

        public string ViewControllerName;
        public string ViewControllerEventName;
    }

    public class DataBindingCodeGeneratorViewSetMethod
    {
        public bool IsListTemlate;
        public string ViewVariableName;
        public string ViewModelBindableName;
        public string ArgType;
        public string ViewPropName;
        public string MethodName;
        public bool IsReleated;
        public string RelatedViewModelPropName;
        public string ViewModelPropName;
    }

    public class DataBindingCodeGeneratorViewEvent
    {
        public Component ViewComp;
        public string ViewEventVariableName;
        public string EventName;
    }

    public class DataBindingCodeGeneratorViewEventMethod
    {
        public Component ViewComp;

        public string ViewVariableName;
        public string ViewModelPropName;
        public string MethodName;
        public string EventName;

        public string ViewControllerName;
        public string ViewControllerEventName;
    }

    public class DataBindingCodeGeneratorViewControllerEvent
    {
        public string ViewControllerName;
        public string EventName;
    }
}
