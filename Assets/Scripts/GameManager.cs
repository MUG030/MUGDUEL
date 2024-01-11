using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 手札にカードを生成

    [SerializeField] Transform playerHandTransform,
                               playerFieldTransform,
                               enemyHandTransform,
                               enemyFieldTransform;
    [SerializeField] CardController cardPrefab;
    private bool isPlayerTurn;
    private List<int> playerDeck = new List<int>() {3,1,2,4,3,4,1},
                      enemyDeck = new List<int>() {2,4,3,1,1,3,4};

    // シングルトン化（どこからでもアクセスできるようにする）
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        // カードをそれぞれに３枚配る
        SettingInitHand();
        // カードをシャッフル
        // カードを配る
        // ターンを開始する
        isPlayerTurn = true;
        TurnCalc();             // ターン切り替え処理
    }

    private void TurnCalc()
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    } 

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if(isPlayerTurn)
        {
            GiveCardToHand(playerDeck, playerHandTransform);
        }
        else
        {
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
        TurnCalc();
    }

    private void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            GiveCardToHand(playerDeck, playerHandTransform);
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
    }

    void GiveCardToHand(List<int> deck,Transform hand)
    {
        if (deck.Count == 0)
        {
            // 山札がなくなったら，カードを引かない
            return;
        }
        int cardID = deck[0];
        deck.RemoveAt(0);       // 0番目のカードを抜き取る
        CreateCard(cardID, hand);
    }

    private void CreateCard(int cardID, Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        // カードのID（種類）
        card.Init(cardID);
    }

    private void PlayerTurn()
    {
        Debug.Log("Playerのターン");
        // Fieldのカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in playerFieldCardList)
        {
            // cardを攻撃可能にする
            card.SetCanAttack(true);
        }
    }

    private void EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        // 手札のカードリストを取得
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        // 場に出すカードを選択
        CardController enemyCard = handCardList[0];
        // カードを移動
        enemyCard.cardMovement.SetCardTransform(enemyFieldTransform);

        /* 攻撃 */
        // Fieldのカードリストを取得
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能カードを取得
        CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.cardModel.canAttack);   // 検索Array.FindAll(検索対象の配列, 条件)
        // defenderカードを選択(Playerフィールドから選択)
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        
        if (enemyCanAttackCardList.Length > 0 && playerFieldCardList.Length > 0)
        {
            // attakerカードを選択
            CardController attacker = enemyCanAttackCardList[0];
            // dienderカードを選択
            CardController defender = playerFieldCardList[0];
            // attakerとdefenderが戦う
            CardsBattle(attacker, defender);
        }

        // 最後にターンチェンジを自動で行う
        ChangeTurn();
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

    
}
