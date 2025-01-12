using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ModiBuff.Core;

public class ModifierRecipes : ModiBuff.Core.ModifierRecipes
{
    public ModifierRecipes(ModifierIdManager idManager) : base(idManager)
    {
        //CreateGenerators 함수를 호출하여 설정을 완료해야 합니다
        //(기본 클래스는 우리가 오버라이드하는 SetupRecipes 함수를 사용합니다)
        //이는 생성자에서 수행하거나 클래스 외부에서 수행할 수 있습니다
        CreateGenerators();
    }

    protected override void SetupRecipes()
    {
        // //이것이 수정자를 정의하는 방법입니다. Add 메서드는 새로운 레시피를 제공합니다
        // //이름을 지정한 다음 기능을 추가해야 합니다

        // //이것은 가장 기본적인 수정자입니다
        // //이 수정자가 유닛에 추가되면 5의 데미지를 입힙니다
        // //표시 이름(Light Blow)과 설명은 선택사항이며 UI/UX에 사용됩니다
        // Add("InitDamage", "Light Blow", "Description")
        //     //EffectOn.Init는 수정자가 추가될 때마다 효과가 발동됨을 의미합니다
        //     .Effect(new DamageEffect(5), EffectOn.Init);

        // //이것은 전형적인 DoT(지속 데미지) 수정자의 예시입니다
        // //이 수정자가 유닛에 추가되면 매초마다 2의 데미지를 입힙니다
        // //5초 후에는 자동으로 제거됩니다
        // //refresh 메서드는 수정자가 다시 추가될 경우 제거 타이머가 초기화됨을 의미합니다
        // Add("DoT", "Poison Dart", "대상을 5초간 중독시켜 지속적으로 데미지를 입히며, 갱신 가능")
        //     //매 초마다 효과 발동
        //     .Interval(1)
        //     .Effect(new DamageEffect(2), EffectOn.Interval)
        //     //5초 후 제거, 수정자가 다시 추가되면 타이머 초기화
        //     .Remove(5).Refresh();

        // Add("FireSlimeSelfDoT", "Fireball", "대상을 5초간 화상시켜 지속적으로 데미지를 입히며, 갱신 가능")
        //     .Interval(1)
        //     .Effect(new DamageEffect(1), EffectOn.Interval);

        // //여기서는 새로운 효과와 수정자가 적용될 확률을 소개합니다
        // Add("DisarmChance", "Disarm", "대상을 1초간 무장해제시키며, 20% 확률로 적용")
        //     //수정자를 적용할 때(공격이나 시전을 통해)
        //     //20% 확률로 유닛에 수정자가 적용됩니다
        //     .ApplyChance(0.2f)
        //     //적용 시 대상 유닛을 1초간 무장해제(공격 불가)시킵니다
        //     .Effect(new StatusEffectEffect(StatusEffectType.Disarm, 1f), EffectOn.Init)
        //     .Remove(1f).Refresh();

        // Add("InitHeal", "Healing Touch", "대상을 치유합니다")
        //     .Effect(new HealEffect(5), EffectOn.Init);
    }
}