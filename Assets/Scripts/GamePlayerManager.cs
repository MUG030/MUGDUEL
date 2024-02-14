using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerManager : MonoBehaviour
{
    public List<int> deck = new List<int>();

    public int heroHp;
    public int manaCost;
    public int defaultManaCost;

    public void Init(List<int> cardDeck)
    {
        deck = cardDeck;
        heroHp = 10;
        manaCost = 3;
        defaultManaCost = 3;
    }

    public void IncreaseManaCost()
    {
        defaultManaCost++;
        manaCost = defaultManaCost;
    }
}
