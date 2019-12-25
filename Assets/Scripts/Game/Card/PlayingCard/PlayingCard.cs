using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;
using Photon.Pun;
public class PlayingCard : Card
{
    public EPlayType ePlayerType;
    public virtual void Use()
    {

    }

    public override void AwakeSet()
    {
        base.AwakeSet();
        flipSprite = Resources.Load<Sprite>("PlayCard/PlayCardBack");
        if(System.Convert.ToInt32(gameObject.GetComponentInParent<PlayerBoard>().name) != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            CardFlip();
        }
    }
}