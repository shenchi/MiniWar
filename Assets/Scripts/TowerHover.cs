using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(TowerInfo))]
public class TowerHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Mesh mesh;
    public Material material;

    public bool showRange = false;

    private int range = 0;
    private List<Vector3> positions = null;
    private Vector3 lastPosition;
    private int layer = 0;

    void Awake()
    {
        layer = LayerMask.NameToLayer("TransparentFX");
    }

    void OnEnable()
    {
        var t = GetComponent<TowerInfo>();
        if (t.type == TowerType.VisionTower)
        {
            range = t.vision;
        }
        else
        {
            range = t.range;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showRange = true;

        if (positions == null || lastPosition != transform.position)
        {
            lastPosition = transform.position;
            HexCoord c = HexagonUtils.Pos2Coord(lastPosition);
            var r = HexagonUtils.NeighborHexagons(c, range);
            r.Add(c);
            r.IntersectWith(VisionController.Instance.Vision);
            r.RemoveWhere(x => { return !MapManager.Instance.Exists(x); });

            positions = new List<Vector3>();
            foreach (var coord in r)
            {
                Vector3 pos = MapManager.Instance.GetMountPosition(coord) + Vector3.up * 0.02f;
                positions.Add(pos);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        showRange = false;
    }

    void OnRenderObject()
    {
        if (!showRange)
            return;

        material.SetPass(0);

        for (int i = 0; i < positions.Count; i++)
        {
            Graphics.DrawMeshNow(mesh, positions[i], Quaternion.identity);
        }
    }
}
