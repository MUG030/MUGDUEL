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
            CreateCard(playerHandTransform);
        }
        else
        {
            CreateCard(enemyHandTransform);
        }
        TurnCalc();
    }

    private void PlayerTurn()
    {
        Debug.Log("Playerのターン");
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
        // attakerカードを選択
        CardController attacker = fieldCardList[0];
        // defenderカードを選択(Playerフィールドから選択)
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        CardController defender = playerFieldCardList[0];
        // attakerとdefenderが戦う
        CardsBattle(attacker, defender);


        // 最後にターンチェンジを自動で行う
        ChangeTurn();
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        Debug.Log("CardsBattle");
        Debug.Log("attacker HP:" + attacker.cardModel.hp);
        Debug.Log("defender HP:" + defender.cardModel.hp);
        
        attacker.cardModel.Attack(defender);
        defender.cardModel.Attack(attacker);

        Debug.Log("attacker HP:" + attacker.cardModel.hp);
        Debug.Log("defender HP:" + defender.cardModel.hp);

        attacker.CheckAlive();
        defender.CheckAlive();
    }

    private void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateCard(playerHandTransform);
            CreateCard(enemyHandTransform);
        }
    }

    private void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }
}
