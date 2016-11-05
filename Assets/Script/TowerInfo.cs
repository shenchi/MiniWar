using UnityEngine;
using System.Collections;

public class TowerInfo : MonoBehaviour
{
    public bool destroyWait;
    public bool consumeWait;

    public int health;
    public int level;
    public int energy;
    public int range;
    public int cost;
    public int team;

    public int consumption;

    public PlayerInfo player;

    public bool wait;

    public bool manualDestroy = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((health <= 0 || manualDestroy) && !destroyWait)
        {
            destroy();
            destroyWait = true;
        }

        if (!consumeWait)
        {
            player.resource -= consumption;
            consumeWait = true;
        }
    }

    public void destroy()
    {

    }

    public void manuallyDestroy()
    {

    }
}
