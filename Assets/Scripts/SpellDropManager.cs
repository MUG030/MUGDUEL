using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
// 場に置かれた時にフィールドが反映するように，攻撃された側が攻撃処理を行う
public class SpellDropManager : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardController spellCard = eventData.pointerDrag.GetComponent<CardController>();
        CardController targetCard = GetComponent<CardController>();   // nullの可能性あり

        if (spellCard == null)
        {
            return;
        }
        // スペルカードをドラッグできるか判定
        if (!spellCard.cardMovement.isDraggable)
        {
            return;
        }
        if (spellCard.CanUseSpell())
        {
            spellCard.UseSpellTo(targetCard);
        }
    }
}
