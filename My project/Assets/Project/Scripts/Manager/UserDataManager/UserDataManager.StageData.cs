using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public partial class UserDataManager
    {
        private int _curStage;
        public int curStage { get => _curStage; set => _curStage = value; }
    }
}