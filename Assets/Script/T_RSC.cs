using UnityEngine;
using System.Collections;

public class T_RSC : MonoBehaviour
{
    public bool earnWait;

    public int earning;

    public TowerInfo selfInfo;
    // Use this for initialization
    void Start()
    {
        selfInfo = GetComponent<TowerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!earnWait)
        {
            selfInfo.player.resource += earning;
            earnWait = true;
        }
    }
}
