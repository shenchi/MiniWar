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
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && GetComponent<Camera>().fieldOfView > 25)
        {
            GetComponent<Camera>().fieldOfView--;
            if(GetComponent<Camera>().fieldOfView >= 60)
            {
                Quaternion newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 0.33334f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                transform.rotation = newRotation;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && GetComponent<Camera>().fieldOfView < 120)
        {
            GetComponent<Camera>().fieldOfView++;
            if (GetComponent<Camera>().fieldOfView > 60)
            {
                Quaternion newRotation = Quaternion.Euler(transform.rotation.eulerAngles.x + 0.33334f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
                transform.rotation = newRotation;
            }
        }
    }

    public void setCameraFocus(GameObject hex)
    {
        transform.position = new Vector3 (hex.transform.position.x, transform.position.y + 5, hex.transform.position.z - 5);
    }

    public void setCameraFocus(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y + 5, pos.z - 5);
    }
}
