using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// カードを置ける場所を示す
public class DropPlace : MonoBehaviour, IDropHandler
{
    public enum TYPE
    {
        HAND,
        FIELD,
    }
    public TYPE type;
    public void OnDrop(PointerEventData eventData)
    {
        if (type == TYPE.HAND)
        {
            return;         // 手札にドラッグ&ドロップされてもコスト計算フィールド移動はしない
        }
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            if (!card.cardMovement.isDraggable)
            {
                return;     // ドラッグできないカードは置けない
            }
            card.cardMovement.defaltParent = this.transform;
            if(card.cardModel.isFieldCard)
            {
                return;     // 既にフィールドにあるカードは置けない
            }
            card.OnField();    // フィールドにカードを出した時によぶ関数
        }
    }
}
