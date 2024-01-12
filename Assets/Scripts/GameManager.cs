using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // 手札にカードを生成

    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;

    [SerializeField] Transform playerHandTransform,
                               playerFieldTransform,
                               enemyHandTransform,
                               enemyFieldTransform;
    [SerializeField] CardController cardPrefab;
    private bool isPlayerTurn;
    private List<int> playerDeck = new List<int>() {3,1,2,4,3,4,1},
                      enemyDeck = new List<int>() {2,4,3,1,1,3,4};

    private int playerHeroHp;
    private int enemyHeroHp;

    [SerializeField] TextMeshProUGUI playerHeroHpText,
                                     enemyHeroHpText;

    public int playerManaCost;
    public int enemyManaCost;

    [SerializeField] TextMeshProUGUI playerManaCostText,
                                     enemyManaCostText;

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
        // リザルトは非表示
        resultPanel.SetActive(false);
        // 両プレイヤーのHPを設定
        playerHeroHp = 1;
        enemyHeroHp = 1;
        playerManaCost = 1;
        enemyManaCost = 1;
        ShowHeroHp();
        ShowManaCost();
        // カードをそれぞれに３枚配る
        SettingInitHand();
        // カードをシャッフル
        // カードを配る
        // ターンを開始する
        isPlayerTurn = true;
        TurnCalc();             // ターン切り替え処理
    }

    private void ShowManaCost()
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }

    /// <summary> マナコストを減らす </summary>
    /// <param name="cost"> カード召喚に必要なコスト </param>
    /// <param name="isPlayerCard"> prayerのカードが動かされたか判定 </param>
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
        ShowManaCost();
    }

    public void Restart()
    {
        // handとFieldのカードを削除
        foreach (Transform card in playerHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyHandTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in playerFieldTransform)
        {
            Destroy(card.gameObject);
        }
        foreach (Transform card in enemyFieldTransform)
        {
            Destroy(card.gameObject);
        }
        // デッキを生成
        playerDeck = new List<int>() {3,1,2,4,3,4,1};
        enemyDeck = new List<int>() {2,4,3,1,1,3,4};
        StartGame();
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
        /* フィールドのカードを攻撃可能にする */
        // Fieldのカードを攻撃可能にする
        CardController[] enemyFieldCard = enemyFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in enemyFieldCard)
        {
            // cardを攻撃可能にする
            card.SetCanAttack(true);
        }

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
        
        if (enemyCanAttackCardList.Length > 0)
        {
            // attakerカードを選択
            CardController attacker = enemyCanAttackCardList[0];
            if (playerFieldCardList.Length > 0)
            {
                // dienderカードを選択
                CardController defender = playerFieldCardList[0];
                // attakerとdefenderが戦う
                CardsBattle(attacker, defender);
            }
            else
            {
                AttackToHero(attacker, false);
            }
            
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

    private void ShowHeroHp()
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }

    public void AttackToHero(CardController attacker, bool isPLayerCard)
    {
        if (isPLayerCard)
        {
            // PlayerのHeroを攻撃
            // HeroのHPを減らす
            enemyHeroHp -= attacker.cardModel.atk;
        }
        else
        {
            // EnemyのHeroを攻撃
            // HeroのHPを減らす
            playerHeroHp -= attacker.cardModel.atk;
        }
        attacker.SetCanAttack(false);
        ShowHeroHp();
        CheckHeroHp();
    }

    private void CheckHeroHp()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            resultPanel.SetActive(true);
            if (playerHeroHp <= 0)
            {
                resultText.text = "LOSE";
            }
            else
            {
                resultText.text = "WIN";
            }
        }
    }

    
}
