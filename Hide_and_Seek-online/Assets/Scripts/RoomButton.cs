using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using TMPro;

public class RoomButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomNameText;

    private RoomInfo _roomInfo;

    public void SetButtonDetails(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        _roomNameText.text = _roomInfo.Name;
    }

    public void OpenRoom()
    {
        Launcher.Instance.JoinRoom(_roomInfo);
    }
}
