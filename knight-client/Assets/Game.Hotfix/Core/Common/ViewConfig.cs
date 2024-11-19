using Knight.Framework.UI;
using System.Collections.Generic;

namespace Game
{
    public class ViewConfig : ViewAssetConfig
    {
        protected override void OnInitialize()
        {
            this.mViewModuleConfigs = new Dictionary<string, ViewModuleConfig>()
            {
                { 
                    "Login", 
                    new ViewModuleConfig() 
                    { 
                        Name = "Login",
                        ViewState = ViewState.Fixed,
                        ViewConfigs = new Dictionary<string, Knight.Framework.UI.ViewConfig>()
                        {
                            { "UILogin", new Knight.Framework.UI.ViewConfig() { Name = "UILogin",  Layer = ViewLayer.Fixed } }
                        }
                    } 
                }
            };
        }
    }
}
