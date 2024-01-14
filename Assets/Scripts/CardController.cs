using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CardController : MonoBehaviour
{
    private CardView cardView;          // 見かけ(View)に関する事を操作
    public CardModel cardModel;        // データ(Model)に関する事を操作
    public CardMovement cardMovement;  // 移動(Movement)に関する事を操作
    // 全部処理するとややこしくなるため，別のクラスで管理する

    private void Awake()
    {
        cardView = GetComponent<CardView>();
        cardMovement = GetComponent<CardMovement>();
    }

    public void Init(int cardID, bool isPlayer)
    {
        cardModel = new CardModel(cardID, isPlayer);
        cardView.Show(cardModel);
    }

    public void Attack(CardController enemyCard)
    {
        cardModel.Attack(enemyCard);
        SetCanAttack(false);
    }

    public void SetCanAttack(bool canAttack)
    {
        cardModel.canAttack = canAttack;
        cardView.SetActiveSelectablePanel(canAttack);
    }

    public void CheckAlive()
    {
        if (cardModel.isAlive)
        {
            cardView.Refresh(cardModel);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
