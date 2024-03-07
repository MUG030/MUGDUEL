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

    GameManager gameManager;

    public bool IsSpell
    {
        get { return cardModel.spell != SPELL.NONE; }
    }
    private void Awake()
    {
        cardView = GetComponent<CardView>();
        cardMovement = GetComponent<CardMovement>();
        gameManager = GameManager.instance;
    }

    public void Init(int cardID, bool isPlayer)
    {
        cardModel = new CardModel(cardID, isPlayer);
        cardView.SetCard(cardModel);
    }

    /// <summary>
    /// カードを攻撃する関数の呼び出し
    /// </summary>
    /// <param name="enemyCard"></param>
    public void Attack(CardController enemyCard)
    {
        cardModel.Attack(enemyCard);
        SetCanAttack(false);
    }

    public void Heal(CardController friendCard)
    {
        cardModel.Heal(friendCard);
        friendCard.RefreshView();
    }

    public void Show()
    {
        cardView.Show();
    }

    public void RefreshView()
    {
        cardView.Refresh(cardModel);
    }

    public void SetCanAttack(bool canAttack)
    {
        cardModel.canAttack = canAttack;
        cardView.SetActiveSelectablePanel(canAttack);
    }

    // フィールドにカードを出した時によぶ関数

    public void OnField()
    {
        GameManager.instance.ReduceManaCost(cardModel.cost, cardModel.isPlayerCard);
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
            RefreshView();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /*
     *敵が居ないのに攻撃しようとしている ＝＞ 敵AIのチェックと同様にする

        */ 

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
                if (targetCard == null)
                {
                    return;
                }
                if (targetCard.cardModel.isPlayerCard == cardModel.isPlayerCard)
                {
                    return;
                }
                Attack(targetCard);
                targetCard.CheckAlive();
                break;
            case SPELL.DAMAGE_ENEMY_CARDS:
                // 相手フィールドの全てのカードを攻撃
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.cardModel.isPlayerCard);
                foreach (CardController enemyCard in enemyCards)
                {
                    Attack(enemyCard);
                }
                foreach (CardController enemyCard in enemyCards)
                {
                    enemyCard.CheckAlive();
                }
                break;
            case SPELL.DAMAGE_ENEMY_HERO:
                // 相手プレイヤーを攻撃
                gameManager.AttackToHero(this);
                break;
            case SPELL.HEAL_FRIEND_CARD:
                // 特定の味方カードを回復
                if (targetCard == null)
                {
                    return;
                }
                if (targetCard.cardModel.isPlayerCard != cardModel.isPlayerCard)
                {
                    return;
                }
                Heal(targetCard);
                break;
            case SPELL.HEAL_FRIEND_CARDS:
                // 味方フィールドの全てのカードを回復
                // 配列の中身をすべて回復
                CardController[] playerCards = gameManager.GetFriendFieldCards(this.cardModel.isPlayerCard);
                foreach (CardController playerCard in playerCards)
                {
                    Heal(playerCard);
                }
                break;
            case SPELL.HEAL_FRIEND_HERO:
                // 味方プレイヤーを回復
                gameManager.HealToHero(this);
                break;
            case SPELL.DECREASE_TIME:
                gameManager.DecreseTime(5);
                break;
            case SPELL.INCREASE_TIME:
                gameManager.IncreaseTime(5);
                break;
        }
        gameManager.ReduceManaCost(cardModel.cost, cardModel.isPlayerCard);
        Destroy(this.gameObject);
    }

    public bool CanUseSpell()
    {
        if (cardModel.spell == SPELL.NONE)
        {
            return false;
        }
        switch (cardModel.spell)
        {
            case SPELL.DAMAGE_ENEMY_CARD:
            case SPELL.DAMAGE_ENEMY_CARDS:
                // 相手フィールドの全てのカードを攻撃
                CardController[] enemyCards = gameManager.GetEnemyFieldCards(this.cardModel.isPlayerCard);
                if (enemyCards.Length > 0)
                {
                    return true;
                }
                return false;
            case SPELL.DAMAGE_ENEMY_HERO:
            case SPELL.HEAL_FRIEND_HERO:
            case SPELL.HEAL_FRIEND_CARD:
            case SPELL.DECREASE_TIME:
            case SPELL.INCREASE_TIME:
                return true;
            case SPELL.HEAL_FRIEND_CARDS:
                // 味方フィールドの全てのカードを回復
                // 配列の中身をすべて回復
                CardController[] playerCards = gameManager.GetFriendFieldCards(this.cardModel.isPlayerCard);
                if (playerCards.Length > 0)
                {
                    return true;
                }
                return false;
        }
        return false;
    }
}
