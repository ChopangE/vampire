using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class TrainingTooltipPanelViewModel : TooltipPanel
    {
        public override void Show(string text, RectTransform triggeredBy)
        {
            base.Show(text, triggeredBy);
            var infos = triggeredBy.GetComponent<PassiveButtonViewModel>();
            Name = infos.PassiveName;
            Cost = infos.PassiveCost;
            Icon = infos.Icon;
            Info = infos.PassiveInfo;
        }
        private string _name;
        [Binding]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _cost;
        [Binding]
        public string Cost
        {
            get => _cost;
            set
            {
                _cost = value;
                OnPropertyChanged(nameof(Cost));
            }
        }
        private Sprite _icon;
        [Binding]
        public Sprite Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }
    }
}