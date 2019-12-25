using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData;
using PlayGM;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class Deck : MonoBehaviourPunCallbacks
{
    public Dictionary<int,PlayingCardData> deckDictionary = new Dictionary<int, PlayingCardData>();
    private List<PlayingCardData> deckShuffleList = new List<PlayingCardData>();
    private Stack<PlayingCardData> deckStack = new Stack<PlayingCardData>();

    private void Start()
    {
    }
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.GetTurn() + 1 != PhotonNetwork.LocalPlayer.ActorNumber)
            return;
    }
    public void DeckSet(EExpansionFlags expansionFlags) // 비트 플래그로 카드 추가
    {
        deckShuffleList.Clear();
        if (expansionFlags == 0)
            return;
        if ((expansionFlags & EExpansionFlags.Original) == EExpansionFlags.Original)
        {
            int cardCode = 0;
            for (int i = 20; i <= 68; i++)
            {
                if(i == 44|| i == 45 || i == 58|| i==39||i==40||i == 41 || i== 62|| i==63|| i == 64 || i == 65 || i == 66)
                {
                    cardCode++;
                    continue;
                }
                deckDictionary.Add(cardCode, new PlayingCardData(cardCode, "PlayCard1", i));
                cardCode++;
            }
            for (int i = 20; i <= 50; i++)
            {
                if(i == 20||i==21||i==22)
                {
                    cardCode++;
                    continue;
                }
                deckDictionary.Add(cardCode, new PlayingCardData(cardCode, "PlayCard2", i));
                cardCode++;
            }
        }
        if ((expansionFlags & EExpansionFlags.DodgeCity) == EExpansionFlags.DodgeCity)
        {

        }
        if ((expansionFlags & EExpansionFlags.WWS) == EExpansionFlags.WWS)
        {

        }
        if ((expansionFlags & EExpansionFlags.GoldRush) == EExpansionFlags.GoldRush)
        {

        }
        if ((expansionFlags & EExpansionFlags.TheValleyOfShadows) == EExpansionFlags.TheValleyOfShadows)
        {

        }
        if ((expansionFlags & EExpansionFlags.Privilege) == EExpansionFlags.Privilege)
        {

        }
        foreach(KeyValuePair<int,PlayingCardData> item in deckDictionary)
        {
            deckShuffleList.Add(item.Value);
        }
    }
    public void DrawSend()
    {
        ERaiseEvent raiseEvent = ERaiseEvent.CardDrawSend;
        object[] contents = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PlayGameManager.GetGameManager.RaiseEventSend(raiseEvent, contents, raiseEventOptions, sendOptions);
    }
    public PlayingCardData MasterDraw()
    {
        if (deckStack.Count == 0)
        {
            DeckShuffle();
        }
        return deckStack.Pop();
    }

    public void Draw(PlayingCardData cardData, int actorNumber)
    {
        EPlayingCardClass cardClass = PlayGameManager.GetGameManager.KeyCodeToCard(cardData.CardCode);
        GameObject cardObj = Instantiate(Resources.Load<GameObject>("Prefab/Card"));
        GameObject player = GameObject.Find(actorNumber.ToString());
        CardLayerSort layer = player.GetComponentInChildren<HandCard>().gameObject.GetComponent<CardLayerSort>();
        cardObj.transform.SetParent(layer.transform);
        cardObj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        cardObj.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(string.Format("PlayCard/{0}", cardData.SpriteName))[cardData.SpriteCode];
        switch (cardClass)
        {
            case EPlayingCardClass.Bang:
                cardObj.AddComponent<Card_Bang>();
                break;
            case EPlayingCardClass.Miss:
                cardObj.AddComponent<Card_Miss>();
                break;
            case EPlayingCardClass.Beer:
                cardObj.AddComponent<Card_Beer>();
                break;
            case EPlayingCardClass.Duel:
                cardObj.AddComponent<Card_Duel>();
                break;
            case EPlayingCardClass.Indian:
                cardObj.AddComponent<Card_Indian>();
                break;
            case EPlayingCardClass.Gatling:
                cardObj.AddComponent<Card_Gatling>();
                break;
            case EPlayingCardClass.Saloon:
                cardObj.AddComponent<Card_Saloon>();
                break;
            case EPlayingCardClass.Panic:
                cardObj.AddComponent<Card_Panic>();
                break;
            case EPlayingCardClass.CatBalou:
                cardObj.AddComponent<Card_CatBalou>();
                break;
            case EPlayingCardClass.GeneralStore:
                cardObj.AddComponent<Card_GeneralStore>();
                break;
            case EPlayingCardClass.Stagecoach:
                cardObj.AddComponent<Card_Stagecoach>();
                break;
            case EPlayingCardClass.WellsFargo:
                cardObj.AddComponent<Card_WellsFargo>();
                break;
            case EPlayingCardClass.Schofield:
                cardObj.AddComponent<Card_Schofield>();
                break;
            case EPlayingCardClass.Remington:
                cardObj.AddComponent<Card_Remington>();
                break;
            case EPlayingCardClass.Carabine:
                cardObj.AddComponent<Card_Carbine>();
                break;
            case EPlayingCardClass.Winchester:
                cardObj.AddComponent<Card_Winchester>();
                break;
            case EPlayingCardClass.Volcanic:
                cardObj.AddComponent<Card_Volcanic>();
                break;
            case EPlayingCardClass.Scope:
                cardObj.AddComponent<Card_Scope>();
                break;
            case EPlayingCardClass.Mustang:
                cardObj.AddComponent<Card_Mustang>();
                break;
            case EPlayingCardClass.Barrel:
                cardObj.AddComponent<Card_Barrel>();
                break;
            case EPlayingCardClass.Jail:
                cardObj.AddComponent<Card_Jail>();
                break;
            case EPlayingCardClass.Dynamite:
                cardObj.AddComponent<Card_Dynamite>();
                break;
        }
        cardObj.name = cardData.CardCode.ToString();
        layer.CardPosSet();
    }
    public void DeckShuffle()
    {
        PlayGameManager.GetGameManager.ShuffleList(deckShuffleList);
        deckStack.Clear();
        foreach (PlayingCardData data in deckShuffleList)
        {
            deckStack.Push(data);
        }
        deckShuffleList.Clear();
    }

    public void CardGraveyard(PlayingCardData cardData)
    {
        deckShuffleList.Add(cardData);
    }
}
