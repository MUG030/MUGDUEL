using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
// 場に置かれた時にフィールドが反映するように，攻撃された側が攻撃処理を行う
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */
        // attakerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        CardController defender = GetComponent<CardController>();

        if (attacker == null || defender == null)
        {
            return;
        }
        if (attacker.cardModel.isPlayerCard == defender.cardModel.isPlayerCard)
        {
            return;
        }
        // defenderカードを選択(Playerフィールドから選択)
        // CardController defender = GetComponent<CardController>();

        CardController[] enemyFieldCards = GameManager.instance.GetEnemyFieldCards(attacker.cardModel.isPlayerCard);
        if (Array.Exists(enemyFieldCards, card => card.cardModel.ability == ABILITY.SHIELD) && defender.cardModel.ability != ABILITY.SHIELD)
        {
            Debug.Log("盾がある");
            return;
        }
        if (attacker.cardModel.canAttack)
        {
            Debug.Log("攻撃したよ！");
            // attakerとdefenderが戦う
            GameManager.instance.CardsBattle(attacker, defender);    // CardsBattle(attacker, defender);だとGameManagerの関数をpublicにしても呼び出せない
        }
        
    }
}
