using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_SidKetchum : CharacterCard
{
    private void Awake() 
    {
        AwakeSet();
    }
    public override void AwakeSet()
    {
        base.AwakeSet();
        maxHp = 4;
        currentHp = maxHp;
        HpPosSet();
    }
}
