using Knight.Framework.UI;
using UnityEngine.UI;

namespace Game.Test
{
    public class FancyCellItemData : FancyScrollCellBridge
    {
        public Text NameText;
        public Text PasswordText;

        public ViewModel ViewModel;
        
        public override void Initialize()
        {
        }

        public override void UpdateContent(int nIndex, FancyScrollCellData rViewModel)
        {
            if (this.ViewModel != rViewModel.ViewModel)
            {
                var rOldItemDataTest = this.ViewModel as ItemDataTestViewModel;
                if (rOldItemDataTest!= null)
                {
                    rOldItemDataTest.UnregisterPropertyChangeHandler<string>("Name", this.OnNameChanged);
                    rOldItemDataTest.UnregisterPropertyChangeHandler<string>("Password", this.OnPasswordChanged);
                }
                var rNewItemDataTest = rViewModel.ViewModel as ItemDataTestViewModel;
                rNewItemDataTest.RegisterPropertyChangeHandler<string>("Name", this.OnNameChanged);
                rNewItemDataTest.RegisterPropertyChangeHandler<string>("Password", this.OnPasswordChanged);

                this.ViewModel = rViewModel.ViewModel;
            }

            var rItemData = rViewModel.ViewModel as ItemDataTestViewModel;
            this.OnNameChanged(rItemData.Name);
            this.OnPasswordChanged(rItemData.Password);
        }

        private void OnNameChanged(string rValue)
        {
            this.NameText.text = rValue;
        }

        private void OnPasswordChanged(string rValue)
        {
            this.PasswordText.text = rValue;
        }
    }
}