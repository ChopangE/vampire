using System;
using System.Collections.Generic;

namespace Manager
{
    public class Storage
    {
        public Storage()
        {
            GoldData = "0";
            upgradeDataList = new List<Data.UpgradeData>();
        }
        // public HashSet<int> CollectedItem = new();
        public string GoldData;
        public List<Data.UpgradeData> upgradeDataList;
    }
    public partial class UserDataManager
    {
        public Storage storage { get; private set; } = new Storage();
        private string KeyName = "Storage";
        private string FileName = "SaveFile.es3";
        /// <summary>
        /// TODO : 
        /// 1. 스팀 서버 또는 로컬 스토리지에서 클래스를 읽음 (비동기)
        /// 2. 클래스를 UserDataManager에 저장함
        /// 3. 필요한 시점마다 스팀 서버 또는 로컬 스토리지에 세이브 데이터를 저장함 (비동기)
        /// </summary>
        private ES3Settings _saveSettings = new(ES3.Location.File);
        /// <summary>
        /// 게임 켤 때 한 번 호출
        /// </summary>
        public void Load()
        {
            if(ES3.FileExists(FileName))
                ES3.LoadInto(KeyName, storage, _saveSettings);
            else
                Save();
        }
        public void Save()
        {
            ES3.Save(KeyName, storage, _saveSettings);
        }
        
        /// <summary>
        /// 게임 끌 때 한 번 호출
        /// </summary>
        private void OnApplicationQuit() {
            Save();
        }
        private void OnApplicationPause(bool pauseStatus) {
            if(pauseStatus)
            {
                Save();
            }   
        }
    }
}