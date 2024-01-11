using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 手札にカードを生成

    [SerializeField] Transform playerHandTransform;
    [SerializeField] GameObject cardPrefab;
    // Start is called before the first frame update
    void Start()
    {
        CreateCard(playerHandTransform);
    }

    private void CreateCard(Transform hand)
    {
        Instantiate(cardPrefab, hand, false);
    }
}
