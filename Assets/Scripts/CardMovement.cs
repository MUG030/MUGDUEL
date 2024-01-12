using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaltParent;

    public bool isDraggable;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // カードのコストとPlayerのManaコストを比較する
        CardController card = GetComponent<CardController>();
        if (!card.cardModel.isFieldCard && card.cardModel.cost <= GameManager.instance.playerManaCost)
        {
            isDraggable = true;
        }
        else if (card.cardModel.isFieldCard && card.cardModel.canAttack)
        {
            isDraggable = true;
        }
        else
        {
            isDraggable = false;
        }

        if (!isDraggable)
        {
            return;
        }
        defaltParent = transform.parent;                    // 自分自身の親を取得する
        transform.SetParent(defaltParent.parent, false);    // 親の親（Canvas）を取得する
        GetComponent<CanvasGroup>().blocksRaycasts = false; // ドラッグする時はレイキャストをブロックしない
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }

        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable)
        {
            return;
        }

        transform.SetParent(defaltParent, false);           // 離した時に親に戻る
        GetComponent<CanvasGroup>().blocksRaycasts = true;  // ドラッグ後はレイキャストをブロックする
    }

    public void SetCardTransform(Transform parentTransform)
    {
        defaltParent = parentTransform;
        transform.SetParent(defaltParent);
    }
}
