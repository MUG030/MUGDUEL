using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 攻撃される側
// 場に置かれた時にフィールドが反映するように，攻撃された側が攻撃処理を行う
public class AttackedCard : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        /* 攻撃 */
        // attakerカードを選択
        CardController attacker = eventData.pointerDrag.GetComponent<CardController>();
        // defenderカードを選択(Playerフィールドから選択)
        CardController defender = GetComponent<CardController>();
        // attakerとdefenderが戦う
        GameManager.instance.CardsBattle(attacker, defender);    // CardsBattle(attacker, defender);だとGameManagerの関数をpublicにしても呼び出せない
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
