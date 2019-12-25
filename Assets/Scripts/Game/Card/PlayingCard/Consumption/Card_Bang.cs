using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;
using PlayGM;
public class Card_Bang : ConsumptionCard
{
    public void Awake()
    {
        AwakeSet();
    }

    public override void AwakeSet()
    {
        base.AwakeSet();
        clickButton.onClick.AddListener(()=>Bang());
    }

    public void Bang()
    {
        if(PlayGameManager.GetGameManager.currentPlayPhase == EPlayPhase.ActionPhase)
        {
            PlayGameManager.GetGameManager.BangActive(name);
        }
    }
}
