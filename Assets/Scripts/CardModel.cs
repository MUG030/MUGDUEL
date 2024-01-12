using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// オブジェクトとして意味があるわけではない。
// カードデータとその処理が目的。コンポーネントは必要ないため，MonoBehaviourを継承しない
public class CardModel
{
    public string name;
    public int hp;
    public int atk;
    public int cost;
    public Sprite icon;
    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;

    public CardModel(int cardID)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardDataList/Card" + cardID);
        name = cardEntity.name;
        hp = cardEntity.hp;
        atk = cardEntity.atk;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        isAlive = true;
    }

    private void Damage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            hp = 0;
            isAlive = false;
        }
    }

    public void Attack(CardController card)
    {
        card.cardModel.Damage(atk);
    }

}
