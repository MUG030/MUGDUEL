using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMovement : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform defaltParent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        defaltParent = transform.parent;                    // 自分自身の親を取得する
        transform.SetParent(defaltParent.parent, false);    // 親の親（Canvas）を取得する
        GetComponent<CanvasGroup>().blocksRaycasts = false; // ドラッグする時はレイキャストをブロックしない
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(defaltParent, false);           // 離した時に親に戻る
        GetComponent<CanvasGroup>().blocksRaycasts = true;  // ドラッグ後はレイキャストをブロックする
    }
}
