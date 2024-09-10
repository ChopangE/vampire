using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;
using UnityWeld;
using UnityWeld.Binding;
namespace UI
{
    [Binding]
    public class LevelStarsViewModel : ViewModel
    {
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