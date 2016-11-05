using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{

    public int resource;
    public int team;

    public GameManager manager;
	// Use this for initialization
	void Start ()
    {
        manager = FindObjectOfType<GameManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(!manager.wait)
        {
            return;
        }
	}
}
