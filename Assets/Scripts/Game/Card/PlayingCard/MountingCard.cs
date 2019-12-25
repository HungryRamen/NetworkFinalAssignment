using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;

public class MountingCard : PlayingCard
{
    public override void AwakeSet()
    {
        base.AwakeSet();
        ePlayerType = EPlayType.Mounting;
    }
}
