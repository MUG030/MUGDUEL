using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] AI enemyAI;
    [SerializeField] UIManager uiManager;

    public Transform playerHandTransform,
                               playerFieldTransform,
                               enemyHandTransform,
                               enemyFieldTransform;
    [SerializeField] CardController cardPrefab;

    public bool isPlayerTurn;

    List<int> playerDeck = new List<int>() { 3, 1, 2, 2 , 3},
              enemyDeck  = new List<int>() { 2, 1, 3, 1 , 3};

    int playerHeroHp;
    int enemyHeroHp;

    public Transform playerHero;

    public int playerManaCost;
    public int enemyManaCost;
    int playerDefaultManaCost;
    int enemyDefaultManaCost;

    int timeCount;

    // シングルトン化（どこからでもアクセスできるようにする）
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        uiManager.HideResultPanel();
        playerHeroHp = 10;
        enemyHeroHp = 10;
        playerManaCost = playerDefaultManaCost = 10;
        enemyManaCost = enemyDefaultManaCost = 10;
        uiManager.ShowHeroHP(playerHeroHp, enemyHeroHp);
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
        uiManager.ShowManaCost(playerManaCost, enemyManaCost);
    }


    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            playerManaCost -= cost;
        }
        else
        {
            enemyManaCost -= cost;
        }
        uiManager.ShowManaCost(playerManaCost, enemyManaCost);
    }

    public void Restart()
    {
        // handとFiledのカードを削除
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }


        // デッキを生成
        playerDeck = new List<int>() { 3, 1, 2, 2, 3 };
        enemyDeck = new List<int>() { 2, 1, 3, 1, 3 };

        StartGame();
    }

    void SettingInitHand()
    {
        // カードをそれぞれに3まい配る
        for (int i = 0; i < 3; i++)
        {
            GiveCardToHand(playerDeck, playerHandTransform);
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
    }
    void GiveCardToHand(List<int> deck, Transform hand)
    {
        if (deck.Count == 0)
        {
            return;
        }
        int cardID = deck[0];
        deck.RemoveAt(0);
        CreateCard(cardID, hand);
    }

    void CreateCard(int cardID, Transform hand)
    {
        // カードの生成とデータの受け渡し
        CardController card = Instantiate(cardPrefab, hand, false);
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
    }

    void TurnCalc()
    {
        StopAllCoroutines();
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(enemyAI.EnemyTurn());
        }
    }

    IEnumerator CountDown()
    {
        timeCount = 20;
        uiManager.UpDateTime(timeCount);

        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1); // 1秒待機
            timeCount--;
            uiManager.UpDateTime(timeCount);
        }
        ChangeTurn();
    }

    public CardController[] GetEnemyFieldCards()
    {
        return enemyFieldTransform.GetComponentsInChildren<CardController>();
    }

    public void OnClickTurnEnd()
    {
        if (isPlayerTurn)
        {
            ChangeTurn();
        }
    }
    

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);


        if (isPlayerTurn)
        {
            playerDefaultManaCost++;
            playerManaCost = playerDefaultManaCost;
            GiveCardToHand(playerDeck, playerHandTransform);
        }
        else
        {
            enemyDefaultManaCost++;
            enemyManaCost = enemyDefaultManaCost;
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
        uiManager.ShowManaCost(playerManaCost, enemyManaCost);
        TurnCalc();
    }

    public void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList)
        {
            card.SetCanAttack(canAttack);
        }
    }

    void PlayerTurn()
    {
        Debug.Log("Playerのターン");
        // フィールドのカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);
    }
    

    public void CardsBattle(CardController attacker, CardController defender)
    {
        Debug.Log("CardsBattle");
        Debug.Log("attacker HP:" + attacker.cardModel.hp);
        Debug.Log("defender HP:" + defender.cardModel.hp);

        attacker.Attack(defender);
        defender.Attack(attacker);
        Debug.Log("attacker HP:" + attacker.cardModel.hp);
        Debug.Log("defender HP:" + defender.cardModel.hp);
        attacker.CheckAlive();
        defender.CheckAlive();
    }

    public void AttackToHero(CardController attacker, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            enemyHeroHp -= attacker.cardModel.atk;
        }
        else
        {
            playerHeroHp -= attacker.cardModel.atk;
        }
        attacker.SetCanAttack(false);
        uiManager.ShowHeroHP(playerHeroHp, enemyHeroHp);
    }
    public void CheckHeroHP()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            ShowResultPanel(playerHeroHp);
        }
    }
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(heroHp);
    }  
}
