using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SO;
using UnityWeld;
using UnityWeld.Binding;
using Unity.VisualScripting;
namespace UI
{
    [Binding]
    public class LevelStarsGroup : GroupView
    {
        [SerializeField] private Sprite UnStarSprite;
        [SerializeField] private Sprite StarSprite;

        private int _unStarCount;
        [Binding]
        public int UnStarCount
        {
            get => _unStarCount;
            set
            {
                _unStarCount = value;
                OnPropertyChanged(nameof(UnStarCount));
            }
        }
        private int _starCount;
        [Binding]
        public int StarCount
        {
            get => _starCount;
            set
            {
                _starCount = value;
                OnPropertyChanged(nameof(StarCount));
            }
        }
        public void SetStarCount(int unstarCount, int starCount)
        {
            UnStarCount = unstarCount;
            StarCount = starCount;

            PrepareViewModels(UnStarCount);
            var stars = GetViewModels();
            for(int i = 0; i < stars.Count; i++) {
                var s = stars[i] as LevelStarsViewModel;
                if(i+1 <= starCount)
                    s.Icon = StarSprite;
                else
                    s.Icon = UnStarSprite;
            }

            gameObject.SetActive(true);
        }
    }
}