using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalData;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;
namespace PlayGM
{

    public class PlayGameManager : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        private static PlayGameManager gameManager;
        public EPlayPhase currentPlayPhase;
        public GameObject readyButton;
        public GameObject waitText;
        public GameObject durationTimeText;
        public PunTurnManager punTurnManager;
        public Deck deck;
        private int startTurn;

        float startDelay;
        private bool isReady;
        private List<ECharCardClass> charShuffleList = new List<ECharCardClass>();
        private Stack<ECharCardClass> charStack = new Stack<ECharCardClass>();
        public static PlayGameManager GetGameManager
        {
            get
            {
                if (gameManager == null)
                    gameManager = FindObjectOfType<PlayGameManager>();
                return gameManager;
            }
        }
        private void Awake()
        {
            GameObject playerObj = Instantiate(Resources.Load<GameObject>("Prefab/Player"));
            playerObj.transform.SetParent(GameObject.FindWithTag("Canvas").transform);  // 이 부분은 Player 별로 나눠서 다시 설정
            playerObj.transform.localPosition = new Vector2(0f, -320f);
            playerObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            playerObj.name = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
            GameObject enemyObj = Instantiate(Resources.Load<GameObject>("Prefab/Player"));
            enemyObj.transform.SetParent(GameObject.FindWithTag("Canvas").transform);  // 이 부분은 Player 별로 나눠서 다시 설정
            enemyObj.transform.localPosition = new Vector2(0f, 320f);
            enemyObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            enemyObj.name = PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "2" : "1";
            if (PhotonNetwork.IsMasterClient)
                startTurn = Random.Range(0, 2);
            OnPhase(EPlayPhase.JoinUnReadyPhase);
        }

        public void OnEvent(EventData photonEvent)
        {
            ERaiseEvent raiseEvent = (ERaiseEvent)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;
            switch (raiseEvent)
            {
                case ERaiseEvent.CardDrawSend:
                    ERaiseEvent sendRaiseEvent = ERaiseEvent.CardDrawRecv;
                    PlayingCardData cardData = deck.MasterDraw();
                    Debug.LogFormat("송신 {0} : {1}", (int)data[0], cardData.CardCode);
                    object[] contents = new object[] { (int)data[0], cardData.CardCode, cardData.SpriteName, cardData.SpriteCode };
                    RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);
                    break;
                case ERaiseEvent.CardDrawRecv:
                    PlayingCardData recvCardData = new PlayingCardData((int)data[1], (string)data[2], (int)data[3]);
                    deck.Draw(recvCardData, (int)data[0]);
                    break;
                case ERaiseEvent.ReadySend:
                    isReady = true;
                    break;
                case ERaiseEvent.JobDrawSend:
                    JobSet((int)data[0] + 1 == PhotonNetwork.MasterClient.ActorNumber ? 1 : 2, EJobCardClass.Vice);
                    JobSet((int)data[0] + 1 != PhotonNetwork.MasterClient.ActorNumber ? 1 : 2, EJobCardClass.Fuorilegge);
                    OnPhase(EPlayPhase.CharaterPhase);
                    break;
                case ERaiseEvent.CharDrawSend:
                    CharDeckDraw(1, (ECharCardClass)data[0]);
                    CharDeckDraw(2, (ECharCardClass)data[1]);
                    OnPhase(EPlayPhase.DeckPhase);
                    break;
                case ERaiseEvent.DeckSetSend:
                    deck.gameObject.SetActive(true);
                    OnPhase(EPlayPhase.GamseStartPhase);
                    break;
                case ERaiseEvent.BangAttackSend:
                    OnPhase(EPlayPhase.ReActionPhase);
                    punTurnManager.Wait = true;
                    break;
                    case ERaiseEvent.BangAttackRecv:
                    GameObject.Find(((int)data[0]).ToString()).GetComponentInChildren<CharacterCard>().currentHp -= 1;
                    punTurnManager.Wait = false;
                    if(PhotonNetwork.CurrentRoom.GetTurn() + 1 == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        OnPhase(EPlayPhase.ActionPhase);
                    }
                    else
                    {
                        OnPhase(EPlayPhase.WaitPhase);
                    }
                    if(PhotonNetwork.IsMasterClient)
                    {

                        CardGraveYardInput((string)data[0]);
                        PhotonNetwork.CurrentRoom.SetTurn(PhotonNetwork.CurrentRoom.GetTurn(), true);
                    }
                    break;
            }
        }
        private void Update()
        {
            if (isReady && EPlayPhase.JoinWaitPhase == currentPlayPhase)
                OnPhase(EPlayPhase.JobPhase);
            if (EPlayPhase.GamseStartPhase == currentPlayPhase)
            {
                startDelay += Time.deltaTime;
                if (startDelay >= 0.5f)
                {
                    OnPhase(EPlayPhase.GameStartWaitPhase);
                }
            }
            if(punTurnManager.Wait)
            {
                if(punTurnManager.IsWaitOver || currentPlayPhase == EPlayPhase.ReActionPhase)
                {
                    ERaiseEvent sendRaiseEvent = ERaiseEvent.BangAttackRecv;
                    object[] contents = new object[] {PhotonNetwork.LocalPlayer.ActorNumber};
                    RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    SendOptions sendOptions = new SendOptions { Reliability = true };
                    RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);
                    PhotonNetwork.CurrentRoom.SetTurn(PhotonNetwork.CurrentRoom.GetTurn(), true);
                    punTurnManager.Wait = false;
                }
            }
            if (!PhotonNetwork.IsMasterClient)
                return;
            if (punTurnManager.IsOver)
            {
                int turn = PhotonNetwork.PlayerList[PhotonNetwork.CurrentRoom.GetTurn()].GetNext().ActorNumber - 1 % PhotonNetwork.PlayerList.Length;
                PhotonNetwork.CurrentRoom.SetTurn(turn, true);
            }
        }

        public void ShuffleList<T>(List<T> list)
        {
            int count = list.Count;
            int r1 = Random.Range(0, count);
            int r2 = Random.Range(0, count);
            T temp;
            for (int i = 0; i < count; i++)
            {
                temp = list[r1];
                list[r1] = list[r2];
                list[r2] = temp;

                r1 = Random.Range(0, count);
                r2 = Random.Range(0, count);
            }
        }

        public void RaiseEventSend(ERaiseEvent code, object[] content, RaiseEventOptions raiseEventOptions, SendOptions sendOption)
        {
            PhotonNetwork.RaiseEvent((byte)code, content, raiseEventOptions, sendOption);
        }

        private void OnPhase(EPlayPhase playPhase)
        {
            currentPlayPhase = playPhase;
            switch (currentPlayPhase)
            {
                case EPlayPhase.JoinUnReadyPhase:
                    isReady = false;
                    readyButton.SetActive(true);
                    break;
                case EPlayPhase.JoinReadyPhase:
                    {

                        ERaiseEvent sendRaiseEvent = ERaiseEvent.ReadySend;
                        RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                        SendOptions sendOptions = new SendOptions { Reliability = true };
                        RaiseEventSend(sendRaiseEvent, null, sendRaiseEventOptions, sendOptions);
                        OnPhase(EPlayPhase.JoinWaitPhase);
                        readyButton.SetActive(false);
                        waitText.SetActive(true);
                    }
                    break;
                case EPlayPhase.JoinWaitPhase:
                    break;
                case EPlayPhase.JobPhase:
                    waitText.SetActive(false);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        ERaiseEvent sendRaiseEvent = ERaiseEvent.JobDrawSend;
                        object[] contents = new object[] { startTurn };
                        RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                        SendOptions sendOptions = new SendOptions { Reliability = true };
                        RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);
                        JobSet(startTurn + 1 == PhotonNetwork.MasterClient.ActorNumber ? 1 : 2, EJobCardClass.Vice);
                        JobSet(startTurn + 1 != PhotonNetwork.MasterClient.ActorNumber ? 1 : 2, EJobCardClass.Fuorilegge);
                        OnPhase(EPlayPhase.CharaterPhase);
                    }
                    break;
                case EPlayPhase.CharaterPhase:
                    {

                        if (PhotonNetwork.IsMasterClient)
                        {
                            CharDeckSet();
                            CharDeckShuffle();
                            ECharCardClass myChar = CharDeckMasterDraw();
                            ECharCardClass enemyChar = CharDeckMasterDraw();
                            ERaiseEvent sendRaiseEvent = ERaiseEvent.CharDrawSend;
                            object[] contents = new object[] { myChar, enemyChar };
                            RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                            SendOptions sendOptions = new SendOptions { Reliability = true };
                            RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);
                            CharDeckDraw(1, myChar);
                            CharDeckDraw(2, enemyChar);
                            OnPhase(EPlayPhase.DeckPhase);
                        }
                    }
                    break;
                case EPlayPhase.DeckPhase:
                    {
                        if (PhotonNetwork.IsMasterClient)
                        {
                            ERaiseEvent sendRaiseEvent = ERaiseEvent.DeckSetSend;
                            RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
                            SendOptions sendOptions = new SendOptions { Reliability = true };
                            RaiseEventSend(sendRaiseEvent, null, sendRaiseEventOptions, sendOptions);
                            deck.gameObject.SetActive(true);
                            deck.DeckSet(EExpansionFlags.Original);
                            deck.DeckShuffle();
                            OnPhase(EPlayPhase.GamseStartPhase);
                        }
                    }
                    break;
                case EPlayPhase.GamseStartPhase:
                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.CurrentRoom.SetTurn(startTurn, false);
                        int count = GameObject.Find("1").GetComponentInChildren<CharacterCard>().maxHp;
                        ERaiseEvent sendRaiseEvent = ERaiseEvent.CardDrawRecv;
                        RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                        SendOptions sendOptions = new SendOptions { Reliability = true };
                        for (int i = 0; i < count; i++)
                        {
                            PlayingCardData cardData = deck.MasterDraw();
                            object[] contents = new object[] { 1, cardData.CardCode, cardData.SpriteName, cardData.SpriteCode };
                            RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);

                        }
                        count = GameObject.Find("2").GetComponentInChildren<CharacterCard>().maxHp;
                        for (int i = 0; i < count; i++)
                        {
                            PlayingCardData cardData = deck.MasterDraw();
                            object[] contents = new object[] { 2, cardData.CardCode, cardData.SpriteName, cardData.SpriteCode };
                            RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);
                        }
                    }
                    break;
                case EPlayPhase.GameStartWaitPhase:

                    if (PhotonNetwork.CurrentRoom.GetTurn() + 1 != PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        OnPhase(EPlayPhase.WaitPhase);
                    }
                    else
                    {
                        OnPhase(EPlayPhase.CardDrawPhase);
                    }
                    if (PhotonNetwork.IsMasterClient)
                        PhotonNetwork.CurrentRoom.SetTurn(startTurn, true);
                    durationTimeText.SetActive(true);
                    break;
                case EPlayPhase.CardFlipPhase:
                    break;
                case EPlayPhase.CardDrawPhase:
                    {
                        CharacterCard myChar = GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber.ToString()).GetComponentInChildren<CharacterCard>();
                        myChar.currentBang = myChar.maxBang;
                        for (int i = 0; i < 2; i++)
                        {
                            deck.DrawSend();

                        }
                        OnPhase(EPlayPhase.ActionPhase);
                    }
                    break;
                case EPlayPhase.ActionPhase:

                    break;
                case EPlayPhase.ClenUpPhase:
                    break;
                case EPlayPhase.WaitPhase:
                    break;
                case EPlayPhase.ReActionPhase:
                    break;
                case EPlayPhase.GameEndPhase:
                    break;
                case EPlayPhase.QuitPhase:
                    break;
            }
        }

        public void WrapPhase(int phase)
        {
            OnPhase((EPlayPhase)phase);
        }

        public void JobSet(int actorNumber, EJobCardClass cardClass)
        {
            GameObject cardObj = Instantiate(Resources.Load<GameObject>("Prefab/Card"));
            switch (cardClass)
            {
                case EJobCardClass.Vice:
                    cardObj.AddComponent<Job_Vice>();
                    break;
                case EJobCardClass.Fuorilegge:
                    cardObj.AddComponent<Job_Fuorilegge>();
                    break;
            }
            GameObject player = GameObject.Find(actorNumber.ToString());
            GameObject job = player.GetComponentInChildren<Job>().gameObject;
            cardObj.transform.SetParent(job.transform);
            cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            cardObj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            cardObj.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Job/Job")[(int)cardClass];
        }

        void CharDeckSet()
        {
            charShuffleList.Clear();
            charShuffleList.Add(ECharCardClass.BartCassidy);
            charShuffleList.Add(ECharCardClass.CalamityJanet);
            //charShuffleList.Add(ECharCardClass.Jourdonnais);
            charShuffleList.Add(ECharCardClass.PaulRegret);
            charShuffleList.Add(ECharCardClass.RoseDollan);
            //charShuffleList.Add(ECharCardClass.SidKetchum);
            //charShuffleList.Add(ECharCardClass.SlabTheKiller);
            charShuffleList.Add(ECharCardClass.SuzyLafayette);
            charShuffleList.Add(ECharCardClass.WillyTheKid);
        }
        void CharDeckShuffle()
        {
            ShuffleList(charShuffleList);
            charStack.Clear();
            foreach (ECharCardClass data in charShuffleList)
            {
                charStack.Push(data);
            }
        }

        ECharCardClass CharDeckMasterDraw()
        {
            return charStack.Pop();
        }

        void CharDeckDraw(int actorNumber, ECharCardClass charCardClass)
        {
            GameObject cardObj = Instantiate(Resources.Load<GameObject>("Prefab/Card"));
            GameObject player = GameObject.Find(actorNumber.ToString());
            GameObject charLife = player.GetComponentInChildren<CharLife>().gameObject;
            cardObj.transform.SetParent(charLife.transform);
            cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            cardObj.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>("Character/Character")[(int)charCardClass];
            switch (charCardClass)
            {
                case ECharCardClass.WillyTheKid:
                    cardObj.AddComponent<Char_WillyTheKid>();
                    break;
                case ECharCardClass.SuzyLafayette:
                    cardObj.AddComponent<Char_SuzyLafayette>();
                    break;
                case ECharCardClass.SlabTheKiller:
                    cardObj.AddComponent<Char_SlabTheKiller>();
                    break;
                case ECharCardClass.SidKetchum:
                    cardObj.AddComponent<Char_SidKetchum>();
                    break;
                case ECharCardClass.RoseDollan:
                    cardObj.AddComponent<Char_RoseDollan>();
                    break;
                case ECharCardClass.PaulRegret:
                    cardObj.AddComponent<Char_PaulRegret>();
                    break;
                case ECharCardClass.Jourdonnais:
                    cardObj.AddComponent<Char_Jourdonnais>();
                    break;
                case ECharCardClass.CalamityJanet:
                    cardObj.AddComponent<Char_CalamityJanet>();
                    break;
                case ECharCardClass.BartCassidy:
                    cardObj.AddComponent<Char_BartCassidy>();
                    break;
            }
        }
        public EPlayingCardClass KeyCodeToCard(int keyCode)
        {
            if (keyCode <= 23)
            {
                return EPlayingCardClass.Bang;
            }
            else if (keyCode <= 25)
            {
                return EPlayingCardClass.Barrel;
            }
            else if (keyCode <= 31)
            {
                return EPlayingCardClass.Beer;
            }
            else if (keyCode <= 35)
            {
                return EPlayingCardClass.CatBalou;
            }
            else if (keyCode <= 37)
            {
                return EPlayingCardClass.Stagecoach;
            }
            else if (keyCode <= 38)
            {
                return EPlayingCardClass.Dynamite;
            }
            else if (keyCode <= 41)
            {
                return EPlayingCardClass.Duel;
            }
            else if (keyCode <= 43)
            {
                return EPlayingCardClass.GeneralStore;
            }
            else if (keyCode <= 44)
            {
                return EPlayingCardClass.Gatling;
            }
            else if (keyCode <= 46)
            {
                return EPlayingCardClass.Indian;
            }
            else if (keyCode <= 48)
            {
                return EPlayingCardClass.Miss;
            }
            else if (keyCode <= 51)
            {
                return EPlayingCardClass.Jail;
            }
            else if (keyCode <= 52)
            {
                return EPlayingCardClass.Remington;
            }
            else if (keyCode <= 53)
            {
                return EPlayingCardClass.Carabine;
            }
            else if (keyCode <= 54)
            {
                return EPlayingCardClass.Saloon;
            }
            else if (keyCode <= 57)
            {
                return EPlayingCardClass.Schofield;
            }
            else if (keyCode <= 59)
            {
                return EPlayingCardClass.Volcanic;
            }
            else if (keyCode <= 60)
            {
                return EPlayingCardClass.WellsFargo;
            }
            else if (keyCode <= 61)
            {
                return EPlayingCardClass.Winchester;
            }
            else if (keyCode <= 70)
            {
                return EPlayingCardClass.Miss;
            }
            else if (keyCode <= 71)
            {
                return EPlayingCardClass.Scope;
            }
            else if (keyCode <= 73)
            {
                return EPlayingCardClass.Mustang;
            }
            else if (keyCode <= 77)
            {
                return EPlayingCardClass.Panic;
            }
            else if (keyCode <= 78)
            {
                return EPlayingCardClass.Miss;
            }
            else if (keyCode <= 79)
            {
                return EPlayingCardClass.Bang;
            }
            return EPlayingCardClass.None;
        }
        public void BangActive(string keycode)
        {
            CharacterCard myChar = GameObject.Find(PhotonNetwork.LocalPlayer.ActorNumber.ToString()).GetComponentInChildren<CharacterCard>();
            if(myChar.currentBang <= 0)
                return;
            if(myChar.enemyDistance < GameObject.Find((PhotonNetwork.LocalPlayer.ActorNumber == 1 ? "2" : "1")).GetComponentInChildren<CharacterCard>().myDistance)
                return;
            ERaiseEvent sendRaiseEvent = ERaiseEvent.BangAttackSend;
            object[] contents = new object[]{keycode};
            RaiseEventOptions sendRaiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            RaiseEventSend(sendRaiseEvent, contents, sendRaiseEventOptions, sendOptions);
            OnPhase(EPlayPhase.WaitPhase);
            punTurnManager.Wait = true;
            myChar.currentBang -= 1;
        }

        public void CardGraveYardInput(string keyCode)
        {
            int cast = System.Convert.ToInt32(keyCode);
            if(deck.deckDictionary.ContainsKey(cast))
            {
                Debug.Log("묘지 카드 넣기 실패");
                return;
            }
            deck.CardGraveyard(deck.deckDictionary[cast]);
        }
    }

}