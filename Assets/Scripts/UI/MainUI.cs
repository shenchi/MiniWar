using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public GameManager manager;
    public RoomBroadcaster broadcaster;

    public GameObject mainScreen;
    public GameObject lobby;
    public GameObject roomList;

    public void CreateGame()
    {
        try
        {
            if (broadcaster.running)
                broadcaster.StopBroadcast();
        }
        catch
        {
            Debug.LogWarning("Broadcaster Failed.");
        }
        manager.StartHost();
    }

    public void RoomList()
    {
        
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
