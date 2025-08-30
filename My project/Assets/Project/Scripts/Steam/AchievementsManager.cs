using System.Collections.Generic;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public SteamworksBehaviour steamworksBehaviour;

    // AchievementData를 써도 됨 
    // public AchievementData a;
    //* 아니면 AchievementObject 스크립터블 오브젝트 써도 됨
    //* 이건 AchievementData 구조체를 Unity의 ScriptableObject로 나타냄
    //* 이를 통해 스크립트에서 업적을 참조하고, 업적을 드래그 앤 드롭 가능
    //* 또한, 업적이 잠금 상태인지 해제 상태인지를 관리하는 UnityEvent를 처리 가능
    public List<AchievementObject> achievementObjects;

    //* +추가) 업적 패널에 표기할 것들
    void Awake()
    {
        if (steamworksBehaviour == null)
        {
            steamworksBehaviour = FindAnyObjectByType<SteamworksBehaviour>();
        }
    }

    //* SteamworkBehaviour이 초기화 되면 업뎃하기
    public void UpdateAchievements()
    {
        achievementObjects = steamworksBehaviour.settings.achievements;
        for (int i = 0; i < achievementObjects.Count; i++)
        {
            if (achievementObjects[i] != null)
            {
                achievementObjects[i].StatusChanged?.Invoke(achievementObjects[i].IsAchieved);
                Debug.Log(achievementObjects[i].Id);

            }
        }
    }
    public void AchievementTrue(AchievementObject achievementObject)
    {
        achievementObject.IsAchieved = true;
    }
    public void AchievementReset(AchievementObject achievementObject)
    {
        achievementObject.IsAchieved = false;
    }

    public void AchivementTrueByID(string id)
    {
        AchievementData myAch = id;
        myAch.Unlock();
        myAch.Store();
    }
}