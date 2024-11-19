using System;
using Knight.Core.Editor;
using Knight.Core;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

namespace Knight.Framework.UI.Editor
{
    public class DataBindingListTemplateCellBridge : CodeGenerator
    {
        private GameObject mGameObject;
        private string mViewName;
        private Type mViewModelType;

        private Dictionary<Component, DataBindingCodeGeneratorProp> mDataBindingProps;

        private Dictionary<Component, DataBindingCodeGeneratorVariable> mViewBindingVariables;
        private Dictionary<string, DataBindingCodeGeneratorVariable> mViewModelBindingVariables;
        private Dictionary<DataBindingEvent, DataBindingCodeGenertorEvent> mViewBindingEvents;

        private List<DataBindingCodeGeneratorViewSetMethod> mViewSetMethods;
        private List<DataBindingCodeGeneratorViewEventMethod> mViewEventMethods;
        private List<DataBindingCodeGeneratorViewEventMethod> mViewBindingEventMethods;

        public GameObject GameObject => this.mGameObject;
        public Type ViewModelType => this.mViewModelType;

        public DataBindingListTemplateCellBridge(string rFilePath, GameObject rGameObject, string rViewName) 
            : base(rFilePath)
        {
            this.mGameObject = rGameObject;
            this.mViewName = rViewName;
            this.mDataBindingProps = new Dictionary<Component, DataBindingCodeGeneratorProp>();

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

            this.mViewBindingVariables.Clear();
            this.mViewModelBindingVariables.Clear();
            this.mViewBindingEvents.Clear();

            this.mViewSetMethods.Clear();
            this.mViewEventMethods.Clear();
            this.mViewBindingEventMethods.Clear();

            this.BuildDataBindingProps();

            var rCellItem = this.mGameObject.GetComponentInChildren<FancyScrollRectCellItem>();
            if (rCellItem == null)
            {
                Debug.LogErrorFormat("Can not find FancyScrollRectCellItem in {0}", this.mGameObject.name);
                this.StringBuilder
                    .T(1).AL("}");
                return;
            }
            this.mViewModelType = TypeResolveManager.Instance.GetType(rCellItem.DataSource.TemplatePath);
            if (this.mViewModelType == null)
            {
                Debug.LogErrorFormat("Can not find ViewModel Type {0}", rCellItem.DataSource.TemplatePath);
                this.StringBuilder
                    .T(1).AL("}");
                return;
            }

            this.StringBuilder
                .T(1).AL($"public class {this.mViewModelType.Name}CellBridge : FancyScrollCellBridge")
                .T(1).AL("{");

            this.GenerateAllVaribles();
            this.GenerateInitializeMethod();
            this.GenerateUpdateContentMethod();
            this.GenerateViewSetMethod();

            this.StringBuilder
                .T(1).AL("}");
        }

        private void GenerateFooter()
        {
            this.StringBuilder
                .A("}")
                .N();
        }

        private void BuildDataBindingProps()
        {
            var rAllDataBindingOneWays = this.mGameObject.GetComponentsInChildren<DataBindingOneWay>(true);
            foreach (var rDataBindingOneWay in rAllDataBindingOneWays)
            {
                if (!rDataBindingOneWay.IsListTemlate) continue;

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
                if (!rDataBindingTwoWay.IsListTemlate) continue;

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
                if (!rDataBindingEvent.IsListTemlate) continue;

                var rViewComp = rDataBindingEvent.GetViewComponent();
                if (rViewComp == null) continue;

                if (!this.mDataBindingProps.TryGetValue(rViewComp, out var rProp))
                {
                    rProp = new DataBindingCodeGeneratorProp(rViewComp);
                    this.mDataBindingProps.Add(rViewComp, rProp);
                }
                rProp.AddEvent(rDataBindingEvent);
            }
        }

        private void GenerateAllVaribles()
        {
            this.StringBuilder
                .T(2).AL("public int Index;")
                .T(2).AL("public ViewModel ViewModel;")
                .N();

            var rDataBindingCodeGeneratorViewModel = new DataBindingCodeGeneratorViewModel()
            {
                Key = "ListTemlate",
                ViewModelTypeName = this.mViewModelType.FullName,
                ViewModelName = this.mViewModelType.Name,
            };

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

                    var rViewModelVariable = this.BuildViewModelVariable(rOneWay.ViewModelPath, rDataBindingCodeGeneratorViewModel);
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

                    var rViewModelVariable = this.BuildViewModelVariable(rTwoWay.ViewModelPath, rDataBindingCodeGeneratorViewModel);
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
            }
            this.StringBuilder
                .N();
        }

        private void GenerateInitializeMethod()
        {
            this.StringBuilder
                .T(2).AL("public override void Initialize()")
                .T(2).AL("{");

            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                this.StringBuilder
                    .T(3).AL($"this.{rBindingVariable.VariableName} = new {rBindingVariable.VariableType}();");
                
                if (this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                {
                    if (!rBindingVariable.ViewModelVariable.IsReleated)
                    {
                        var rViewSetMethod = new DataBindingCodeGeneratorViewSetMethod();
                        rViewSetMethod.IsListTemlate = false;
                        rViewSetMethod.MethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                        rViewSetMethod.ViewVariableName = rViewVariable.VariableName;
                        rViewSetMethod.ArgType = rBindingVariable.ViewVariable.PropType;
                        rViewSetMethod.ViewPropName = rBindingVariable.ViewVariable.PropName;
                        rViewSetMethod.IsReleated = false;
                        this.mViewSetMethods.Add(rViewSetMethod);
                    }
                    else
                    {
                        for (int i = 0; i < rBindingVariable.ViewModelVariable.RelatedProps.Count; i++)
                        {
                            var rRelatedProp = rBindingVariable.ViewModelVariable.RelatedProps[i];
                            var rRelatedType = rBindingVariable.ViewModelVariable.RelatedTypes[i];
                            var rViewSetMethod = new DataBindingCodeGeneratorViewSetMethod();
                            rViewSetMethod.IsListTemlate = false;
                            rViewSetMethod.MethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}_Related_{rRelatedProp}";
                            rViewSetMethod.ViewVariableName = rViewVariable.VariableName;
                            rViewSetMethod.ViewPropName = rBindingVariable.ViewVariable.PropName;
                            rViewSetMethod.ArgType = rRelatedType;
                            rViewSetMethod.RelatedViewModelPropName = rRelatedProp;
                            rViewSetMethod.ViewModelPropName = rBindingVariable.ViewModelVariable.PropName;
                            rViewSetMethod.ViewModelBindableName = rBindingVariablePair.Value.VariableName;
                            rViewSetMethod.IsReleated = true;
                            this.mViewSetMethods.Add(rViewSetMethod);
                        }
                    }
                }
            }
            this.StringBuilder
                .T(2).AL("}")
                .N();
        }

        private void GenerateUpdateContentMethod()
        {
            this.StringBuilder
                .T(2).AL("public override void UpdateContent(int nIndex, FancyScrollCellData rCellData)")
                .T(2).AL("{")
                    .T(3).AL("if (this.ViewModel != rCellData.ViewModel)")
                    .T(3).AL("{");
            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                if (!rBindingVariable.ViewModelVariable.IsReleated)
                {
                    if (this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                    {
                        var rMethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                        this.StringBuilder
                            .T(4).AL($"if (this.{rBindingVariable.VariableName}.ViewModel != null)")
                                .T(5).AL($"this.{rBindingVariable.VariableName}.ViewModel.UnregisterPropertyChangeHandler<{rBindingVariable.ViewModelVariable.PropType}>")
                                    .T(6).AL($"(nameof({this.mViewModelType.FullName}.{rBindingVariable.ViewModelVariable.PropName}), this.{rMethodName});");
                    }
                }
                else
                {
                    if (this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                    {
                        for (int i = 0; i < rBindingVariable.ViewModelVariable.RelatedProps.Count; i++)
                        {
                            var rRelatedProp = rBindingVariable.ViewModelVariable.RelatedProps[i];
                            var rRelatedType = rBindingVariable.ViewModelVariable.RelatedTypes[i];
                            var rMethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}_Related_{rRelatedProp}";
                            this.StringBuilder
                                .T(4).AL($"if (this.{rBindingVariable.VariableName}.ViewModel != null)")
                                    .T(5).AL($"this.{rBindingVariable.VariableName}.ViewModel.UnregisterPropertyChangeHandler<{rRelatedType}>")
                                        .T(6).AL($"(nameof({this.mViewModelType.FullName}.{rRelatedProp}), this.{rMethodName});");
                        }
                    }
                }
            }

            this.StringBuilder
                                .N()
                                .T(4).AL($"var rNewItem = rCellData.ViewModel as {this.mViewModelType.FullName};");
            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                this.StringBuilder
                                .T(4).AL($"this.{rBindingVariable.VariableName}.ViewModel = rNewItem;");
            }
            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                if (this.mViewBindingVariables.TryGetValue(rBindingVariable.ViewVariable.ViewComp, out var rViewVariable))
                {
                    if (!rBindingVariable.ViewModelVariable.IsReleated)
                    {
                        var rMethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                        this.StringBuilder
                                .T(4).AL($"this.{rBindingVariable.VariableName}.ViewModel.RegisterPropertyChangeHandler<{rBindingVariable.ViewModelVariable.PropType}>")
                                    .T(5).AL($"(nameof({this.mViewModelType.FullName}.{rBindingVariable.ViewModelVariable.PropName}), this.{rMethodName});");
                    }
                    else
                    {
                        for (int i = 0; i < rBindingVariable.ViewModelVariable.RelatedProps.Count; i++)
                        {
                            var rRelatedProp = rBindingVariable.ViewModelVariable.RelatedProps[i];
                            var rRelatedType = rBindingVariable.ViewModelVariable.RelatedTypes[i];
                            var rMethodName = $"Set{rViewVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}_Related_{rRelatedProp}";
                            this.StringBuilder
                                .T(4).AL($"this.{rBindingVariable.VariableName}.ViewModel.RegisterPropertyChangeHandler<{rRelatedType}>")
                                    .T(5).AL($"(nameof({this.mViewModelType.FullName}.{rRelatedProp}), this.{rMethodName});");
                        }
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
                                .T(4).AL("// 双向绑定")
                                .T(4).AL($"this.{rViewVariable.VariableName}.{rViewEventMethod.EventName}.RemoveListener(this.{rViewEventMethod.MethodName});")
                                .T(4).AL($"this.{rViewVariable.VariableName}.{rViewEventMethod.EventName}.AddListener(this.{rViewEventMethod.MethodName});");
                }
            }
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
                                .N()
                                .T(4).AL($"this.{rBindingEvent.ViewVariableName}.{rBindingEvent.EventName}.RemoveListener(this.{rViewEventMethod.MethodName});")
                                .T(4).AL($"this.{rBindingEvent.ViewVariableName}.{rBindingEvent.EventName}.AddListener(this.{rViewEventMethod.MethodName});")
                                .N();
            }
            this.StringBuilder
                                .T(4).AL($"this.ViewModel = rCellData.ViewModel;")
                            .T(3).AL("}")
                            .N();

            this.StringBuilder
                            .T(3).AL("this.Index = nIndex;")
                            .N();

            this.StringBuilder
                            .T(3).AL($"var rItem = rCellData.ViewModel as {this.mViewModelType.FullName};");
            foreach (var rBindingVariablePair in this.mViewModelBindingVariables)
            {
                var rBindingVariable = rBindingVariablePair.Value;
                if (!rBindingVariable.ViewModelVariable.IsReleated)
                {
                    var rMethodName = $"Set{rBindingVariable.VariableName}_{rBindingVariable.ViewVariable.PropName}";
                    this.StringBuilder
                                .T(3).AL($"this.{rBindingVariable.VariableName}.ViewModel.{rBindingVariable.ViewModelVariable.PropName} = this.{rBindingVariable.VariableName}.ViewModel.{rBindingVariable.ViewModelVariable.PropName};");
                }
            }

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
                    this.StringBuilder
                        .T(2).AL($"private void {rViewSetMethod.MethodName}({rViewSetMethod.ArgType} rValue)")
                        .T(2).AL("{")
                            .T(3).AL($"this.{rViewSetMethod.ViewVariableName}.{rViewSetMethod.ViewPropName} = rValue;")
                        .T(2).AL("}")
                        .N();
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
                        .A($"rArg{0}")
                        .A(";").N();
                }

                this.StringBuilder
                    .T(2).AL("}")
                    .N();
            }

            // 组件事件
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
                        .T(3).AL($"var r{this.mViewName} = this.gameObject.GetComponentInParent<{this.mViewName}>();")
                        .T(3).A($"r{this.mViewName}?.{this.mViewName}Controller?.{rViewBindingEventMethod.ViewControllerEventName}")
                        .A($"(this.Index");
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

        private DataBindingCodeGeneratorViewModelVariable BuildViewModelVariable(string rViewModelPath, DataBindingCodeGeneratorViewModel rDataBindingCodeGeneratorViewModel)
        {
            if (string.IsNullOrEmpty(rViewModelPath)) return null;

            var rViewModelPathStrs = rViewModelPath.Split('/');
            if (rViewModelPathStrs.Length < 2) return null;

            var rKeyViewModelName = rViewModelPathStrs[0].Trim();
            var rKeyViewModelNameStrs = rKeyViewModelName.Split('@');

            if (rKeyViewModelNameStrs.Length < 2) return null;

            var rPropPath = rViewModelPathStrs[1].Trim();
            var rPropPathStrs = rPropPath.Split(':');
            if (rPropPathStrs.Length < 2) return null;

            var rPropName = rPropPathStrs[0].Trim();
            var rPropType = rPropPathStrs[1].Trim();

            var rViewModelVarible = new DataBindingCodeGeneratorViewModelVariable()
            {
                ViewModel = rDataBindingCodeGeneratorViewModel,
                PropName = rPropName,
                PropType = rPropType
            };            
            
            // 是否是Releated的ViewModel
            var rViewModelType = TypeResolveManager.Instance.GetType(rDataBindingCodeGeneratorViewModel.ViewModelTypeName);
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
            foreach (var rBindingVariablePair in this.mViewBindingVariables)
            {
                var rSerializedProp = rViewSerializedViewObject.FindProperty(rBindingVariablePair.Value.VariableName);
                rSerializedProp.objectReferenceValue = rBindingVariablePair.Key;
            }
            rViewSerializedViewObject.ApplyModifiedProperties();
        }
    }
}
