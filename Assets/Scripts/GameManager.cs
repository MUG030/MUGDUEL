using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

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

    [SerializeField] Transform playerHero;

    [SerializeField] TextMeshProUGUI playerHeroHpText,
                                     enemyHeroHpText;

    public int playerManaCost;
    public int enemyManaCost;
    private int playerDefaltManaCost;
    private int enemyDefaltManaCost;

    [SerializeField] TextMeshProUGUI playerManaCostText,
                                     enemyManaCostText;

    // 時間管理
    [SerializeField] TextMeshProUGUI timeCountText;
    private int timeCount;


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
        playerManaCost = playerDefaltManaCost = 10;
        enemyManaCost = enemyDefaltManaCost = 10;
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
        StopAllCoroutines();
        StartCoroutine(CountDown());
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    } 

    IEnumerator CountDown()
    {
        timeCount = 8;
        timeCountText.text = timeCount.ToString();

        while (timeCount > 0)
        {
            yield return new WaitForSeconds(1);  // 1秒待つ
            timeCount--;
            timeCountText.text = timeCount.ToString();
        }
        ChangeTurn();
    }

    public void ChangeTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, false);
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, false);

        if(isPlayerTurn)
        {
            playerDefaltManaCost++;
            playerManaCost = playerDefaltManaCost;
            GiveCardToHand(playerDeck, playerHandTransform);
        }
        else
        {
            enemyDefaltManaCost++;
            enemyManaCost = enemyDefaltManaCost;
            GiveCardToHand(enemyDeck, enemyHandTransform);
        }
        ShowManaCost();
        TurnCalc();
    }

    private void SettingCanAttackView(CardController[] fieldCardList, bool canAttack)
    {
        foreach (CardController card in fieldCardList)
        {
            // cardを攻撃可能にする
            card.SetCanAttack(canAttack);
        }
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
        if (hand.name == "PlayerHand")
        {
            card.Init(cardID, true);
        }
        else
        {
            card.Init(cardID, false);
        }
        card.Init(cardID, false);
    }

    private void PlayerTurn()
    {
        Debug.Log("Playerのターン");
        // Fieldのカードを攻撃可能にする
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(playerFieldCardList, true);
    }

    private IEnumerator EnemyTurn()
    {
        Debug.Log("Enemyのターン");
        /* フィールドのカードを攻撃可能にする */
        // Fieldのカードを攻撃可能にする
        CardController[] enemyFieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1);

        // 手札のカードリストを取得
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();

        // コスト以下のカードがあれば，カードをフィールドに出し続ける
        while (Array.Exists(handCardList, card => card.cardModel.cost <= enemyManaCost))
        {
            // コスト以下のカードリストを取得
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card => card.cardModel.cost <= enemyManaCost);   // 検索Array.FindAll(検索対象の配列, cardに配列の要素を入れる)
            // 場に出すカードを選択
            CardController enemyCard = selectableHandCardList[0];
            // カードを移動
            StartCoroutine(enemyCard.cardMovement.MoveToField(enemyFieldTransform));
            ReduceManaCost(enemyCard.cardModel.cost, false);
            enemyCard.cardModel.isFieldCard = true;
            handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }


        

        yield return new WaitForSeconds(1);

        /* 攻撃 */
        // Fieldのカードリストを取得
        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        // 攻撃可能カードがあれば攻撃を繰り返す
        while (Array.Exists(fieldCardList, card => card.cardModel.canAttack))
        {
            // 攻撃可能カードを取得
            CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.cardModel.canAttack);   // 検索Array.FindAll(検索対象の配列, 条件)
            // defenderカードを選択(Playerフィールドから選択)
            CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
            // attakerカードを選択
            CardController attacker = enemyCanAttackCardList[0];
            if (playerFieldCardList.Length > 0)
            {
                // dienderカードを選択
                CardController defender = playerFieldCardList[0];
                // attakerとdefenderが戦う
                StartCoroutine(attacker.cardMovement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.25f);
                CardsBattle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.cardMovement.MoveToTarget(playerHero.transform));
                yield return new WaitForSeconds(0.25f);
                AttackToHero(attacker, false);
                yield return new WaitForSeconds(0.25f);
                CheckHeroHp();
            }
            fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();  // フィールドのカードリストを更新
            yield return new WaitForSeconds(1);
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
    }

    private void CheckHeroHp()
    {
        if (playerHeroHp <= 0 || enemyHeroHp <= 0)
        {
            ShowResultPanel(playerHeroHp);
        }
    }

    private void ShowResultPanel(int hpHoro)
    {
        StopAllCoroutines();
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
