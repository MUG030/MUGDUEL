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

    // フィールドにカードを出した時によぶ関数
    public void OnField(bool isPlayer)
    {
        GameManager.instance.ReduceManaCost(cardModel.cost, isPlayer);
        cardModel.isFieldCard = true;
        if (cardModel.ability == ABILITY.INIT_ATTACKABLE)
        {
            SetCanAttack(true);
        }
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

    public void UseSpellTo(CardController targetCard)
    {
        if (cardModel.spell == SPELL.NONE)
        {
            return;
        }
        switch (cardModel.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
                // 特定の敵を攻撃
                Attack(targetCard);
                targetCard.CheckAlive();
                break;
            case SPELL.DAMAGE_ENEMY_CARDS: 
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                break;
            case SPELL.HEAL_FRIEND_CARD:
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                break;
        }
        Destroy(this.gameObject);
    }
}
