using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// カードを置ける場所を示す
public class DropPlace : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        CardController card = eventData.pointerDrag.GetComponent<CardController>();
        if (card != null)
        {
            card.cardMovement.defaltParent = this.transform;
            GameManager.instance.ReduceManaCost(card.cardModel.cost, true);
        }
    }
}
