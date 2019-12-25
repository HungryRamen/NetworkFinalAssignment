using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_PaulRegret : CharacterCard
{
    private void Awake() 
    {
        AwakeSet();
    }
    public override void AwakeSet()
    {
        base.AwakeSet();
        maxHp = 3;
        currentHp = maxHp;
        myDistance += 1;
        HpPosSet();
    }
}
