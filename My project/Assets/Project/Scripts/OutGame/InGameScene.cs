using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UI.Page;

namespace OutGame
{
    public class InGameScene : SceneBase
    {
        protected override void Start()
        {
            int stage = GameManager.Instance.CurStage;
            switch (stage)
            {
                case 3:
                    bGMEnum = Data.BGMEnum.TheOddsAreAgainstUs; // 마법사
                    break;
                case 7:
                    bGMEnum = Data.BGMEnum.Showdown; // 드래곤
                    break;
                case 11:
                    bGMEnum = Data.BGMEnum.TheDarkestNightAddedFunk; // 골렘
                    break;
                case 12:
                    bGMEnum = Data.BGMEnum.AnnulusLoop; // 마녀 1 phase
                    break;
                default:
                    if (stage >= 0 && stage <= 2)
                        bGMEnum = Data.BGMEnum.FranticBattleLoop;
                    else if (stage >= 4 && stage <= 6)
                        bGMEnum = Data.BGMEnum.SeaBattleLoop;
                    else if (stage >= 8 && stage <= 10)
                        bGMEnum = Data.BGMEnum.HeavyCombatMysticMelee;
                    else
                        bGMEnum = Data.BGMEnum.NONE;
                    break;
            }
            isLoop = true;
            base.Start();
        }
    }
}
