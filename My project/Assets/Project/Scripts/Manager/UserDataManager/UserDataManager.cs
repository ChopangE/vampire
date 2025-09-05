using System;
using System.Collections.Generic;
#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
using CloudSave = HeathenEngineering.SteamworksIntegration.API.RemoteStorage.Client;
#endif

namespace Manager
{
    public class Storage
    {
        public Storage()
        {
            GoldData = "0";
            upgradeDataList = new List<Data.UpgradeData>();
            itemDataInfoList = new List<ItemDataInfo>();
            passiveItemDataInfoList = new List<PassiveItemDataInfo>();
            purchasedShopItems = new Dictionary<string, bool>();
            curStage = 0;
            level = 0;
            evolutionStages = new Dictionary<string, int>();
            killSaveData = new Dictionary<string, int>();
        }
        // public HashSet<int> CollectedItem = new();
        public string GoldData;
        public List<Data.UpgradeData> upgradeDataList;
        public List<ItemDataInfo> itemDataInfoList;
        public List<PassiveItemDataInfo> passiveItemDataInfoList;
        public Dictionary<string, bool> purchasedShopItems;
        public int curStage;
        public int level;
        public Dictionary<string, int> evolutionStages;
        public Dictionary<string, int> killSaveData;
    }
    public partial class UserDataManager

    {
        public Storage storage { get; private set; } = new Storage();
        private string KeyName = "Storage";
        private string CloudFileName = "SaveFile.json"; // Steam Cloud용 파일명
        private string LocalFileName = "SaveFile.es3";  // 로컬 ES3용 파일명
        /// <summary>
        /// TODO : 
        /// 1. 스팀 서버 또는 로컬 스토리지에서 클래스를 읽음 (비동기)
        /// 2. 클래스를 UserDataManager에 저장함
        /// 3. 필요한 시점마다 스팀 서버 또는 로컬 스토리지에 세이브 데이터를 저장함 (비동기)
        /// </summary>
        private ES3Settings _localSaveSettings;

        public UserDataManager()
        {
            // ES3 로컬 저장 설정 (Steam Cloud와 호환되도록 JSON 형식 사용)
            _localSaveSettings = new ES3Settings(ES3.Location.File)
            {
                path = LocalFileName,
                format = ES3.Format.JSON,
                prettyPrint = false  // 파일 크기를 줄이기 위해 false
            };
        }
        
        /// <summary>
        /// 스팀 클라우드 사용 여부 확인
        /// </summary>
        public bool IsSteamCloudAvailable
        {
            get
            {
#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
                return CloudSave.IsEnabled;
#else
                return false;
#endif
            }
        }

#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
        /// <summary>
        /// Steam Cloud 저장소 용량 확인
        /// </summary>
        public (ulong total, ulong remaining) GetCloudQuota()
        {
            CloudSave.GetQuota(out ulong total, out ulong remaining);
            return (total, remaining);
        }

        /// <summary>
        /// Steam Cloud에 저장할 수 있는지 용량 확인
        /// </summary>
        private bool CanSaveToCloud()
        {
            var (total, remaining) = GetCloudQuota();
            var estimatedSize = System.Text.Encoding.UTF8.GetByteCount(UnityEngine.JsonUtility.ToJson(storage));
            return remaining > estimatedSize;
        }
#endif
        
        /// <summary>
        /// 게임 켤 때 한 번 호출
        /// </summary>
        public void Load()
        {
#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
            // 스팀 클라우드가 사용 가능하면 클라우드에서 로드
            if (IsSteamCloudAvailable)
            {
                LoadFromSteamCloud();
                return;
            }
#endif
            // 스팀 클라우드를 사용할 수 없으면 로컬에서 로드
            LoadFromLocal();
        }
        
        public void Save()
        {
#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
            // 스팀 클라우드가 사용 가능하면 클라우드에 저장
            if (IsSteamCloudAvailable)
            {
                SaveToSteamCloud();
                return;
            }
#endif
            // 스팀 클라우드를 사용할 수 없으면 로컬에 저장
            SaveToLocal();
        }

#if !DISABLESTEAMWORKS && HE_SYSCORE && STEAMWORKSNET
        /// <summary>
        /// 스팀 클라우드에서 데이터 로드
        /// </summary>
        private void LoadFromSteamCloud()
        {
            try
            {
                if (CloudSave.FileExists(CloudFileName))
                {
                    storage = CloudSave.FileReadJson<Storage>(CloudFileName, System.Text.Encoding.UTF8);
                    UnityEngine.Debug.Log("Data loaded from Steam Cloud");
                }
                else
                {
                    SaveToSteamCloud(); // 파일이 없으면 새로 생성
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to load from Steam Cloud: {e.Message}");
                LoadFromLocal(); // 실패시 로컬에서 로드 시도
            }
        }

        /// <summary>
        /// 스팀 클라우드에 데이터 저장
        /// </summary>
        private void SaveToSteamCloud()
        {
            try
            {
                // 용량 체크
                if (!CanSaveToCloud())
                {
                    UnityEngine.Debug.LogWarning("Not enough space in Steam Cloud, saving locally");
                    SaveToLocal();
                    return;
                }

                bool success = CloudSave.FileWrite(CloudFileName, storage, System.Text.Encoding.UTF8);
                if (success)
                {
                    UnityEngine.Debug.Log("Data saved to Steam Cloud");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Failed to save to Steam Cloud, trying local save");
                    SaveToLocal(); // 실패시 로컬에 저장
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to save to Steam Cloud: {e.Message}");
                SaveToLocal(); // 실패시 로컬에 저장
            }
        }
#endif

        /// <summary>
        /// 로컬에서 데이터 로드 (백업용)
        /// </summary>
        private void LoadFromLocal()
        {
            if(ES3.FileExists(LocalFileName))
                ES3.LoadInto(KeyName, storage, _localSaveSettings);
            else
                SaveToLocal();
        }

        /// <summary>
        /// 로컬에 데이터 저장 (백업용)
        /// </summary>
        private void SaveToLocal()
        {
            ES3.Save(KeyName, storage, _localSaveSettings);
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

        // 상점 구매 항목 초기화 메서드
        public void ResetPurchasedShopItems()
        {
            storage.purchasedShopItems.Clear();
            Save();
        }
        public void ResetPassiveItemData()
        {
            storage.passiveItemDataInfoList.Clear();
            Save();
        }
        public void ResetStageData()
        {
            curStage = 0;
            level = 0;
        }
    }
}