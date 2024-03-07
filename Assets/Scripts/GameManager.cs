using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

    public bool isPlayerTurn;

    public Transform playerHero;
    public Transform enemyHero;

    private int defaltPlayerTimeCount,
                defaultEnemyTimeCount;

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
        // そして、以下のようにプレイヤーと敵のデッキを初期化します：
        player.Init(GenerateRandomDeck(10, 1, 6, 3));
        enemy.Init(GenerateRandomDeck(10, 1, 6, 3));
        uiManager.ShowHeroHP(player.heroHp, enemy.heroHp);
        uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
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

        for (int i = 0; i < size; i++)
        {
            int card;
            do
            {
                card = UnityEngine.Random.Range(minValue, maxValue + 1);
            } while (counts.ContainsKey(card) && counts[card] >= maxDuplicates);

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
        player.deck = GenerateRandomDeck(5, 1, 6, 10);
        enemy.deck = GenerateRandomDeck(5, 1, 6, 10);

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
        defaltPlayerTimeCount = player.heroTimeCount;
        defaultEnemyTimeCount = enemy.heroTimeCount;

        uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);

        while (defaltPlayerTimeCount > 0 || defaultEnemyTimeCount > 0)
        {
            yield return new WaitForSeconds(1); // 1秒待機
            if (isPlayerTurn)
            {
                defaltPlayerTimeCount--;
                uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);
            }
            else
            {
                defaultEnemyTimeCount--;
                uiManager.UpDateTime(defaultEnemyTimeCount, defaltPlayerTimeCount);
            }
        }
        ChangeTurn();
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

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);


        if (isPlayerTurn)
        {
            player.IncreaseManaCost();
            GiveCardToHand(player.deck, playerHandTransform);
        }
        else
        {
            enemy.IncreaseManaCost();
            GiveCardToHand(enemy.deck, enemyHandTransform);
        }
        uiManager.ShowManaCost(player.manaCost, enemy.manaCost);
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
    public void CheckHeroHP()
    {
        if (player.heroHp <= 0 || enemy.heroHp <= 0)
        {
            ShowResultPanel(player.heroHp);
        }
    }
    void ShowResultPanel(int heroHp)
    {
        StopAllCoroutines();
        uiManager.ShowResultPanel(heroHp);
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
}
