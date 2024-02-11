using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaltParent;

    public bool isDraggable;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // カードのコストとPlayerのManaコストを比較する
        CardController card = GetComponent<CardController>();
        if (card.cardModel.isPlayerCard && GameManager.instance.isPlayerTurn && !card.cardModel.isFieldCard && card.cardModel.cost <= GameManager.instance.player.manaCost)
        {
            isDraggable = true;
        }
        else if (card.cardModel.isPlayerCard && GameManager.instance.isPlayerTurn && card.cardModel.isFieldCard && card.cardModel.canAttack)
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

    public IEnumerator MoveToField(Transform field)
    {
        // 一度親をCanvasに変更する
        transform.SetParent(defaltParent.parent);
        // Dotweenでカードを移動させる
        transform.DOMove(field.position, 0.25f);
        yield return new WaitForSeconds(0.25f);
        defaltParent = field;
        transform.SetParent(defaltParent);
    }
    public IEnumerator MoveToTarget(Transform target)
    {
        // 現在の位置と並びを取得する
        Vector3 currentPos = transform.position;
        int siblingIndex = transform.GetSiblingIndex(); // 子要素として何番目にいるか，並び順を取得する
        
        // 一度親をCanvasに変更する
        transform.SetParent(defaltParent.parent);
        // DotweenでカードをTargetに移動させる
        transform.DOMove(target.position, 0.25f);
        yield return new WaitForSeconds(0.25f);

        // カードを元の位置と並びに戻す
        transform.DOMove(currentPos, 0.25f);
        yield return new WaitForSeconds(0.25f);
        if (this != null)
        {
            transform.SetParent(defaltParent);
            transform.SetSiblingIndex(siblingIndex);
        }
    }


    void Start()
    {
        defaltParent = transform.parent;
    }
}
