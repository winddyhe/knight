using Cysharp.Threading.Tasks;
using Knight.Core;
using Knight.Framework.UI;
using System.Collections.Generic;

namespace Game
{
    [DataBinding]
    public partial class ViewModelTest : ViewModel
    {
        [DataBinding]
        public string Name { get; set; }
        [DataBinding]
        public int Age { get; set; }  
    }

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

    [DataBinding]
    public partial class PlayerTestItem : ViewModel
    {
        [DataBinding]
        public string Test1 { get; set; }
        [DataBinding]
        public string Test2 { get; set; }

        // Related Binding with single property
        [DataBindingRelated("Test2")]
        public string Test1RelatedTest2 => this.Test2 + "_RelatedTest2";
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

            //this.mNetworkMsgProtoTest = NetworkHotfixManager.Instance.RegisterServerHandler<AddressBook>(
            //    GAME_CMD.GAME_CMD_LOGIN,
            //    GAME_CMD_LOGIN.GAME_CMD_LOGIN_PLAYER_DATA_S,
            //    this.NetworkMsgHandle_Test1);
        }

        protected override void OnClose()
        {
            //NetworkHotfixManager.Instance.UnregisterServerHandler(this.mNetworkMsgProtoTest);
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

        //public void NetworkMsgHandle_Test1(IMessage rBodyBytes)
        //{
        //    var rMessageProto = rBodyBytes as AddressBook;
        //    this.Test2.Name.Value = rMessageProto.People[0].Name;
        //}
    }
}
