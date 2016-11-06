using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField]
    private List<Hexagon> startPoints;

    private Dictionary<HexCoord, Hexagon> hexagons;

    void OnEnable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif
            if (null == hexagons)
            {
                hexagons = new Dictionary<HexCoord, Hexagon>();
                var list = GetComponentsInChildren<Hexagon>();
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].UpdateCoord();
                    //print(list[i].gameObject.name + ": " + list[i].coord.x + ",  " + list[i].coord.y);
                    hexagons.Add(list[i].coord, list[i]);
                }
            }
            if (Instance != null)
                throw new System.Exception("MapManager already exists");
            Instance = this;

#if UNITY_EDITOR
        }
#endif
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
#endif

            if (Instance == this)
                Instance = null;

#if UNITY_EDITOR
        }
#endif
    }


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
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
#endif
    }
    
    public HexCoord GetStartPointCoord(int slot)
    {
        return startPoints[slot].coord;
    }

    public Vector3 GetMountPosition(Hexagon hexagon)
    {
        return hexagon.transform.position + Vector3.up * hexagon.transform.localScale.y * 0.1f;
    }

    public Vector3 GetMountPosition(HexCoord coord)
    {
        if (hexagons.ContainsKey(coord))
        {
            return GetMountPosition(hexagons[coord]);
        }
        return HexagonUtils.Coord2Pos(coord);
    }

    public bool Exists(HexCoord coord)
    {
        return hexagons.ContainsKey(coord);
    }
}
