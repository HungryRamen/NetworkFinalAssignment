using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : Card
{
    public int maxHp;
    private int privatecurrentHp;
    public int currentHp
    {
        get{return privatecurrentHp;}
        set
        {
            privatecurrentHp = value;
            if(privatecurrentHp > maxHp)
            {
                privatecurrentHp = maxHp;
            }
            HpPosSet();
        }
    }
    public int myDistance;  // 나를 볼때 거리
    public int enemyDistance; // 적을 볼때 거리
    public int maxBang; // 뱅 최대 횟수
    public int currentBang; // 현제 뱅 횟수
    public override void AwakeSet()
    {
        base.AwakeSet();
        myDistance = 1;
        enemyDistance = 1;
        currentBang = 1;
    }

    protected void HpPosSet()
    {
        transform.localPosition = new Vector2(0.0f,(-145f / 5) * currentHp);
    }
}
