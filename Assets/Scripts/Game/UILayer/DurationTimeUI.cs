using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalData;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Text;
public class DurationTimeUI : MonoBehaviour
{
    Text durationText;
    public PunTurnManager punTurnManager;
    private void Awake()
    {
        durationText = GetComponent<Text>();
        durationText.text = DurationTimeStr();
    }

    private void Update()
    {
        durationText.text = DurationTimeStr();
    }

    public string DurationTimeStr()
    {
        durationText = GetComponent<Text>();
        StringBuilder strText = new StringBuilder();
        if (punTurnManager.Wait)
        {
            if (PhotonNetwork.CurrentRoom.GetTurn() + 1 == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                strText.Append("상대의 반응");
            }
            else
            {
                strText.Append("나의 반응");
            }
            strText.AppendFormat("남은시간 : {0}", punTurnManager.WaitSecondsInTurn.ToString("F1"));
        }
        else
        {

            if (PhotonNetwork.CurrentRoom.GetTurn() + 1 != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                strText.Append("상대 ");
            }
            else
            {
                strText.Append("나의 ");
            }
            strText.AppendFormat("남은시간 : {0}", punTurnManager.RemainingSecondsInTurn.ToString("F1"));
        }
        return strText.ToString();
    }
}
