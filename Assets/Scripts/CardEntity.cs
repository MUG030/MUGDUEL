using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardEntity", menuName = "CreateCardEntity")]
// カードデータそのものとその処理
public class CardEntity : ScriptableObject
{
    public new string name;
    public int hp;
    public int atk;
    public int cost;
    public Sprite icon;
    public ABILITY ability;
    public SPELL spel;
}

public enum ABILITY
{
    NONE,
    INIT_ATTACKABLE,
    SHIELD,
}

public enum SPELL
{
    NONE,
    DAMAGE_ENEMY_CARD,
    DAMAGE_ENEMY_CARDS,
    DAMAGE_ENEMY_HERO,
    HEAL_FRIEND_CARD,
    HEAL_FRIEND_CARDS,
    HEAL_FRIEND_HERO,
    DECREASE_TIME,
    INCREASE_TIME,
    DECREASE_DECK,
    DRAW_CARDS,
}
