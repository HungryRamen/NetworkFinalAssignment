using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardLayerSort : MonoBehaviour
{
    public void CardPosSet()
    {
        Image[] cards = GetComponentsInChildren<Image>();   // 카드 받아오기
        if(cards == null) // 카드가 없다면 return
            return;
        if((cards.Length - 1) * 100 < 270) // 카드 정렬
        {
            for (int i = 0; i < cards.Length;i++)
            {
                cards[i].gameObject.transform.localPosition = new Vector2(i * 100.0f,0.0f);
            }
        }
        else
        {
            float x = 270.0f / cards.Length;
            for (int i = 0; i < cards.Length;i++)
            {
                cards[i].gameObject.transform.localPosition = new Vector2(i * x,0.0f);
            }
        }
    }
}
