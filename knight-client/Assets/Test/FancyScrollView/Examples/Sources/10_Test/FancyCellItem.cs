using FancyScrollView;
using Knight.Framework.UI;
using UnityEngine;

namespace Game.Test
{
    [DataBinding]
    public class ItemDataTestViewModel : ViewModel
    {
        [DataBinding]
        public string Name { get; set; }
        [DataBinding]
        public string Password { get; set; }
    } 
}