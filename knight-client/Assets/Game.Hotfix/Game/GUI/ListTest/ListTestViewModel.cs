using Knight.Core;
using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Game
{
    [DataBinding]
    public class ListDataItem : ViewModel
    {
        private string                          mValue1;
        private string                          mValue2;

        [DataBinding]
        public string Value1
        {
            get { return mValue1;       }
            set { mValue1 = value;      }
        }

        [DataBinding]
        public string Value2
        {
            get { return mValue2;       }
            set { mValue2 = value;      }
        }
    }

    [DataBinding]
    public class ListTestViewModel : ViewModel
    {
        private ObservableList<ListDataItem>    mItemDatas;

        [DataBinding]
        public  ObservableList<ListDataItem>    ItemDatas
        {
            get { return mItemDatas;    }
            set
            {
                mItemDatas = value;
                this.PropChanged("ItemDatas");
            }
        }
    }
}
