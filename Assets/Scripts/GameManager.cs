using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public enum Weather
{
    // NONE,
    SUNNY,
    RAINY,
    SNOWY,
    CLOUDY
}

public class GameManager : MonoBehaviour
{
    public GamePlayerManager player;
    public GamePlayerManager enemy;
    [SerializeField] AI enemyAI;
    [SerializeField] UIManager uiManager;

    public Transform playerHandTransform,
                               playerFieldTransform,
                               enemyHandTransform,
                               enemyFieldTransform;
    [SerializeField] CardController cardPrefab;
    [SerializeField] LeadersAbility leadersAbility;

    public bool isPlayerTurn;

    public Transform playerHero;
    public Transform enemyHero;


    private int defaltPlayerTimeCount,
                defaultEnemyTimeCount;

    private Weather currentWeather;
    private Weather nextWeather;
    private int turnCount;

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
        defaltPlayerTimeCount = player.heroTimeCount;
        defaultEnemyTimeCount = enemy.heroTimeCount;
        DisplayDeckCount();
        // player.Init(GenerateRandomDeck(10, 1, 6, 3));
        // enemy.Init(GenerateRandomDeck(10, 1, 6, 3));
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
        uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);

        turnCount++;
        Debug.Log(turnCount);
        currentWeather = nextWeather;
        nextWeather = (Weather)UnityEngine.Random.Range(0, 1);
        Debug.Log("Current Weather is:" + currentWeather);
        Debug.Log("Next Weather is:" + nextWeather);
    }

    public void DisplayDeckCount()
    {
        List<int> playerDeck = GenerateRandomDeck(10, 1, 7, 3);
        List<int> enemyDeck = GenerateRandomDeck(10, 1, 7, 3);

        player.Init(playerDeck);
        enemy.Init(enemyDeck);
    }

    /// <summary>
    /// ランダムなデッキを生成する
    /// </summary>
    /// <param name="size"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="maxDuplicates"></param>
    /// <returns></returns>
    private List<int> GenerateRandomDeck(int size, int minValue, int maxValue, int maxDuplicates)
    {
        List<int> deck = new List<int>();
        Dictionary<int, int> counts = new Dictionary<int, int>();

        /*
        deck.Add(7);
        counts.Add(7, 1);
        */

        for (int i = 0; i < size /* - 1*/ ; i++)
        {
            int card;
            do
            {
                card = UnityEngine.Random.Range(minValue, maxValue + 1);
            } while (counts.ContainsKey(card) && counts[card] >= maxDuplicates /*|| card == 7*/);

            deck.Add(card);

            if (counts.ContainsKey(card))
            {
                counts[card]++;
            }
            else
            {
                counts.Add(card, 1);
            }
        }

        uiManager.ShowDeckCount(player.deck.Count, enemy.deck.Count);

        // cardPrefab.RefreshView();

        return deck;
    }

    public void ReduceManaCost(int cost, bool isPlayerCard)
    {
        if (isPlayerCard)
        {
            player.manaCost -= cost;
        }
        else
        {
            enemy.manaCost -= cost;
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
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
        DisplayDeckCount();
        // player.deck = GenerateRandomDeck(5, 1, 6, 10);
        // enemy.deck = GenerateRandomDeck(5, 1, 6, 10);

        StartGame();
    }

    void SettingInitHand()
    {
        // カードをそれぞれに3まい配る
        for (int i = 0; i < 3; i++)
        {
            GiveCardToHand(player.deck, playerHandTransform);
            GiveCardToHand(enemy.deck, enemyHandTransform);
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
        uiManager.ShowDeckCount(player.deck.Count, enemy.deck.Count);
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

        LeadersAbility(card);
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
        defaltPlayerTimeCount = player.heroTimeCount;
        defaultEnemyTimeCount = enemy.heroTimeCount;

        uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);

        if (isPlayerTurn)
        {
            while (defaltPlayerTimeCount > 0)
            {
                yield return new WaitForSeconds(1); // 1秒待機
                defaltPlayerTimeCount--;
                uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);
            }
            ChangeTurn();
        }
        else
        {
            while (defaultEnemyTimeCount > 0)
            {
                yield return new WaitForSeconds(1); // 1秒待機
                defaultEnemyTimeCount--;
                uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);
            }
            ChangeTurn();
        }
        
    }

    public CardController[] GetEnemyFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }        
    }

    public CardController[] GetFriendFieldCards(bool isPlayer)
    {
        if (isPlayer)
        {
            return playerFieldTransform.GetComponentsInChildren<CardController>();
        }
        else
        {
            return enemyFieldTransform.GetComponentsInChildren<CardController>();
        }        
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

        CheckDeckCount();
        CheckDefaultTime();

        if (player.heroTimeCount == 0 || enemy.heroTimeCount == 0)
        {
            return;
        }

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);


        if (isPlayerTurn)
        {
            player.IncreaseManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
            turnCount++;
        }
        else
        {
            enemy.IncreaseManaCost();
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);

        if (turnCount % 3 == 0)
        {
            currentWeather = nextWeather;
            nextWeather = (Weather)UnityEngine.Random.Range(0, 1);
            Debug.Log("Current Weather is:" + currentWeather);
            Debug.Log("Next Weather is:" + nextWeather);
        }
        else if (turnCount % 3 == 2)
        {
            Debug.Log("Next Weather will be:" + nextWeather);
        }
        
        ApplyWeatherEffects();

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
        attacker.Attack(defender);
        defender.Attack(attacker);

        attacker.CheckAlive();
        defender.CheckAlive();
    }

    public void AttackToHero(CardController attacker)
    {
        if (attacker.cardModel.isPlayerCard)
        {
            enemy.heroHp -= attacker.cardModel.atk;
        }
        else
        {
            player.heroHp -= attacker.cardModel.atk;
        }
        attacker.SetCanAttack(false);
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
    }

    public void HealToHero(CardController healer)
    {
        if (healer.cardModel.isPlayerCard)
        {
            player.heroHp += healer.cardModel.atk;
        }
        else
        {
            enemy.heroHp += healer.cardModel.atk;
        }
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
    }

    // ヒーローのHPをチェック
    public void CheckHeroHP()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
    }

    // カードの枚数をチェック
    public void CheckDeckCount()
    {
        if (isPlayerTurn)
        {
            if (player.deck.Count == 0)
            {
                ShowResultPanel(player.deck.Count);
            }
        }
        else
        {
            if (enemy.deck.Count == 0)
            {
                ShowResultPanel(player.deck.Count);
            }
        }
    }

    // defaulrTimeをチェック
    public void CheckDefaultTime()
    {
        if (player.heroTimeCount <= 0 || enemy.heroTimeCount <= 0)
        {
            ShowResultPanel(player.heroTimeCount);
        }
    }

    // 結果を表示する
    void ShowResultPanel(int resultCount)
    {
        StopAllCoroutines();
        enemyAI.StopAllCoroutines();
        uiManager.ShowResultPanel(resultCount);
    }

    public void DecreseTime(int decreseTime)
    {
        if (isPlayerTurn)
        {
            enemy.heroTimeCount -= decreseTime;
            defaultEnemyTimeCount = enemy.heroTimeCount;
            uiManager.UpDateTime(enemy.heroTimeCount, defaltPlayerTimeCount);
        }
        else
        {
            player.heroTimeCount -= decreseTime;
            defaltPlayerTimeCount = player.heroTimeCount;
            uiManager.UpDateTime(defaultEnemyTimeCount, player.heroTimeCount);
        }
    }

    public void IncreaseTime(int increaseTime)
    {
        if (isPlayerTurn)
        {
            player.heroTimeCount += increaseTime;
            defaltPlayerTimeCount = player.heroTimeCount;
            uiManager.UpDateTime(defaultEnemyTimeCount, player.heroTimeCount);
        }
        else
        {
            enemy.heroTimeCount += increaseTime;
            defaultEnemyTimeCount = enemy.heroTimeCount;
            uiManager.UpDateTime(enemy.heroTimeCount, defaltPlayerTimeCount);
        }
    }

    public void DecreaseDeck(int decreseDeck)
    {
        if (isPlayerTurn)
        {
            enemy.deck.RemoveAt(decreseDeck);
        }
        else
        {
            player.deck.RemoveAt(decreseDeck);
        }
        uiManager.ShowDeckCount(player.deck.Count, enemy.deck.Count);
    }


    // デッキからdrawCards枚引く
    public void DrawCards(int drawCards)
    {
        for (int i = 0; i < drawCards; i++)
        {
            if (isPlayerTurn)
            {
                GiveCardToHand(player.deck, playerHandTransform);
            }
            else
            {
                GiveCardToHand(enemy.deck, enemyHandTransform);
            }
        }
    }

    private void LeadersAbility(CardController cardController)
    {
        if (leadersAbility.leaderType == LEADER.NONE)
        {
            return;
        }
        switch (leadersAbility.leaderType)
        {
            case LEADER.ATK_LEADER:
                cardController.LeaderAtkSkill(cardController, 1);
                break;
            case LEADER.HP_LEADER:
                cardController.LeaderHpSkill(cardController, 1);
                break;
        }
    }

    public void ApplyWeatherEffects()
    {
        Debug.Log("ApplyWeatherEffects");
        switch (currentWeather)
        {
            /*case Weather.NONE:
                break;*/
            case Weather.RAINY:
                // Decrease time limit by 3
                break;
            case Weather.SNOWY:
                // Increase mana by 3
                player.manaCost += 3;
                break;
            case Weather.SUNNY:
                // Increase attack power by 1
                Debug.Log("SUNNY");
                CardController[] playerCards = GetFriendFieldCards(true);
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
                /*if (isPlayerTurn)
                {
                    CardController[] playerCards = GetFriendFieldCards(true);
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
                }
                else
                {
                    CardController[] enemyCards = GetEnemyFieldCards(true);
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
                }*/
                
                break;
            case Weather.CLOUDY:
                // Increase HP by 1
                player.heroHp += 1;
                break;
        }
    }
}
