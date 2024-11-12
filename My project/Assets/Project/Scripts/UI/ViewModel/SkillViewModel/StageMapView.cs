using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine.SceneManagement;
using UnityWeld;
using UnityWeld.Binding;

namespace UI
{
    [Binding]
    public class StageMapView : GroupView
    {
        [Binding]
        public void CallScene()
        {
            SceneManager.LoadScene("LoadingScene");
        }
    }
}