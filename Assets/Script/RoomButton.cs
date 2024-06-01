using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomButton : MonoBehaviour
{
    public TMP_Text buttonText;
    private RoomInfo info;


    public void SetButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;
        buttonText.text = info.Name;
    }
    public void OpenRoom()
    {
        Launcher.instance.JoinRoom(info);
    }
}

