using Knight.Framework;
using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ListTestView : ViewController
    {
        [HotfixBinding("ListTest")]
        public ListTestViewModel    ListTest;

        protected override async Task OnInitialize()
        {
            await base.OnInitialize();

            this.ListTest.ItemDatas = new Knight.Core.ObservableList<ListDataItem>();
            for (int i = 0; i < 10; i++)
            {
                this.ListTest.ItemDatas.Add(new ListDataItem()
                {
                    Value1 = (i * 11111).ToString(),
                    Value2 = i * 1111
                });
            }
        }

        [DataBinding]
        protected void OnBtnAdd_Clicked(EventArg rEventArg)
        {
            var rListItem = new ListDataItem() { Value1 = "hhh", Value2 = 300 };
            Debug.LogError(this.ListTest.ItemDatas.GetType());
            this.ListTest.ItemDatas.Insert(0, rListItem);
            this.ListTest.ItemDatas.Refresh();
        }

        [DataBinding]
        protected void OnBtnDelete_Clicked(EventArg rEventArg)
        {
            this.ListTest.ItemDatas.RemoveAt(0);
            this.ListTest.ItemDatas.Refresh();
        }

        [DataBinding]
        protected void OnListItem_Clicked(int nIndex, EventArg rEventArg)
        {
            Debug.LogError($"Item {nIndex} clicked.. {this.ListTest.ItemDatas[nIndex].Value1}");
        }
    }
}
