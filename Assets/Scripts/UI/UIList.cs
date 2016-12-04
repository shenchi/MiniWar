using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIList : MonoBehaviour
{
    public enum Orientation
    {
        Vertical,
        Horizontal
    }

    public Orientation orientation = Orientation.Vertical;

    public GameObject prefab;
    public RectTransform content;

    private List<GameObject> list = new List<GameObject>();

    public GameObject Add(object data)
    {
        GameObject go = Instantiate(prefab);
        RectTransform t = go.transform as RectTransform;

        if (!t)
            throw new System.Exception("Cannot Find RectTransform");

        t.SetParent(content, false);

        var pos = t.localPosition;
        if (orientation == Orientation.Vertical)
        {
            pos.y = -content.rect.height;
        }
        else
        {
            pos.x = content.rect.width;
        }
        t.localPosition = pos;

        var r = content.sizeDelta;

        if (orientation == Orientation.Vertical)
        {
            float height = t.rect.height;
            r.y += height;
        }
        else
        {
            float width = t.rect.width;
            r.x += width;
        }
        content.sizeDelta = r;

        list.Add(go);

        go.SendMessage("SetData", data, SendMessageOptions.DontRequireReceiver);

        return go;
    }

    public void Clear()
    {
        for (int i = 0; i < list.Count; ++i)
        {
            Destroy(list[i]);
        }
        list.Clear();

        var r = content.sizeDelta;
        if (orientation == Orientation.Vertical)
        {
            r.y = 0;
        }
        else
        {
            r.x = 0;
        }
        content.sizeDelta = r;
    }
}
