using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 手札にカードを生成

    [SerializeField] Transform playerHandTransform,enemyHandTransform;
    [SerializeField] CardController cardPrefab;
    private bool isPlayerTurn;

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
        
        // 最後にターンチェンジを自動で行う
        ChangeTurn();
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
