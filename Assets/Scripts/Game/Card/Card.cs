using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Sprite flipSprite;
    public EExpansion eExpansion;
    public Button clickButton;
    public void CardFlip()
    {
        Sprite Temp = GetComponent<Image>().sprite;
        GetComponent<Image>().sprite = flipSprite;
        flipSprite = Temp;
    }

    public virtual void AwakeSet()
    {
        clickButton = GetComponent<Button>();
    }
}
