using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Manager;
using UI.Page;
using UnityWeld;
using UnityWeld.Binding;
using Debug = UnityEngine.Debug;

namespace UI.Page
{
    [Binding]
    public class MapPageNew : PageViewModel
    {
        private int _coin;

        [Binding]
        public int Coin
        {
            get => _coin;
            set
            {
                _coin = value;
                OnPropertyChanged(nameof(Coin));
            }
        }
        private List<GroupView> mapPageViews = new List<GroupView>();
        private void OnEnable()
        {
            var allChildrenPages = GetComponentsInChildren<GroupView>();
            // 자기 자신의 경우엔 무시 
            foreach (var child in allChildrenPages)
            {
                if (child.transform.name != transform.name) mapPageViews.Add(child);
            }

            OnClickStageMapPageButton();
        }
        private void OnDisable()
        {
            foreach (var view in mapPageViews)
            {
                if (view as TrainingView)
                {
                    view.gameObject.SetActive(false);
                }
            }
        }
        [Binding]
        public void OnClickTraingButton()
        {
            foreach (var view in mapPageViews)
            {
                if (view as TrainingView)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
        }
        [Binding]
        public void OnClickStageMapPageButton()
        {
            foreach (var view in mapPageViews)
            {
                if (view as StageMapView)
                {
                    view.gameObject.SetActive(true);
                }
                else view.gameObject.SetActive(false);
            }
        }
    }
}