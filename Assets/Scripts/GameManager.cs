using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 手札にカードを生成

    [SerializeField] Transform playerHandTransform;
    [SerializeField] CardController cardPrefab;
    // Start is called before the first frame update
    void Start()
    {
        CreateCard(playerHandTransform);
    }

    private void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }
}
