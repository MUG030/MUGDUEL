using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    public List<int> deck = new List<int>();

    public int heroHp;
    public int manaCost;
    public int defaultManaCost;

    public int heroTimeCount;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        heroHp = 10;
        manaCost = 3;
        defaultManaCost = 3;
        heroTimeCount = 20;
    }

    public void IncreaseManaCost()
    {
        defaultManaCost++;
        manaCost = defaultManaCost;
    }
}
