using Game.Test;
using Knight.Framework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace FancyScrollView.Example09
{
    class Example10 : MonoBehaviour
    {
        public List<FancyScrollCellData> itemData;
        public List<ItemDataTestViewModel> itemDataViewModel;

        [SerializeField] FancyScrollRectView scrollView = default;

        void Start()
        {
            this.itemData = new List<FancyScrollCellData>();
            this.itemDataViewModel = new List<ItemDataTestViewModel>();
            for (int i = 0; i < 10; i++)
            {
                var rItemDataTest = new FancyScrollCellData();

                var rItemDataTestViewModel = new ItemDataTestViewModel();
                rItemDataTestViewModel.Name = ("Name " + i);
                rItemDataTestViewModel.Password =("Password " + i);
                rItemDataTest.ViewModel = rItemDataTestViewModel;

                this.itemDataViewModel.Add(rItemDataTestViewModel);
                this.itemData.Add(rItemDataTest);
            }

            this.scrollView.UpdateData(this.itemData);
        }

        public void BtnClickTest1()
        {
            this.itemDataViewModel[4].Name = ("Name 4444");
        }
    }
}
