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
    public int abillityScore;
    public Sprite icon;
    public ABILITY ability;
    public SPELL spell;
    public int id;

    public bool isAlive;
    public bool canAttack;
    public bool isFieldCard;
    public bool isPlayerCard;

    public bool hasIncreasedAttack { get; set; }
    public bool hasIncreasedHp { get; set; }

    public CardModel(int cardID, bool isPlayer)
    {
        CardEntity cardEntity = Resources.Load<CardEntity>("CardDataList/Card" + cardID);
        name = cardEntity.name;
        hp = cardEntity.hp;
        atk = cardEntity.atk;
        cost = cardEntity.cost;
        abillityScore = cardEntity.abillityScore;
        icon = cardEntity.icon;
        ability = cardEntity.ability;
        isAlive = true;
        isPlayerCard = isPlayer;
        spell = cardEntity.spel;

        id = cardID;
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

    private void AddAtk(int addAtk)
    {
        atk += addAtk;
    }

    private void AddHp(int addHp)
    {
        hp += addHp;
    }

    /// <summary>
    /// カードを攻撃する処理の実装
    /// </summary>
    /// <param name="card"></param>
    public void Attack(CardController card)
    {
        if (card.cardModel.ability == ABILITY.PROTECT)
        {
            Debug.Log("プロテクト発動");
            int dmg = atk - card.cardModel.abillityScore;
            card.cardModel.Damage(dmg);
        }
        else
        {
            Debug.Log("通常攻撃");
            card.cardModel.Damage(atk);
        }
    }

    public void Heal(CardController card)
    {
        card.cardModel.RecoveryHP(atk);
    }

    public void LeaderAtkSkill(CardController card, int addAtk)
    {
        card.cardModel.AddAtk(addAtk);
    }

    public void LeaderHpSkill(CardController card, int addHp)
    {
        card.cardModel.AddHp(addHp);
    }
}
