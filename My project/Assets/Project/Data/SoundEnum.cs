using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Data
{
    public enum BGMEnum
    {
        NONE,
        TitleSpace, //* 타이틀 우주 소리
        TitleFireBurning, //* 타이틀 불 소리
    }
    public enum SFXEnum
    {
        NONE,
        
        // === 스킬 효과음 ===
        Staff_270Degree,        // 270도 봉술
        SpikeFloor,            // 가시 장판
        SwordWind,             // 검풍
        DaggerSwing,           // 단검 휘두르는 소리
        ThrowRock,             // 돌 던지기
        CatapultRockDrop,      // 돌 투석기 돌 떨어지는 소리
        RockWind_1,            // 돌풍 1
        RockWind_2,            // 돌풍 2
        RockWind_3,            // 돌풍 3
        RockWind_Short_4,      // 돌풍(짧) 4
        RockWind_Short_5,      // 돌풍(짧) 5
        WindShuriken,          // 바람 수리검
        WindFloor,             // 바람 장판
        StaffSkill,            // 봉술
        Clone,                 // 분신
        Shuriken,              // 수리검
        Tornado,               // 토네이도
        ExplosionCharm,        // 폭발 부적
        UltimateSwing,         // 필살기 스윙
        UltimateEarthquake,    // 필살기 컷신부터 스윙까지 재생할 땅 울리는 소리

        // === 행동 효과음 ===
        GetExpStone,           // 경험치 스톤 먹는 소리
        LevelUp,               // 레벨 업
        SkillSelectEvaluate,   // 레벨업 시 스크롤에서 스킬 3개 중 진화 스킬 선택
        SkillSelectOne,        // 레벨업 시 스크롤에서 스킬 3개중 1개 선택
        GetMapItem,            // 맵 드랍 아이템 먹는 소리
        Revive,                // 부활
        HGD_Death,             // HGD 죽는 소리
        HGD_Hit,               // HGD 피격 시

        // === 마녀 효과음 ===
        Witch_Crow_1,          // Crow 001
        Witch_Crow_2,          // Crow 002
        Witch_SpecialPunch,    // 독 장판 부글거리는 소리
        Witch_SpecialRemove,   // 독병 깨지는 소리
        Witch_MagicCharge,     // 마녀 주력 내려찍기
        Witch_FireTile_1,      // 불 타일 1
        Witch_FireTile_After,  // 불 타일 후보
        Witch_SpaceMove,       // 빗자루 스매시

        // === 스테이지1 마법사 효과음 ===
        DarkFireBall,          // 다크 파이어볼
        Blink,                 // 빈개
        IceBolt,              // 아이스볼트
        FrozenMelt,            // 얼음 떨어뜨리기

        // === 스테이지2 용 효과음 ===
        Dragon_Laser,          // 용 레이저
        Dragon_Craw,    // 용 발톱 할퀴기

        // === 스테이지3 골렘 효과음 ===
        Golem_Punch,           // 골렘 강펀치
        Golem_HandDown,        // 골렘 대지 흔들기

        // === 몬스터 효과음 ===
        Monster_Hit_1,         // 몬 피격 소리 1
        Monster_Hit_After,     // 몬 피격 소리 후보

        // === 상점 효과음 ===
        Shop_SingleBuy,        // 단약 사용 소리
        Shop_MultiBuy,         // 물약 사용 소리
        Shop_Button_1,         // 버튼음 1
        Shop_Button_After,     // 버튼음 후보
        Shop_ItemBuy,          // 상점 아이템 구매 소리
        Shop_StageOpen,        // 스테이지 클리어
        Shop_StageClose,       // 일반 몹 스테이지 입장
        Shop_BossStageClose,   // 중간 보스 스테이지 입장
        Shop_PageTurn,         // 책 페이지 넘기는 소리
        Shop_FinalBossStage,   // 최종 보스 마녀 스테이지 입장
        OpenButton,            // 오픈 버튼 촤라락
    }
}