using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;

public class ConsumptionCard : PlayingCard
{
    public override void AwakeSet()
    {
        base.AwakeSet();
        ePlayerType = EPlayType.Consumption;
    }
}
