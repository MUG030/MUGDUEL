using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardController : MonoBehaviour
{
    private CardView cardView;// 見かけ(View)に関する事を操作
    private CardModel cardModel;// データ(Model)に関する事を操作
    // 全部処理するとややこしくなるため，別のクラスで管理する

    private void Awake()
    {
        cardView = GetComponent<CardView>();
    }

    public void Init(int cardID)
    {
        cardModel = new CardModel(cardID);
        cardView.Show(cardModel);
    }
}
