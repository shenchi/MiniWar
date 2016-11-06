using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class CullByVision : MonoBehaviour
{
    private HexCoord coord = new HexCoord();
    private Vector3 lastPosition = Vector3.zero;
    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (lastPosition != transform.position)
        {
            lastPosition = transform.position;
            coord = HexagonUtils.Pos2Coord(transform.position);
        }
    }

    void LateUpdate()
    {
        if (!VisionController.Instance)
            renderer.enabled = false;
        else
            renderer.enabled = VisionController.Instance.InVision(coord);
    }
}
