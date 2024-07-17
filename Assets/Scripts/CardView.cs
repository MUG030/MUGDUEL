using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] TextMeshProUGUI atkText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Image iconImage;
    [SerializeField] GameObject selectablePanel;
    [SerializeField] GameObject shieldPanel;
    [SerializeField] GameObject protectPanel;
    [SerializeField] GameObject maskPanel;

    public void SetCard(CardModel cardModel)
    {
        nameText.text = cardModel.name;
        hpText.text = cardModel.hp.ToString();
        atkText.text = cardModel.atk.ToString();
        costText.text = cardModel.cost.ToString();
        iconImage.sprite = cardModel.icon;
        maskPanel.SetActive(!cardModel.isPlayerCard);

        if (cardModel.ability == ABILITY.SHIELD)
        {
            shieldPanel.SetActive(true);
        }
        else
        {
            shieldPanel.SetActive(false);
        }

        if (cardModel.ability == ABILITY.PROTECT)
        {
            protectPanel.SetActive(true);
        }
        else
        {
            protectPanel.SetActive(false);
        }

        if (cardModel.spell != SPELL.NONE)
        {
            hpText.gameObject.SetActive(false);
        }
    }

    public void Show()
    {
        maskPanel.SetActive(false);
    }

    public void Refresh(CardModel cardModel)
    {
        hpText.text = cardModel.hp.ToString();
        atkText.text = cardModel.atk.ToString();
    }

    public void SetActiveSelectablePanel(bool isAttackFlag)
    {
        selectablePanel.SetActive(isAttackFlag);
    }
}
