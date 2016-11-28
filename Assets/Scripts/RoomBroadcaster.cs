using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class RoomBroadcaster : NetworkDiscovery
{
    public delegate void OnBroadcastDelegate(string address, string data);

    public OnBroadcastDelegate OnBroadcast;

    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        base.OnReceivedBroadcast(fromAddress, data);
        OnBroadcast(fromAddress, data);
    }

    private static string BytesToString(byte[] bytes)
    {
        char[] array = new char[bytes.Length / 2];
        Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
        return new string(array);
    }

    void OnGUI()
    {
        if (!showGUI)
        {
            return;
        }

        int num = 10 + offsetX;
        int num2 = 40 + offsetY;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            return;
        }
        
        if (running)
        {
            if (isClient)
            {
                GUI.Label(new Rect((float)num, (float)num2, 200f, 20f), "Rooms (in LAN):");
                num2 += 24;
            }

            if (broadcastsReceived != null)
            {
                foreach (string current in broadcastsReceived.Keys)
                {
                    NetworkBroadcastResult networkBroadcastResult = broadcastsReceived[current];
                    if (GUI.Button(new Rect((float)num, (float)(num2 + 20), 200f, 20f), "Game at " + current) && useNetworkManager)
                    {
                        string text = BytesToString(networkBroadcastResult.broadcastData);
                        string[] array = text.Split(new char[]
                        {
                                ':'
                        });
                        if (array.Length == 3 && array[0] == "NetworkManager" && NetworkManager.singleton != null && NetworkManager.singleton.client == null)
                        {
                            NetworkManager.singleton.networkAddress = array[1];
                            NetworkManager.singleton.networkPort = Convert.ToInt32(array[2]);
                            NetworkManager.singleton.StartClient();
                        }
                    }
                    num2 += 24;
                }
            }
        }
    }
}
