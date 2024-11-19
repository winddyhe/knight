# UI Framework of MVMC
* This framework is a lightweight UI framework based on MVVM data binding, achieving complete separation between logical data and display data. It integrates the idea of separating data and logic in MVC, forming a unique MVMC UI framework.
* Developers only need to focus on the binding between data and the interface, as well as implementing logical data to automatically drive the display of the UI.
Implement the interaction logic between the interface ViewModel and data Model in the controller ViewController to achieve the separation of data and logic.
* A ViewManager is provided for managing the hierarchy of UIs and handling opening and closing operations, incorporating caching of interface information and fallback functionality.
* Provides a UGUI-based atlas management module: UIAtlasManager.
* Provides a series of commonly used UGUI extension components.

## UI Data Binding
### ViewModel container (View)
* ![ui_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_1.png)
Each UI prefab contains a ViewModel container script (a script inheriting from View). This script automatically generates data binding code when the prefab is saved, completely eliminating the need for data binding through reflection.
* When initializing the prefab, View. Initialize(ViewController) and View. Bind() are called to perform data binding between the View controller and the ViewModel.

### ViewModel Data Source (ViewModelDataSource)
* ![ui_2](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_2.png)
* This class is used to associate data with the UI, and it is stored in the ViewModelContainer.
* Create a ViewModel object by reflecting the ViewModelPath.

### One-way binding (MemberBindingOneWay)
* ![ui_3](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_3.png)
* ViewPath variable: Automatically identifies all variables exposed to the outside world in all components under the current node.
* ViewModelPath variable: Automatically locate the variable in our customized ViewModel class based on the type of ViewPath.
* By utilizing these two variables, the effect of binding data with the UI can be achieved.

### Two-way binding (MemberBindingTwoWay)
* ![ui_4](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_4.png)
* EventPath variable: Automatically identifies the event-triggered interfaces exposed by all components in the current node, enabling the monitoring of component value changes through event binding.
* View Path variable: Automatically identify all variables exposed to the outside world in all components under the current node.
* ViewModelPath variable: Automatically locate the variable in our customized ViewModel class based on the type of ViewPath.

### Event Binding
* ![ui_5](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_5.png)
* ViewEvent variable: Automatically identifies the event-triggered interfaces exposed by all components in the current node, enabling the monitoring of component value changes through event binding.
* ViewModelMethod: Automatically identifies all methods in the ViewModel that contain binding tags.

### Hot update logic side
* By adding the DataBinding attribute tag to the logic classes, property variables, and methods in the hot update section, you can add them under the Inspector.
* In addition, the DataBindingReleated property is provided, which facilitates the use of relative dependency variables, such as the NameRelatedTest1 variable. If the value of Name changes, NameRelatedTest1 will also notify the display of the UI interface layer accordingly.
By adding the ViewModelKey attribute tag to the variables in PlayerViewController, the corresponding ViewModel values in ViewModelContainer can be automatically bound.
* Marking a method in PlayerViewController with the attribute DataBindingEvent indicates that this method can be used as a method for event binding in the Inspector.
```C#
    [DataBinding]
    public partial class PlayerViewModel : ViewModel    
    {
        // Data Binding
        [DataBinding]
        public string Name { get; set; }
        [DataBinding]
        public int Level { get; set; }
        [DataBinding]
        public int Exp { get; set; }
        [DataBinding]
        public int Coin { get; set; }
        [DataBinding]
        public string Password { get; set; }
        // Related Binding with single property
        [DataBindingRelated("Name")]
        public string NameRelatedTest1 => this.Name + "_RelatedTest1";
        // Related Binding with multiple properties
        [DataBindingRelated("Level, Exp")]
        public string LevelRelatedTest1 => (this.Level + this.Exp).ToString();
        // List Binding
        [DataBinding]
        public List<PlayerTestItem> TestList { get; set; }
    }
    public class PlayerViewController : ViewController
    {
        [ViewModelKey("Test1")]
        public PlayerViewModel Test1;
        [ViewModelKey("Test2")]
        public PlayerViewModel Test2;
        protected override async UniTask OnOpen()
        {
            await base.OnOpen();
            this.Test1.Name = "Test444.";
            this.Test1.Level = 100;
            this.Test1.Exp =200;
            this.Test1.Coin = 300;
            var rPlayerTestItems = new List<PlayerTestItem>();
            for (int i = 0; i < 100; i++)
            {
                var rPlayerTestItem = new PlayerTestItem();
                rPlayerTestItem.Test1 = $"Test1-{i}";
                rPlayerTestItem.Test2 = $"Test2-{i}";
                rPlayerTestItems.Add(rPlayerTestItem);
            }
            this.Test1.TestList = rPlayerTestItems;
        }
        protected override void OnClose()
        {
        }
        [DataBindingEvent(false)]
        public void OnBtnEnter_Clicked()
        {
            this.Test1.TestList[4].Test1 = this.Test1.Name;
            this.Test1.Exp = 400;
            LogManager.LogError($"OnBtnEnter_Clicked Test..{this.Test2.Password}, {this.Test1.Name}");
            ViewManager.Instance.Close(this.View.GUID);
            HotfixBattle.Instance.Initialize().WrapErrors();
        }
        [DataBindingEvent(true)]
        public void OnListBtnComfirmClicked(int nIndex)
        {
            LogManager.LogError($"OnListButton_Clicked Test..{nIndex}, {this.Test1.TestList[nIndex].Test1}");
        }
    }
```