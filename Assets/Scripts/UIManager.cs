using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI playerHeroHpText;
    [SerializeField] TextMeshProUGUI enemyHeroHpText;
    [SerializeField] TextMeshProUGUI playerManaCostText;
    [SerializeField] TextMeshProUGUI enemyManaCostText;
    // 時間管理
    [SerializeField] TextMeshProUGUI enemyTimeCountText;
    [SerializeField] TextMeshProUGUI playerTimeCountText;
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

    public void ShowResultPanel(int heroHp)
    {        
        resultPanel.SetActive(true);
        if (heroHp <= 0)
        {
            resultText.text = "LOSE";
        }
        else
        {
            resultText.text = "WIN";
        }
    }

}
