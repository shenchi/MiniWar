using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowerHealth : MonoBehaviour {

    public TowerInfo thisTower;
    public Image healthFill;

	// Use this for initialization
	void Start () {
        thisTower = GetComponentInParent<TowerInfo>();
        healthFill = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        healthFill.fillAmount = thisTower.healthGP;
	}
}
