using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{

    public GameObject target;

    public Vector3 axis;

    public float omega;

    // Use this for initialization
    void Start()
    {
        if (null == target)
            target = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        target.transform.localRotation *= Quaternion.AngleAxis(omega * Time.deltaTime, axis);
    }
}
