using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnoCard : MonoBehaviour
{
    public string card;
    public Image backgroundColor;
    public TMP_Text cardNumber;
    public Button playButton;
        
    public void SetCard(string card)
    {
        print("setting card to " + card);
        this.card = card;
        if (card.EndsWith("R"))
        {
            backgroundColor.color = Color.red;
        }
        if (card.EndsWith("Y"))
        {
            backgroundColor.color = Color.yellow;
        }
        if (card.EndsWith("G"))
        {
            backgroundColor.color = Color.green;
        }
        if (card.EndsWith("B"))
        {
            backgroundColor.color = Color.blue;
        }

        cardNumber.text = card.Substring(0, 1);
    }
}
