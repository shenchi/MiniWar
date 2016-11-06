using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class FogByVision : MonoBehaviour
{
    private HexCoord coord = new HexCoord();
    private Vector3 lastPosition = Vector3.zero;
    private Renderer renderer;

    private Material originalMaterial;
    private Material foggedMaterial;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        originalMaterial = renderer.sharedMaterial;
        foggedMaterial = new Material(originalMaterial);
        foggedMaterial.color = foggedMaterial.color * 0.5f;
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
            renderer.sharedMaterial = foggedMaterial;
        else
            renderer.sharedMaterial = VisionController.Instance.InVision(coord) ? originalMaterial : foggedMaterial;
    }
}
