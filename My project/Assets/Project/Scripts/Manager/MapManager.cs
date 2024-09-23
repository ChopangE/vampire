using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class MapManager : MMSingleton<MapManager>
    {
        public Bounds GetMapBounds() => GameManager.Instance.CurStageBounds();
    }
}