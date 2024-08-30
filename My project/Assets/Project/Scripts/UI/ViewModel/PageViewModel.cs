using Manager;
using UI.Page;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class PageViewModel : BaseViewModel
    {
        [Binding]
        public void ClosePage()
        {
            Global.UIManager.ClosePage();
        }
        [Binding]
        public void OnClickShopButton()
        {
            Global.UIManager.OpenPage<ShopPage>();
        }
    }
}