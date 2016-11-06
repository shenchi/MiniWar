using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeIndicator : MonoBehaviour
{
    public Mesh mesh;
    public Material material;

    private Material mat;
    private List<Vector3> positions = null;

    public Color TintColor
    {
        get
        {
            return mat.color;
        }
        set
        {
            mat.color = value;
        }
    }
    
    public void RestoreMaterial()
    {
        Destroy(mat);
        mat = new Material(material);
    }

    public void SetRange(HashSet<HexCoord> range)
    {
        positions = new List<Vector3>();
        foreach (var coord in range)
        {
            Vector3 pos = MapManager.Instance.GetMountPosition(coord) + Vector3.up * 0.02f;
            positions.Add(pos);
        }
    }

    void Awake()
    {
        mat = new Material(material);
    }

    void OnDestroy()
    {
        Destroy(mat);
    }

    void OnRenderObject()
    {
        if (null == positions || null == mat || null == mesh)
            return;

        mat.SetPass(0);

        for (int i = 0; i < positions.Count; i++)
        {
            Graphics.DrawMeshNow(mesh, positions[i], Quaternion.identity);
        }
    }
}
