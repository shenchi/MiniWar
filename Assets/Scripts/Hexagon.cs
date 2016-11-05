using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Hexagon : MonoBehaviour
{
    public int height = 1;
    public HexCoord coord;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            coord = HexagonUtils.Pos2Coord(transform.position);
            transform.position = HexagonUtils.Coord2Pos(coord);

            height = Mathf.Clamp(height, 1, HexagonUtils.MaxHeight);
            transform.localScale = new Vector3(1, height, 1);
        }
#endif
    }
}
