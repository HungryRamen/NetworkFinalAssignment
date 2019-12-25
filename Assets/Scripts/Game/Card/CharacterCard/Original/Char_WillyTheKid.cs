using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_WillyTheKid : CharacterCard
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
        maxBang = int.MaxValue;
        HpPosSet();
    }
}
