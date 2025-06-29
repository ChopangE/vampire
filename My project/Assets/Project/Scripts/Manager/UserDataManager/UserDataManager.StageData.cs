using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public partial class UserDataManager
    {
        public int curStage 
        { 
            get => storage.curStage;
            set
            {
                storage.curStage = value;
                Save();
            }
        }

        public int level 
        { 
            get => storage.level;
            set
            {
                storage.level = value;
                Save();
            }
        }
    }
}