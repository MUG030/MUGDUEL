using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI playerHeroHpText;
    [SerializeField] TextMeshProUGUI enemyHeroHpText;
    [SerializeField] TextMeshProUGUI playerManaCostText;
    [SerializeField] TextMeshProUGUI enemyManaCostText;
    [SerializeField] TextMeshProUGUI playerDeckCountText;
    [SerializeField] TextMeshProUGUI enemyDeckCountText;
    // 時間管理
    [SerializeField] TextMeshProUGUI enemyTimeCountText;
    [SerializeField] TextMeshProUGUI playerTimeCountText;
    [SerializeField] TextMeshProUGUI weatherBoardText;
    [SerializeField] GameObject weatherBoard;

    [SerializeField] TextMeshProUGUI deadPoolText;
    // Start is called before the first frame update
    public void HideResultPanel()
    {
        resultPanel.SetActive(false);
    }
    public void ShowManaCost(int playerManaCost, int enemyManaCost)
    {
        playerManaCostText.text = playerManaCost.ToString();
        enemyManaCostText.text = enemyManaCost.ToString();
    }

    public void ShowDeckCount(int playerDeckCount, int enemyDeckCount)
    {
        playerDeckCountText.text = playerDeckCount.ToString();
        enemyDeckCountText.text = enemyDeckCount.ToString();
    }

    public void UpdateDeckCount(int playerDeckCount, int enemyDeckCount)
    {
        playerDeckCountText.text = playerDeckCount.ToString();
        enemyDeckCountText.text = enemyDeckCount.ToString();
    }

    public void UpDateTime(int enemyTimeCount, int playerTimeCount)
    {
        enemyTimeCountText.text = enemyTimeCount.ToString();
        playerTimeCountText.text = playerTimeCount.ToString();
    }
    
    public void ShowHeroHP(int playerHeroHp, int enemyHeroHp)
    {
        playerHeroHpText.text = playerHeroHp.ToString();
        enemyHeroHpText.text = enemyHeroHp.ToString();
    }

    public void DeadCardList(int deadCardCount)
    {
        deadPoolText.text = deadCardCount.ToString();
    }

    public void ShowResultPanel(int resultCount)
    {
        resultPanel.SetActive(true);
        if (resultCount <= 0)
        {
            resultText.text = "LOSE";
        }
        else
        {
            resultText.text = "WIN";
        }
    }

    public async void ShowWeatherForecast(string weather)
    {
        weatherBoard.SetActive(true);
        weatherBoardText.text = weather;
        // Unitaskを使って3秒待機
        await UniTask.Delay(3000);
        weatherBoard.SetActive(false);
    }
}
