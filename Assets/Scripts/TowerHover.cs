using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(TowerInfo), typeof(RangeIndicator))]
public class TowerHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RangeIndicator indicator;

    private int range = 0;
    private Vector3 curPosition;

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

        indicator = GetComponent<RangeIndicator>();
        curPosition = transform.position;
        UpdateRange();

        indicator.enabled = false;
    }

    void OnDisable()
    {
        indicator.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        indicator.enabled = true;

        if (curPosition != transform.position)
        {
            curPosition = transform.position;
            UpdateRange();
        }
    }

    public void UpdateRange()
    {
        HexCoord c = HexagonUtils.Pos2Coord(curPosition);
        var r = RangeUtils.GetRangeClient(c, range);
        r.Add(c);
        indicator.SetRange(r);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        indicator.enabled = false;
    }

}
