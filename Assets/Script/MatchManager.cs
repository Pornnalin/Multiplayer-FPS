using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    public enum EventCodes : byte
    {
        NewPlayer,
        ListPlayer,
        UpdateStat
    }
    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();
    private int index;
    // public EventCodes theEvent;
    private void Awake()
    {
        allPlayers = new List<PlayerInfo>();
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);//menu
        }
        else
        {
            NewPlayerSend(PhotonNetwork.NickName);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)//เหตุการณ์ที่กำหนดเองโดยผู้ใช้ ถ้ามากกว่า200คือเป็นของระบบ
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;
            //  Debug.Log("Receiver" + theEvent);

            switch (theEvent)
            {
                case EventCodes.NewPlayer:
                    NewPlayerReceive(data);
                    break;
                case EventCodes.ListPlayer:
                    ListPlayersReceive(data);
                    break;
                case EventCodes.UpdateStat:
                    UpdateStatsReceive(data);
                    break;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerSend(string username)
    {
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;
        PhotonNetwork.RaiseEvent(

            (byte)EventCodes.NewPlayer,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }

            );

    }
    public void NewPlayerReceive(object[] dataReceived)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceived[0], (int)dataReceived[1], (int)dataReceived[2], (int)dataReceived[3]);
        allPlayers.Add(player);

        ListPlayersSend();
    }
    public void ListPlayersSend()
    {
        object[] package = new object[allPlayers.Count];
        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] pice = new object[4];
            pice[0] = allPlayers[i].name;
            pice[1] = allPlayers[i].actor;
            pice[2] = allPlayers[i].kills;
            pice[3] = allPlayers[i].deaths;

            package[i] = pice;
        }
        PhotonNetwork.RaiseEvent(

           (byte)EventCodes.ListPlayer,
           package,
           new RaiseEventOptions { Receivers = ReceiverGroup.All },
           new SendOptions { Reliability = true }

           );
    }
    public void ListPlayersReceive(object[] dataReceived)
    {
        allPlayers.Clear();
        for (int i = 0; i < dataReceived.Length; i++)
        {
            object[] piece = (object[])dataReceived[i];
            PlayerInfo player = new PlayerInfo
           (

                (string)piece[0],
                (int)piece[1],
                (int)piece[2],
                (int)piece[3]
           );
            allPlayers.Add(player);
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i;
            }
        }
    }
    public void UpdateStatsSend(int actorSending, int statToUpdate, int amountToChange)
    {
        object[] package = new object[] { actorSending, statToUpdate, amountToChange };

        PhotonNetwork.RaiseEvent(
            (byte)EventCodes.UpdateStat,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }


            );
    }
    public void UpdateStatsReceive(object[] dataReceived)
    {
        int actor = (int)dataReceived[0];
        int statType = (int)dataReceived[1];
        int amount = (int)dataReceived[2];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                switch (statType)
                {
                    case 0://kills
                        allPlayers[i].kills += amount;
                        Debug.Log("Player" + allPlayers[i].name + ":kills" + allPlayers[i].kills);
                        break;

                    case 1://kills
                        allPlayers[i].deaths += amount;
                        Debug.Log("Player" + allPlayers[i].name + ":kills" + allPlayers[i].deaths);
                        break;
                }
            }
            break;
        }
    }
}
[System.Serializable]
public class PlayerInfo
{
    public string name;
    public int actor, kills, deaths;

    public PlayerInfo(string _name, int _actor, int _kills, int _deaths)
    {
        name = _name;
        actor = _actor;
        kills = _kills;
        deaths = _deaths;
    }
}
