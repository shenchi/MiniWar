using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class RoomItem : MonoBehaviour
{
    public Text roomName;

    private string address = null;
    private int port;

    public void SetData(object data)
    {
        var str = data as string;
        if (string.IsNullOrEmpty(str))
            return;

        var arr = str.Split(':');
        if (arr.Length != 4 || arr[0] != "MiniWarRoom")
            return;

        address = arr[1];
        port = Convert.ToInt32(arr[2]);
        roomName.text = arr[3];
    }

    public void OnButtonJoin()
    {
        if (string.IsNullOrEmpty(address))
            return;

        var mgr = NetworkManager.singleton;
        mgr.networkAddress = address;
        mgr.networkPort = port;
        mgr.StartClient();
    }
}
