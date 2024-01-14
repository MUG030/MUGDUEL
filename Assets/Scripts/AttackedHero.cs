using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
// 場に置かれた時にフィールドが反映するように，攻撃された側が攻撃処理を行う
public class AttackedHero : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */
        // attakerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // defenderカードを選択(Playerフィールドから選択)
        CardController defender = GetComponent<CardController>();

        if (attacker == null)
        {
            return;
        }
        if (attacker.cardModel.canAttack)
        {
            // attakerとHeroに攻撃する
            GameManager.instance.AttackToHero(attacker, true);    // CardsBattle(attacker, defender);だとGameManagerの関数をpublicにしても呼び出せない
            GameManager.instance.CheckHeroHp();
        }
        
    }
}
