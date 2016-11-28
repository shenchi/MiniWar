using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RoomList : MonoBehaviour {

    public RoomBroadcaster broadcaster;

    public UIList list;

	// Use this for initialization
	void Start () {
        var mgr = NetworkManager.singleton;

        broadcaster.broadcastData = string.Concat(new object[]
        {
            "MiniWarRoom:",
            mgr.networkAddress,
            ':',
            mgr.networkPort,
            ":Client"
        });

        broadcaster.OnBroadcast += OnBroadcast;

        broadcaster.Initialize();
        broadcaster.StartAsClient();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();

    void OnBroadcast(string address, string data)
    {
        if (!rooms.ContainsKey(address))
        {
            GameObject go = list.Add(data);
            rooms.Add(address, go);
        }

        rooms[address].SendMessage("SetData", data, SendMessageOptions.DontRequireReceiver);
    }
}
