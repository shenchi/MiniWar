using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{

    public int roundTime;

    public bool wait;

    public int order;
    public string[] turnOrder;

    public GameObject[] units;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(wait)
        {
            return;
        }

        foreach (string action in turnOrder)
        {
            if(action == "attack")
            {
                foreach (GameObject unit in units)
                {
                    if (unit.tag == "T_ATK")
                    {
                        unit.GetComponent<T_ATK>().attackWait = false;
                    }
                }
            }

            else if(action == "consume")
            {
                foreach (GameObject unit in units)
                {
                    unit.GetComponent<TowerInfo>().consumeWait = false;
                }
            }

            else if(action == "earn")
            {

                foreach (GameObject unit in units)
                {
                    if (unit.tag == "T_RSC")
                    {
                        unit.GetComponent<T_RSC>().earnWait = false;
                    }
                }
            }

            else if(action == "destroy")
            {
                foreach (GameObject unit in units)
                {
                    unit.GetComponent<TowerInfo>().destroyWait = false;
                }
            }
        }

        wait = true;
	}
}
