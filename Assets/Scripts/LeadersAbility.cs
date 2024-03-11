using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LEADER
{
    NONE,
    ATK_LEADER,
    HP_LEADER,
}

public class LeadersAbility : MonoBehaviour
{
    public LEADER leaderType;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;
    }

    public void SunnyAbillity()
    {
        CardController[] playerCards = gameManager.GetFriendFieldCards(true);
        CardController[] enemyCards = gameManager.GetEnemyFieldCards(false);
        foreach (var card in playerCards)
        {
            if (!card.cardModel.hasIncreasedAttack)
            {
                Debug.Log("Increase attack power by 1");                        
                card.cardModel.atk += 1;
                card.cardModel.hasIncreasedAttack = true;
                card.RefreshView();
            }
        }
        foreach (var card in enemyCards)
        {
            if (!card.cardModel.hasIncreasedAttack)
            {
                Debug.Log("Increase attack power by 1");
                card.cardModel.atk += 1;
                card.cardModel.hasIncreasedAttack = true;
                card.RefreshView();
            }
        }
    }

    public void SnowyAbillity()
    {
        Debug.Log("Decrease mana cost by 1");
        if (gameManager.isPlayerTurn)
        {
            gameManager.ReduceManaCost(1, true);
        }
        else
        {
            gameManager.ReduceManaCost(1, false);
        }
    }

    public void RainyAbillity(int decreaseTime)
    {
        if (!gameManager.weatherSwitch) return;
        gameManager.enemy.heroTimeCount -= decreaseTime;
        gameManager.defaultEnemyTimeCount = gameManager.enemy.heroTimeCount;
        gameManager.uiManager.UpDateTime(gameManager.enemy.heroTimeCount, gameManager.defaltPlayerTimeCount);

        gameManager.player.heroTimeCount -= decreaseTime;
        gameManager.defaltPlayerTimeCount = gameManager.player.heroTimeCount;
        gameManager.uiManager.UpDateTime(gameManager.defaultEnemyTimeCount, gameManager.player.heroTimeCount);
        gameManager.weatherSwitch = false;
    }

    public void CloudyAbillity()
    {
        CardController[] playerCards = gameManager.GetFriendFieldCards(true);
        CardController[] enemyCards = gameManager.GetEnemyFieldCards(true);
        foreach (var card in playerCards)
        {
            if (!card.cardModel.hasIncreasedHp)
            {
                Debug.Log("Increase attack power by 1");                        
                card.cardModel.hp += 1;
                card.cardModel.hasIncreasedHp = true;
                card.RefreshView();
            }
        }
        foreach (var card in enemyCards)
        {
            if (!card.cardModel.hasIncreasedHp)
            {
                Debug.Log("Increase attack power by 1");
                card.cardModel.hp += 1;
                card.cardModel.hasIncreasedHp = true;
                card.RefreshView();
            }
        }
    }
}
