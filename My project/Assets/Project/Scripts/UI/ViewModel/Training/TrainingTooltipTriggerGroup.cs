using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TrainingTooltipTriggerGroup : TooltipTriggerGroup
    {
        protected override void ShowTooltip(string _text, RectTransform _rectTransform)
        {
            if (tooltipPanel != null)
            {
                var trainingPanel = tooltipPanel as TrainingTooltipPanelViewModel;
                trainingPanel.Show(_text, _rectTransform);
            }
        }
    }
}