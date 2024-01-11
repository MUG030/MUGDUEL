using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardController : MonoBehaviour
{
    // 見かけ(View)に関する事を操作
    private CardModel cardModel;// データ(Model)に関する事を操作
    // 全部処理するとややこしくなるため，別のクラスで管理する

    public void Init()
    {
        cardModel = new CardModel();
    }
}
