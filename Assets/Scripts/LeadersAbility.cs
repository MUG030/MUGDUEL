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

    public void WeatherTypeChange(Weather weatherType)
    {
        switch (weatherType)
        {
            case Weather.NONE:
                break;
            case Weather.SNOWY:
                break;
            case Weather.SUNNY:
                SunnyAbillity(-1);
                break;
            case Weather.RAINY:
                RainyAbillity(-3);
                break;
            case Weather.CLOUDY:
                CloudyAbillity(-1);
                break;
        }
    }

    public void SunnyAbillity(int sunnyAbillity)
    {
        CardController[] playerCards = gameManager.GetFriendFieldCards(true);
        CardController[] enemyCards = gameManager.GetEnemyFieldCards(false);
        foreach (var card in playerCards)
        {
            if (!card.cardModel.hasIncreasedAttack)
            {                        
                card.cardModel.atk += sunnyAbillity;
                card.cardModel.hasIncreasedAttack = true;
                card.RefreshView();
            }
        }
        foreach (var card in enemyCards)
        {
            if (!card.cardModel.hasIncreasedAttack)
            {
                card.cardModel.atk += sunnyAbillity;
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

    public void RainyAbillity(int rainyAbillity)
    {
        if (!gameManager.weatherSwitch) return;
        gameManager.enemy.heroTimeCount -= rainyAbillity;
        gameManager.defaultEnemyTimeCount = gameManager.enemy.heroTimeCount;
        gameManager.uiManager.UpDateTime(gameManager.enemy.heroTimeCount, gameManager.defaltPlayerTimeCount);

        gameManager.player.heroTimeCount -= rainyAbillity;
        gameManager.defaltPlayerTimeCount = gameManager.player.heroTimeCount;
        gameManager.uiManager.UpDateTime(gameManager.defaultEnemyTimeCount, gameManager.player.heroTimeCount);
        gameManager.weatherSwitch = false;
    }

    public void CloudyAbillity(int cloudyAbillity)
    {
        CardController[] playerCards = gameManager.GetFriendFieldCards(true);
        CardController[] enemyCards = gameManager.GetEnemyFieldCards(true);
        foreach (var card in playerCards)
        {
            if (!card.cardModel.hasIncreasedHp)
            {                        
                card.cardModel.hp += cloudyAbillity;
                card.cardModel.hasIncreasedHp = true;
                card.RefreshView();
            }
        }
        foreach (var card in enemyCards)
        {
            if (!card.cardModel.hasIncreasedHp)
            {
                card.cardModel.hp += cloudyAbillity;
                card.cardModel.hasIncreasedHp = true;
                card.RefreshView();
            }
        }
    }
}
