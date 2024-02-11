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
    public ABILITY ability;
    public SPELL spell;

    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;

    public CardModel(int cardID, bool isPlayer)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardDataList/Card" + cardID);
        name = cardEntity.name;
        hp = cardEntity.hp;
        atk = cardEntity.atk;
        cost = cardEntity.cost;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        isAlive = true;
        isPlayerCard = isPlayer;
        spell = cardEntity.spel;
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

    private void RecoveryHP(int heal)
    {
        hp += heal;
    }

    /// <summary>
    /// カードを攻撃する処理の実装
    /// </summary>
    /// <param name="card"></param>
    public void Attack(CardController card)
    {
        card.cardModel.Damage(atk);
    }

    public void Heal(CardController card)
    {
        card.cardModel.RecoveryHP(atk);
    }

}
