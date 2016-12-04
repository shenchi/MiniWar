﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class CullByVision : MonoBehaviour
{
    public GameObject children = null;
    private HexCoord coord = new HexCoord();
    private Vector3 lastPosition = Vector3.zero;
    private Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        coord = HexagonUtils.Pos2Coord(transform.position);
        lastPosition = transform.position;
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
        {
            renderer.enabled = false;
            if (null != children)
                children.SetActive(false);
        }
        else
        {
            bool inVision = VisionController.Instance.InVision(coord);
            renderer.enabled = inVision;
            if (null != children)
                children.SetActive(inVision);
        }
    }
}
