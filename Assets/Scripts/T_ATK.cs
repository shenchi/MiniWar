using UnityEngine;
using System.Collections;

public class T_ATK : MonoBehaviour
{
    public bool attackWait;

    public int damage;
    public GameObject target;

    public TowerInfo selfInfo;
    // Use this for initialization
    void Start ()
    {
        selfInfo = GetComponent<TowerInfo>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(target != null && !attackWait)
        {
            attack(target);
            attackWait = true;
        }
	}

    public void attack(GameObject target)
    {
        target.GetComponent<TowerInfo>().health -= damage;
    }
}
