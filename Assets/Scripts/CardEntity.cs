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
}

public enum ABILITY
{
    NONE,
    INIT_ATTACKABLE,
    SHIELD,
}
