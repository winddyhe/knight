using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Knight.Core.Editor;
using Knight.Core;
using System.Linq;
using System.Reflection;

namespace Knight.Framework.UI.Editor
{
    public class DataBindingView_CodeGenerator : CodeGenerator
    {
        private GameObject mGameObject;
        private string mViewControllerName;

        private Dictionary<Component, DataBindingCodeGeneratorProp> mDataBindingProps;

        private int mViewModelCount;
        private Dictionary<string, Dictionary<string, DataBindingCodeGeneratorViewModel>> mViewModels;

        private Dictionary<Component, DataBindingCodeGeneratorVariable> mViewBindingVariables;
        private Dictionary<string, DataBindingCodeGeneratorVariable> mViewModelBindingVariables;
        private Dictionary<DataBindingEvent, DataBindingCodeGenertorEvent> mViewBindingEvents;

        private List<DataBindingCodeGeneratorViewSetMethod> mViewSetMethods;
        private List<DataBindingCodeGeneratorViewEventMethod> mViewEventMethods;
        private List<DataBindingCodeGeneratorViewEventMethod> mViewBindingEventMethods;

        public DataBindingView_CodeGenerator(string rFilePath, GameObject rGameObject, string rViewControllerName)
            : base(rFilePath)
        {
            this.mGameObject = rGameObject;
            this.mViewControllerName = rViewControllerName;

            this.mDataBindingProps = new Dictionary<Component, DataBindingCodeGeneratorProp>();

            this.mViewModelCount = 0;
            this.mViewModels = new Dictionary<string, Dictionary<string, DataBindingCodeGeneratorViewModel>>();
            this.mViewBindingVariables = new Dictionary<Component, DataBindingCodeGeneratorVariable>();
            this.mViewModelBindingVariables = new Dictionary<string, DataBindingCodeGeneratorVariable>();
            this.mViewBindingEvents = new Dictionary<DataBindingEvent, DataBindingCodeGenertorEvent>();

            this.mViewSetMethods = new List<DataBindingCodeGeneratorViewSetMethod>();
            this.mViewEventMethods = new List<DataBindingCodeGeneratorViewEventMethod>();
            this.mViewBindingEventMethods = new List<DataBindingCodeGeneratorViewEventMethod>();
        }

        public override void CodeGenerate()
        {
            this.GenerateHeader();
            this.GenerateBody();
            this.GenerateFooter();
        }

        private void GenerateHeader()
        {
            this.StringBuilder
                .AL("using System;")
                .AL("using System.Collections;")
                .AL("using System.Collections.Generic;")
                .AL("using UnityEngine;")
                .AL("using UnityEngine.UI;")
                .AL("using Knight.Framework.UI;")
                .N();

            this.StringBuilder
                .AL("///<summary>")
                .AL("/// Auto Generate By Knight, Not Edit It Manually.")
                .AL("///</summary>")
                .AL("namespace Game")
                .AL("{");
        }

        private void GenerateBody()
        {
            this.mDataBindingProps.Clear();

            this.mViewModelCount = 0;
            this.mViewModels.Clear();
            this.mViewBindingVariables.Clear();
            this.mViewModelBindingVariables.Clear();
            this.mViewBindingEvents.Clear();

            this.mViewSetMethods.Clear();
            this.mViewEventMethods.Clear();
            this.mViewBindingEventMethods.Clear();

            this.BuildDataBindingProps();

            var rViewName = this.mViewControllerName.Replace("ViewController", "View");
            this.StringBuilder
                .T(1).AL($"public class {rViewName} : View")
                .T(1).AL("{");

            this.GenerateAllVaribles();
            this.GenerateInitializeMethod();
            this.GenerateBindMethod();
            this.GenerateUnbindMethod();
            this.GenerateViewSetMethod();

            this.StringBuilder
                .T(1).AL("}");
        }

        private void BuildDataBindingProps()
        {
            var rAllDataBindingOneWays = this.mGameObject.GetComponentsInChildren<DataBindingOneWay>(true);
            foreach (var rDataBindingOneWay in rAllDataBindingOneWays)
            {
                if (rDataBindingOneWay.IsListTemlate) continue;

                var rViewComp = rDataBindingOneWay.GetViewComponent();
                if (rViewComp == null) continue;

                if (!this.mDataBindingProps.TryGetValue(rViewComp, out var rProp))
                {
                    rProp = new DataBindingCodeGeneratorProp(rViewComp);
                    this.mDataBindingProps.Add(rViewComp, rProp);
                }
                rProp.AddOneWay(rDataBindingOneWay);
            }

            var rAllDataBindingTwoWays = this.mGameObject.GetComponentsInChildren<DataBindingTwoWay>(true);
            foreach (var rDataBindingTwoWay in rAllDataBindingTwoWays)
            {
                if (rDataBindingTwoWay.IsListTemlate) continue;

                var rViewComp = rDataBindingTwoWay.GetViewComponent();
                if (rViewComp == null) continue;

                if (!this.mDataBindingProps.TryGetValue(rViewComp, out var rProp))
                {
                    rProp = new DataBindingCodeGeneratorProp(rViewComp);
                    this.mDataBindingProps.Add(rViewComp, rProp);
                }
                rProp.AddTwoWay(rDataBindingTwoWay);
            }

            var rAllDataBindingEvents = this.mGameObject.GetComponentsInChildren<DataBindingEvent>(true);
            foreach (var rDataBindingEvent in rAllDataBindingEvents)
            {
                if (rDataBindingEvent.IsListTemlate) continue;

                var rViewComp = rDataBindingEvent.GetViewComponent();
                if (rViewComp == null) continue;

                if (!this.mDataBindingProps.TryGetValue(rViewComp, out var rProp))
                {
                    rProp = new DataBindingCodeGeneratorProp(rViewComp);
                    this.mDataBindingProps.Add(rViewComp, rProp);
                }
                rProp.AddEvent(rDataBindingEvent);
            }

            var rAllDataBindingListTemplates = this.mGameObject.GetComponentsInChildren<ViewModelDataSourceListTemplate>(true);
            foreach (var rDataBindingListTemplate in rAllDataBindingListTemplates)
            {
                var rViewComp = rDataBindingListTemplate.ScrollView;
                if (rViewComp == null) continue;

                if (!this.mDataBindingProps.TryGetValue(rViewComp, out var rProp))
                {
                    rProp = new DataBindingCodeGeneratorProp(rViewComp);
                    this.mDataBindingProps.Add(rViewComp, rProp);
                }
                rProp.AddListTemplate(rDataBindingListTemplate);
            }

            this.mViewModelCount = 0;
            var rAllViewModelDataSources = this.mGameObject.GetComponentsInChildren<ViewModelDataSource>(true);
            foreach (var rViewModelDataSource in rAllViewModelDataSources)
            {
                var rViewModelFullName = rViewModelDataSource.ViewModelPath;
                var rKey = rViewModelDataSource.Key;

                if (!this.mViewModels.TryGetValue(rViewModelFullName, out var rViewModelDict))
                {
                    rViewModelDict = new Dictionary<string, DataBindingCodeGeneratorViewModel>();
                    this.mViewModels.Add(rViewModelFullName, rViewModelDict);
                }

                var rViewModelName = rViewModelFullName.Split('.').Last();
                if (!rViewModelDict.TryGetValue(rKey, out var rViewModel))
                {
                    rViewModel = new DataBindingCodeGeneratorViewModel()
                    {
                        Key = rKey,
                        ViewModelTypeName = rViewModelFullName,
                        ViewModelName = rViewModelName
                    };
                    rViewModelDict.Add(rKey, rViewModel);
                    this.mViewModelCount++;
                }
                else
                {
                    Debug.LogError($"Duplicate ViewModel Key: {rKey}");
                }
            }
        }

        private void GenerateAllVaribles()
        {
            this.StringBuilder
                .T(2).AL($"public {this.mViewControllerName} {this.mViewControllerName};")
                .N();

            foreach (var rPropPair in this.mDataBindingProps.Values)
            {
                var rPropComp = rPropPair.PropComp;
                var rCompPath = UtilUnityTool.GetTransformPathByTrans(this.mGameObject.transform, rPropComp.transform);

                var rPropCompTypeName = rPropComp.GetType().FullName;
                var rPropCompVariableName = $"__{rPropComp.GetType().Name}__{rCompPath}__";
                this.StringBuilder
                    .T(2).AL($"public {rPropCompTypeName} {rPropCompVariableName};");
                this.mViewBindingVariables.Add(
                    rPropComp, 
                    new DataBindingCodeGeneratorVariable() 
                    { 
                        VariableName = rPropCompVariableName, 
                        VariableType = rPropCompTypeName, 
                    });

                int nIndex = 0;
                for (int i = 0; i < rPropPair.OneWays.Count; i++)
                {
                    var rOneWay = rPropPair.OneWays[i];

                    var rViewModelVariable = this.BuildViewModelVariable(rOneWay.ViewModelPath);
                    if (rViewModelVariable == null) continue;
                    
                    var rViewVariable = this.BuildViewVariable(rPropComp, rOneWay.ViewPath);
                    if (rViewVariable == null) continue;

                    var rViewModelTypeGenericArg0 = rPropCompTypeName;
                    var rViewModelTypeGenericArg1 = rViewModelVariable.ViewModel.ViewModelTypeName;
                    var rViewModelTypeName = $"BindableViewModel<{rViewModelTypeGenericArg0}, {rViewModelTypeGenericArg1}>";
                    var rViewModelVariableName = $"__{rPropComp.GetType().Name}__{rCompPath}__{rViewModelVariable.ViewModel.ViewModelName}{rViewModelVariable.ViewModel.Key}_{nIndex}";
                    this.mViewModelBindingVariables.Add(
                        rViewModelVariableName,
                        new DataBindingCodeGeneratorVariable()
                        {
                            IsListTemlate = false,
                            VariableName = rViewModelVariableName,
                            VariableType = rViewModelTypeName,
                            GenericArg0 = rViewModelTypeGenericArg0,
                            GenericArg1 = rViewModelTypeGenericArg1,
                            ViewModelVariable = rViewModelVariable,
                            ViewVariable = rViewVariable
                        });
                    this.StringBuilder
                        .T(2).AL($"public {rViewModelTypeName} {rViewModelVariableName};");
                    nIndex++;
                }
                nIndex = 0;
                for (int i = 0; i < rPropPair.TwoWays.Count; i++)
                {
                    var rTwoWay = rPropPair.TwoWays[i];

                    var rViewModelVariable = this.BuildViewModelVariable(rTwoWay.ViewModelPath);
                    if (rViewModelVariable == null) continue;

                    var rViewVariable = this.BuildViewVariable(rPropComp, rTwoWay.ViewPath);
                    if (rViewVariable == null) continue;

                    var rViewEvent = this.BuildViewEvent(rPropComp, rTwoWay.EventPath);
                    if (rViewEvent == null) continue;

                    var rViewModelTypeGenericArg0 = rPropCompTypeName;
                    var rViewModelTypeGenericArg1 = rViewModelVariable.ViewModel.ViewModelTypeName;
                    var rViewModelTypeName = $"BindableViewModel<{rViewModelTypeGenericArg0}, {rViewModelTypeGenericArg1}>";
                    var rViewModelVariableName = $"__{rPropComp.GetType().Name}__{rCompPath}__{rViewModelVariable.ViewModel.ViewModelName}{rViewModelVariable.ViewModel.Key}_{nIndex}";
                    this.mViewModelBindingVariables.Add(rViewModelVariableName,
                        new DataBindingCodeGeneratorVariable()
                        {
                            IsListTemlate = false,
                            VariableName = rViewModelVariableName,
                            VariableType = rViewModelTypeName,
                            GenericArg0 = rViewModelTypeGenericArg0,
                            GenericArg1 = rViewModelTypeGenericArg1,
                            ViewModelVariable = rViewModelVariable,
                            ViewVariable = rViewVariable,
                            ViewEvent = rViewEvent
                        });
                    this.StringBuilder
                        .T(2).AL($"public {rViewModelTypeName} {rViewModelVariableName};");
                    nIndex++;
                }
                for (int i = 0; i < rPropPair.Events.Count; i++)
                {
                    var rEvent = rPropPair.Events[i];

                    var rViewControllerEvent = this.BuildViewControllerEvent(rEvent.ViewModelEventPath);
                    if (rViewControllerEvent == null) continue;

                    var rViewEvent = this.BuildViewEvent(rPropComp, rEvent.ViewEventPath);
                    if (rViewEvent == null) continue;

                    var rViewEventMethod = new DataBindingCodeGenertorEvent();
                    rViewEventMethod.ViewComp = rPropComp;
                    rViewEventMethod.EventName = rViewEvent.EventName;
                    rViewEventMethod.ViewVariableName = rPropCompVariableName;
                    rViewEventMethod.ViewControllerName = rViewControllerEvent.ViewControllerName;
                    rViewEventMethod.ViewControllerEventName = rViewControllerEvent.EventName;

                    this.mViewBindingEvents.Add(rEvent, rViewEventMethod);
                }

                for (int i = 0; i < rPropPair.ListTemplates.Count; i++)
                {
                    var rListTemplate = rPropPair.ListTemplates[i];

                    var rViewModelVariable = this.BuildViewModelVariable(rListTemplate.ViewModelPath);
                    if (rViewModelVariable == null) continue;

                    var rViewVariable = this.BuildViewVariableListTemplate(rPropComp);
                    if (rViewVariable == null) continue;

                    var rViewModelTypeGenericArg0 = rPropCompTypeName;
                    var rViewModelTypeGenericArg1 = rViewModelVariable.ViewModel.ViewModelTypeName;
                    var rViewModelTypeName = $"BindableViewModel<{rViewModelTypeGenericArg0}, {rViewModelTypeGenericArg1}>";
                    var rViewModelVariableName = $"__{rPropComp.GetType().Name}__{rCompPath}__{rViewModelVariable.ViewModel.ViewModelName}{rViewModelVariable.ViewModel.Key}_{nIndex}";
                    this.mViewModelBindingVariables.Add(rViewModelVariableName,
                        new DataBindingCodeGeneratorVariable()
                        {
                            IsListTemlate = true,
                            ListTemplateName = rListTemplate.TemplatePath,
                            VariableName = rViewModelVariableName,
                            VariableType = rViewModelTypeName,
                            GenericArg0 = rViewModelTypeGenericArg0,
                            GenericArg1 = rViewModelTypeGenericArg1,
                            ViewModelVariable = rViewModelVariable,
                            ViewVariable = rViewVariable
                        });
                    this.StringBuilder
                        .T(2).AL($"public {rViewModelTypeName} {rViewModelVariableName};");
                    nIndex++;
                }

                this.StringBuilder
                    .N();
            }

            this.StringBuilder
                .T(2).AL($"public override ViewController ViewController => this.{this.mViewControllerName};")
                .N();
        }

        private void GenerateInitializeMethod()
        {
            this.StringBuilder
                .T(2).AL("public override void Initialize(ViewController rViewController)")
                .T(2).AL("{")
                    .T(3).AL($"this.{this.mViewControllerName} = ({this.mViewControllerName})rViewController;")
                    .T(3).AL("base.Initialize(rViewController);")
                    .N();

            foreach (var rPropPair in this.mViewModelBindingVariables)
            {
                this.StringBuilder
                    .T(3)
                    .AL($"this.{rPropPair.Value.VariableName} = new {rPropPair.Value.VariableType}();");

                if (this.mViewBindingVariables.TryGetValue(rPropPair.Value.ViewVariable.ViewComp, out var rViewVariable))
                {
                    this.StringBuilder
                        .T(3)
                        .AL($"this.{rPropPair.Value.VariableName}.ViewObject = this.{rViewVariable.VariableName};");
                }
            }

            this.StringBuilder
                .T(2).AL("}")
                .N();
        }

        private void GenerateBindMethod()
        {
            this.StringBuilder
                .T(2).AL($"public override void Bind()")
                .T(2).AL("{");

            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rViewModelName = rBindingVariablePair.Value.GenericArg1.Split('.').Last();
                this.StringBuilder
                    .T(3).AL($"this.{rBindingVariablePair.Value.VariableName}.ViewModel = ({rBindingVariablePair.Value.GenericArg1})this.ViewModels[\"{rBindingVariablePair.Value.ViewModelVariable.ViewModel.Key}\"];");
            }

            this.StringBuilder
                .N();

            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                if (!this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                {
                    continue;
                }

                if (!rBindingVariable.ViewModelVariable.IsReleated)
                {
                    var rViewSetMethod = new DataBindingCodeGeneratorViewSetMethod();
                    if (!rBindingVariable.IsListTemlate)
                    {
                        rViewSetMethod.IsListTemlate = false;
                        rViewSetMethod.MethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                        rViewSetMethod.ViewVariableName = rViewVariable.VariableName;
                        rViewSetMethod.ViewModelBindableName = rBindingVariablePair.Value.VariableName;
                        rViewSetMethod.ArgType = rBindingVariable.ViewVariable.PropType;
                        rViewSetMethod.ViewPropName = rBindingVariable.ViewVariable.PropName;
                        rViewSetMethod.IsReleated = false;
                        this.mViewSetMethods.Add(rViewSetMethod);
                    }
                    else
                    {
                        rViewSetMethod.IsListTemlate = true;
                        rViewSetMethod.MethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                        rViewSetMethod.ViewVariableName = rViewVariable.VariableName;
                        rViewSetMethod.ViewModelBindableName = rBindingVariablePair.Value.VariableName;
                        rViewSetMethod.ArgType = $"List<{rBindingVariable.ListTemplateName}>";
                        rViewSetMethod.IsReleated = false;
                        this.mViewSetMethods.Add(rViewSetMethod);
                    }
                    this.StringBuilder
                        .T(3).AL($"this.{rBindingVariable.VariableName}.ViewModel.RegisterPropertyChangeHandler<{rBindingVariable.ViewModelVariable.PropType}>")
                            .T(4).AL($"(nameof({rBindingVariable.ViewModelVariable.ViewModel.ViewModelTypeName}.{rBindingVariable.ViewModelVariable.PropName}), this.{rViewSetMethod.MethodName});");
                }
                else
                {
                    // 相对关系不处理列表模板
                    for (int i = 0; i < rBindingVariable.ViewModelVariable.RelatedProps.Count; i++)
                    {
                        var rViewSetMethod = new DataBindingCodeGeneratorViewSetMethod();
                        if (!rBindingVariable.IsListTemlate)
                        {
                            rViewSetMethod.IsListTemlate = false;
                            rViewSetMethod.MethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}_Related_{rBindingVariable.ViewModelVariable.RelatedProps[i]}";
                            rViewSetMethod.ViewVariableName = rViewVariable.VariableName;
                            rViewSetMethod.ViewModelBindableName = rBindingVariablePair.Value.VariableName;
                            rViewSetMethod.ViewPropName = rBindingVariable.ViewVariable.PropName;
                            rViewSetMethod.ArgType = rBindingVariable.ViewModelVariable.RelatedTypes[i];
                            rViewSetMethod.RelatedViewModelPropName = rBindingVariable.ViewModelVariable.RelatedProps[i];
                            rViewSetMethod.ViewModelPropName = rBindingVariable.ViewModelVariable.PropName;
                            rViewSetMethod.IsReleated = true;
                            this.mViewSetMethods.Add(rViewSetMethod);
                        }
                        this.StringBuilder
                            .T(3).AL($"this.{rBindingVariable.VariableName}.ViewModel.RegisterPropertyChangeHandler<{rBindingVariable.ViewModelVariable.RelatedTypes[i]}>")
                                .T(4).AL($"(nameof({rBindingVariable.ViewModelVariable.ViewModel.ViewModelTypeName}.{rBindingVariable.ViewModelVariable.RelatedProps[i]}), this.{rViewSetMethod.MethodName});");
                    }
                }

                if (rBindingVariable.ViewEvent != null)
                {
                    var rViewEventMethod = new DataBindingCodeGeneratorViewEventMethod();
                    rViewEventMethod.ViewComp = rBindingVariable.ViewVariable.ViewComp;
                    rViewEventMethod.MethodName = $"Event{rViewVariable.VariableName}_{rBindingVariable.ViewEvent.EventName}";
                    rViewEventMethod.EventName = rBindingVariable.ViewEvent.EventName;
                    rViewEventMethod.ViewVariableName = rBindingVariable.VariableName;
                    rViewEventMethod.ViewModelPropName = rBindingVariable.ViewModelVariable.PropName;
                    this.mViewEventMethods.Add(rViewEventMethod);

                    // 双向绑定
                    this.StringBuilder
                        .T(3).AL("// 双向绑定")
                        .T(3).AL($"this.{rBindingVariable.VariableName}.ViewObject.{rViewEventMethod.EventName}.AddListener(this.{rViewEventMethod.MethodName});")
                        .N();
                }
            }

            this.StringBuilder
                .T(3).AL("// 事件绑定");
            foreach (var rBindingEventPair in this.mViewBindingEvents)
            {
                var rBindingEvent = rBindingEventPair.Value;
                var rViewEventMethod = new DataBindingCodeGeneratorViewEventMethod();
                rViewEventMethod.ViewComp = rBindingEvent.ViewComp;
                rViewEventMethod.EventName = rBindingEvent.EventName;
                rViewEventMethod.MethodName = $"Event{rBindingEvent.ViewVariableName}_{rBindingEvent.EventName}";
                rViewEventMethod.ViewVariableName = rBindingEvent.ViewVariableName;
                rViewEventMethod.ViewControllerName = rBindingEvent.ViewControllerName;
                rViewEventMethod.ViewControllerEventName = rBindingEvent.ViewControllerEventName;
                this.mViewBindingEventMethods.Add(rViewEventMethod);

                this.StringBuilder
                    .T(3).AL($"this.{rBindingEvent.ViewVariableName}.{rBindingEvent.EventName}.AddListener(this.{rViewEventMethod.MethodName});")
                    .N();
            }

            this.StringBuilder
                .T(3).AL("// 数据初始化");
            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                if (!this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                {
                    continue;
                }
                if (!rBindingVariable.ViewModelVariable.IsReleated)
                {
                    this.StringBuilder
                            .T(3).A($"this.{rBindingVariable.VariableName}.ViewModel.{rBindingVariable.ViewModelVariable.PropName} = ")
                            .AL($"this.{rBindingVariable.VariableName}.ViewModel.{rBindingVariable.ViewModelVariable.PropName};");
                }
            }

            this.StringBuilder
                .T(2).AL("}")
                .N();
        }

        private void GenerateUnbindMethod()
        {
            this.StringBuilder
                .T(2).AL($"public override void UnBind()")
                .T(2).AL("{");

            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                if (!this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                {
                    continue;
                }

                if (!rBindingVariable.ViewModelVariable.IsReleated)
                {
                    var rMethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                    this.StringBuilder
                        .T(3).AL($"this.{rBindingVariable.VariableName}.ViewModel.UnregisterPropertyChangeHandler<{rBindingVariable.ViewModelVariable.PropType}>")
                        .T(4).AL($"(nameof({rBindingVariable.ViewModelVariable.ViewModel.ViewModelTypeName}.{rBindingVariable.ViewModelVariable.PropName}), this.{rMethodName});");
                }
                else
                {
                    // 相对关系不处理列表模板
                    for (int i = 0; i < rBindingVariable.ViewModelVariable.RelatedProps.Count; i++)
                    {
                        var rMethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}_Related_{rBindingVariable.ViewModelVariable.RelatedProps[i]}";
                        this.StringBuilder
                            .T(3).AL($"this.{rBindingVariable.VariableName}.ViewModel.UnregisterPropertyChangeHandler<{rBindingVariable.ViewModelVariable.RelatedTypes[i]}>")
                                .T(4).AL($"(nameof({rBindingVariable.ViewModelVariable.ViewModel.ViewModelTypeName}.{rBindingVariable.ViewModelVariable.RelatedProps[i]}), this.{rMethodName});");
                    }
                }

                if (rBindingVariable.ViewEvent != null)
                {
                    var rViewEventMethodName = $"Event{rViewVariable.VariableName}_{rBindingVariable.ViewEvent.EventName}";
                    var rViewEventName = rBindingVariable.ViewEvent.EventName;
                    // 双向绑定
                    this.StringBuilder
                        .T(3).AL("// 双向绑定")
                        .T(3).AL($"this.{rBindingVariable.VariableName}.ViewObject.{rViewEventName}.RemoveListener(this.{rViewEventMethodName});")
                        .N();
                }
            }

            this.StringBuilder
                .T(3).AL("// 事件绑定");
            foreach (var rBindingEventPair in this.mViewBindingEvents)
            {
                var rBindingEvent = rBindingEventPair.Value;
                var rMethodName = $"Event{rBindingEvent.ViewVariableName}_{rBindingEvent.EventName}";
                this.StringBuilder
                    .T(3).AL($"this.{rBindingEvent.ViewVariableName}.{rBindingEvent.EventName}.RemoveListener(this.{rMethodName});")
                    .N();
            }

            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rViewModelName = rBindingVariablePair.Value.GenericArg1.Split('.').Last();
                this.StringBuilder
                    .T(3).AL($"this.{rBindingVariablePair.Value.VariableName}.ViewModel = null;");
            }

            this.StringBuilder
                .N()
                .T(3).AL($"this.{this.mViewControllerName} = null;");

            this.StringBuilder
                .T(2).AL("}")
                .N();
        }

        private void GenerateViewSetMethod()
        {
            foreach (var rViewSetMethod in this.mViewSetMethods)
            {
                if (!rViewSetMethod.IsReleated)
                {
                    if (!rViewSetMethod.IsListTemlate)
                    {
                        this.StringBuilder
                            .T(2).AL($"private void {rViewSetMethod.MethodName}({rViewSetMethod.ArgType} rValue)")
                            .T(2).AL("{")
                                .T(3).AL($"this.{rViewSetMethod.ViewVariableName}.{rViewSetMethod.ViewPropName} = rValue;")
                            .T(2).AL("}")
                            .N();
                    }
                    else
                    {
                        var rTempDataListVariableName = $"__Temp_{rViewSetMethod.ViewVariableName}_DataList";
                        this.StringBuilder
                            .T(2).AL($"private List<FancyScrollCellData> {rTempDataListVariableName} = new List<FancyScrollCellData>();")
                            .T(2).AL($"private void {rViewSetMethod.MethodName}({rViewSetMethod.ArgType} rValueList)")
                            .T(2).AL("{")
                                .T(3).AL($"if (rValueList == null)")
                                .T(3).AL("{")
                                    .T(4).AL($"this.{rTempDataListVariableName}.Clear();")
                                    .T(4).AL($"this.{rViewSetMethod.ViewVariableName}.UpdateData(this.{rTempDataListVariableName});")
                                    .T(4).AL($"return;")
                                .T(3).AL("}")
                                .N()
                                .T(3).AL($"if (this.{rTempDataListVariableName}.Count > rValueList.Count)")
                                .T(3).AL("{")
                                    .T(4).AL($"this.{rTempDataListVariableName}.RemoveRange(rValueList.Count, this.{rTempDataListVariableName}.Count - rValueList.Count);")
                                .T(3).AL("}")
                                .T(3).AL("else")
                                .T(3).AL("{")
                                    .T(4).AL($"for (int i = this.{rTempDataListVariableName}.Count; i < rValueList.Count; i++)")
                                    .T(4).AL("{")
                                        .T(5).AL($"this.{rTempDataListVariableName}.Add(new FancyScrollCellData());")
                                    .T(4).AL("}")
                                .T(3).AL("}")
                                .T(3).AL("for (int i = 0; i < rValueList.Count; i++)")
                                .T(3).AL("{")
                                    .T(4).AL($"this.{rTempDataListVariableName}[i].ViewModel = rValueList[i];")
                                .T(3).AL("}")
                                .T(3).AL($"this.{rViewSetMethod.ViewVariableName}.UpdateData(this.{rTempDataListVariableName});")
                            .T(2).AL("}")
                            .N();
                    }
                }
                else 
                {
                    this.StringBuilder
                        .T(2).AL($"private void {rViewSetMethod.MethodName}({rViewSetMethod.ArgType} rValue)")
                        .T(2).AL("{")
                            .T(3).AL($"var rRelatedValue = this.{rViewSetMethod.ViewModelBindableName}.ViewModel.{rViewSetMethod.ViewModelPropName};")
                            .T(3).AL($"this.{rViewSetMethod.ViewVariableName}.{rViewSetMethod.ViewPropName} = rRelatedValue;")
                        .T(2).AL("}")
                        .N();
                }
            }

            foreach (var rViewEventMethod in this.mViewEventMethods)
            {
                var rUnityEventPropInfo = rViewEventMethod.ViewComp.GetType().GetProperty(rViewEventMethod.EventName);
                if (rUnityEventPropInfo == null) continue;

                var rEventArgTypes = rUnityEventPropInfo.PropertyType.BaseType.GenericTypeArguments;
                if (rEventArgTypes.Length > 1)
                {
                    Debug.LogError($"Event {rViewEventMethod.EventName} has more than one argument.");
                    continue;
                }

                this.StringBuilder
                    .T(2).A($"private void {rViewEventMethod.MethodName}(");

                for (int i = 0; i < rEventArgTypes.Length; i++)
                {
                    if (i <= rEventArgTypes.Length - 2)
                        this.StringBuilder.A($"{rEventArgTypes[i].FullName} rArg{i}, ");
                    else
                        this.StringBuilder.A($"{rEventArgTypes[i].FullName} rArg{i}");
                }
                this.StringBuilder.A(")").N();

                this.StringBuilder
                    .T(2).AL("{");

                if (rEventArgTypes.Length > 0)
                {
                    this.StringBuilder
                        .T(3).A($"this.{rViewEventMethod.ViewVariableName}.ViewModel.{rViewEventMethod.ViewModelPropName} = ")
                        .A($"rArg{0};")
                        .N();
                }

                this.StringBuilder
                    .T(2).AL("}")
                    .N();
            }

            foreach (var rViewBindingEventMethod in this.mViewBindingEventMethods)
            {
                var rUnityEventPropInfo = rViewBindingEventMethod.ViewComp.GetType().GetProperty(rViewBindingEventMethod.EventName);
                if (rUnityEventPropInfo == null) continue;

                var rEventArgTypes = rUnityEventPropInfo.PropertyType.BaseType.GenericTypeArguments;

                this.StringBuilder
                    .T(2).A($"private void {rViewBindingEventMethod.MethodName}(");
                for (int i = 0; i < rEventArgTypes.Length; i++)
                {
                    if (i <= rEventArgTypes.Length - 2)
                        this.StringBuilder.A($"{rEventArgTypes[i].FullName} rArg{i}, ");
                    else
                        this.StringBuilder.A($"{rEventArgTypes[i].FullName} rArg{i}");
                }
                this.StringBuilder.A(")").N();

                this.StringBuilder
                    .T(2).AL("{")
                    .T(3).A($"this.{this.mViewControllerName}.{rViewBindingEventMethod.ViewControllerEventName}(");
                for (int i = 0; i < rEventArgTypes.Length; i++)
                {
                    if (i <= rEventArgTypes.Length - 2)
                        this.StringBuilder.A($"rArg{i}, ");
                    else
                        this.StringBuilder.A($"rArg{i}");
                }
                this.StringBuilder.A(");").N();

                this.StringBuilder
                    .T(2).AL("}")
                    .N();
            }
        }

        private void GenerateFooter()
        {
            this.StringBuilder
                .A("}")
                .N();
        }

        private DataBindingCodeGeneratorViewModelVariable BuildViewModelVariable(string rViewModelPath)
        {
            if (string.IsNullOrEmpty(rViewModelPath)) return null;

            var rViewModelPathStrs = rViewModelPath.Split('/');
            if (rViewModelPathStrs.Length < 2) return null;

            var rKeyViewModelName = rViewModelPathStrs[0].Trim();
            var rKeyViewModelNameStrs = rKeyViewModelName.Split('$');

            if (rKeyViewModelNameStrs.Length < 2) return null;

            var rKey = rKeyViewModelNameStrs[0].Trim();
            var rViewModelFullName = rKeyViewModelNameStrs[1].Trim();

            var rPropPath = rViewModelPathStrs[1].Trim();
            var rPropPathStrs = rPropPath.Split(':');
            if (rPropPathStrs.Length < 2) return null;

            var rPropName = rPropPathStrs[0].Trim();
            var rPropType = rPropPathStrs[1].Trim();

            if (!this.mViewModels.TryGetValue(rViewModelFullName, out var rViewModelDict)) return null;
            if (!rViewModelDict.TryGetValue(rKey, out var rViewModel)) return null;

            var rViewModelVarible = new DataBindingCodeGeneratorViewModelVariable()
            {
                ViewModel = rViewModel,
                PropName = rPropName,
                PropType = rPropType
            };

            // 是否是Releated的ViewModel
            var rViewModelType = TypeResolveManager.Instance.GetType(rViewModel.ViewModelTypeName);
            var rPropInfo = rViewModelType.GetProperty(rPropName);
            if (rPropInfo != null)
            {
                var rReleatedViewModelAttr = rPropInfo.GetCustomAttribute<DataBindingRelatedAttribute>();
                if (rReleatedViewModelAttr != null)
                {
                    rViewModelVarible.IsReleated = true;
                    rViewModelVarible.RelatedProps = new List<string>();
                    rViewModelVarible.RelatedTypes = new List<string>();
                    var rProps = rReleatedViewModelAttr.PropName.Split(',');
                    foreach (var rProp in rProps)
                    {
                        rViewModelVarible.RelatedProps.Add(rProp.Trim());
                        var rRelatedPropInfo = rViewModelType.GetProperty(rProp.Trim());
                        rViewModelVarible.RelatedTypes.Add(rRelatedPropInfo.PropertyType.FullName);
                    }

                }
            }
            return rViewModelVarible;
        }

        private DataBindingCodeGeneratorViewVariable BuildViewVariable(Component rViewComp, string rViewPath)
        {
            var rViewPathStrs = rViewPath.Split('/');
            if (rViewPathStrs.Length < 2) return null;

            var rPropPath = rViewPathStrs[1].Trim();
            var rPropPathStrs = rPropPath.Split(':');
            if (rPropPathStrs.Length < 2) return null;

            var rPropName = rPropPathStrs[0].Trim();
            var rPropType = rPropPathStrs[1].Trim();

            var rViewVariable = new DataBindingCodeGeneratorViewVariable()
            {
                ViewComp = rViewComp,
                PropName = rPropName,
                PropType = rPropType
            };
            return rViewVariable;
        }

        private DataBindingCodeGeneratorViewVariable BuildViewVariableListTemplate(Component rViewComp)
        {
            var rViewVariable = new DataBindingCodeGeneratorViewVariable()
            {
                ViewComp = rViewComp,
                PropName = rViewComp.GetType().Name,
                PropType = rViewComp.GetType().Name
            };
            return rViewVariable;
        }

        private DataBindingCodeGeneratorViewEvent BuildViewEvent(Component rViewComp, string rEventPath)
        {
            var rViewPathStrs = rEventPath.Split('/');
            if (rViewPathStrs.Length < 2) return null;

            var rEventName = rViewPathStrs[1].Trim();
            var rViewEvent = new DataBindingCodeGeneratorViewEvent()
            {
                ViewComp = rViewComp,
                EventName = rEventName,
            };
            return rViewEvent;
        }

        private DataBindingCodeGeneratorViewControllerEvent BuildViewControllerEvent(string rEventPath)
        {
            var rViewPathStrs = rEventPath.Split('/');
            if (rViewPathStrs.Length < 2) return null;

            var rViewControllerTypeName = rViewPathStrs[0].Trim();
            var rEventName = rViewPathStrs[1].Trim();
            var rViewControllerEvent = new DataBindingCodeGeneratorViewControllerEvent()
            {
                ViewControllerName = rViewControllerTypeName,
                EventName = rEventName,
            };
            return rViewControllerEvent;
        }

        public void AddViewMonobehaviour(GameObject rGameObject, string rViewName)
        {
            var rViewType = TypeResolveManager.Instance.GetType($"Game.{rViewName}");
            if (rViewType == null) return;

            var rView = rGameObject.GetComponent(rViewType);
            if (rView == null)
            {
                rView = rGameObject.AddComponent(rViewType);
            }

            var rViewSerializedViewObject = new SerializedObject(rView);
            
            var rViewGoSerializedProp = rViewSerializedViewObject.FindProperty("ViewGo");
            rViewGoSerializedProp.objectReferenceValue = rGameObject;

            var rCanvasSerializedProp = rViewSerializedViewObject.FindProperty("Canvas");
            rCanvasSerializedProp.objectReferenceValue = rGameObject.GetComponent<Canvas>();

            var rCanvasGroupSerializedProp = rViewSerializedViewObject.FindProperty("CanvasGroup");
            rCanvasGroupSerializedProp.objectReferenceValue = rGameObject.GetComponent<CanvasGroup>();

            foreach (var rBindingVariablePair in this.mViewBindingVariables)
            {
                var rSerializedProp = rViewSerializedViewObject.FindProperty(rBindingVariablePair.Value.VariableName);
                rSerializedProp.objectReferenceValue = rBindingVariablePair.Key;
            }
            rViewSerializedViewObject.ApplyModifiedProperties();
        }
    }
}