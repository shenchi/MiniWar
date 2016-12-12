using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SnapToCenter : MonoBehaviour
{

    public Renderer target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (null != target)
            transform.position = target.bounds.center;
    }
}
