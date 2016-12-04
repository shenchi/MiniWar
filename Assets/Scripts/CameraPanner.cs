using UnityEngine;
using System.Collections;

public class CameraPanner : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    private Vector3 lastMousePos;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            Vector3 pos = transform.localPosition;
            pos.x -= delta.x * 0.01f;
            pos.z -= delta.y * 0.01f;
            transform.localPosition = pos;
        }
    }
}
