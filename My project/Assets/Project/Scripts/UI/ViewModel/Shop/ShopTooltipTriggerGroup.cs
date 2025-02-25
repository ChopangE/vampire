using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ShopTooltipTriggerGroup : TooltipTriggerGroup
    {
        protected override void ShowTooltip(string _text, RectTransform _rectTransform)
        {
            if (tooltipPanel != null)
            {
                var shopPanel = tooltipPanel as ShopTooltipPanelViewModel;
                shopPanel.Show(_text, _rectTransform);
            }
        }
    }
}