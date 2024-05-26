using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public GameObject menuBtn;
    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;
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
}



