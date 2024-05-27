using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public GameObject menuBtn;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject roomScreen;
    public TMP_Text roomNameText;

    public GameObject errorScreen;
    public TMP_Text errorText;
    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        CloseMenu();
        loadingScreen.SetActive(true);
        loadingText.text = "Connecting To Network...";
        PhotonNetwork.ConnectUsingSettings();
    }
    void CloseMenu()
    {
        loadingScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        menuBtn.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master");
        PhotonNetwork.JoinLobby();
        loadingText.text = "Join Lobby";
        Debug.Log("Join Lobby");

    }
    public override void OnJoinedLobby()
    {
        CloseMenu();
        menuBtn.SetActive(true);
    }
    public void OpenRoomCreate()
    {
        CloseMenu();
        createRoomScreen.SetActive(true);

    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions option = new RoomOptions();
            option.MaxPlayers = 8;
            PhotonNetwork.CreateRoom(roomNameInput.text, option);
            CloseMenu();
            loadingText.text = "Create room...";
            loadingScreen.SetActive(true);
            Debug.Log("Create");

        }
    }
    public override void OnJoinedRoom()
    {
        CloseMenu();
        roomScreen.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("JoinRoom");

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Fail To Create Room : " + message;
        CloseMenu();
        errorScreen.SetActive(true);
    }
    public void CloseErrorScreen()
    {
        CloseMenu();
        menuBtn.SetActive(true);
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenu();
        loadingText.text = "Leave Room...";
        loadingScreen.SetActive(true);
    }
    public override void OnLeftRoom()
    {
        CloseMenu();
        menuBtn.SetActive(true);
        Debug.Log("leave");
    }
}





