using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    public GameObject loadingScreen;
    public TMP_Text loadingText;
    public GameObject menuBtn;

    public GameObject createRoomScreen;
    public TMP_InputField roomNameInput;

    public GameObject roomPanel;
    public TMP_Text roomNameText, playerNameLabel;
    private List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    public GameObject errorScreen;
    public TMP_Text errorText;

    public GameObject roomBrowserScreen;
    public RoomButton theRoomButton;
    [SerializeField] private List<RoomButton> allRoomButtons = new List<RoomButton>();

    public GameObject nameInputScreen;
    public TMP_InputField nameInputField;
    private bool hasSetNick;
    public string levelToPlay;
    public Button startGameBtn;

    public GameObject roomTestButton;
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

#if UNITY_EDITOR
        roomTestButton.SetActive(true);
#endif
    }
    void CloseMenu()
    {
        loadingScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        menuBtn.SetActive(false);
        roomPanel.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect to Master");
        PhotonNetwork.JoinLobby();
        loadingText.text = "Join Lobby";
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Join Lobby");

    }
    public override void OnJoinedLobby()
    {
        CloseMenu();
        menuBtn.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if (!hasSetNick)
        {
            CloseMenu();
            nameInputScreen.SetActive(true);
            if (PlayerPrefs.HasKey("playerName"))
            {
                nameInputField.text = PlayerPrefs.GetString("playerName");
            }

        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
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
        roomPanel.SetActive(true);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        Debug.Log("JoinRoom");
        ListAllPlayer();

        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.interactable = true;
        }
        else
        {            
            startGameBtn.interactable = false;
        }
    }
    private void ListAllPlayer()
    {
        foreach (TMP_Text player in allPlayerNames)
        {
            Destroy(player.gameObject);
        }
        allPlayerNames.Clear();
        Player[] players = PhotonNetwork.PlayerList;
        playerNameLabel.gameObject.SetActive(false);

        for (int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName.ToString();
            newPlayerLabel.gameObject.SetActive(true);
            allPlayerNames.Add(newPlayerLabel);
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName.ToString();
        newPlayerLabel.gameObject.SetActive(true);
        allPlayerNames.Add(newPlayerLabel);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {        
       // allPlayerNames.RemoveAll(name => name.text == otherPlayer.NickName.ToString());
        ListAllPlayer();
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
    public void OpenRoomBrowser()
    {
        CloseMenu();
        roomBrowserScreen.SetActive(true);
    }
    public void CloseRoomBrowser()
    {
        CloseMenu();
        menuBtn.SetActive(true);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
                newButton.SetButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);

                allRoomButtons.Add(newButton);
            }
        }
    }
    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);
        CloseMenu();
        loadingText.text = "Joining Room...";
        loadingScreen.SetActive(true);
    }
    public void SetNickName()
    {
        if (!string.IsNullOrEmpty(nameInputField.text))
        {
            PhotonNetwork.NickName = nameInputField.text;

            PlayerPrefs.SetString("playerName", nameInputField.text);

            CloseMenu();
            menuBtn.SetActive(true);
            hasSetNick = true;
        }
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(levelToPlay);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.interactable = true;
        }
        else
        {
            startGameBtn.interactable = false;
        }
    }

    public void QuickJoin()
    {
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = 8;
        PhotonNetwork.CreateRoom("Test", option);
        CloseMenu();
        loadingText.text = "Create Room...";
        loadingScreen.SetActive(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}





