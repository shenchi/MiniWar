using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIList : MonoBehaviour
{
    public GameObject prefab;
    public RectTransform content;
    
    public GameObject Add(object data)
    {
        GameObject go = Instantiate(prefab);
        RectTransform t = go.transform as RectTransform;

        if (!t)
            throw new System.Exception("Cannot Find RectTransform");

        t.SetParent(content, false);

        var pos = t.localPosition;
        pos.y = -content.rect.height;
        t.localPosition = pos;

        float height = t.rect.height;
        var r = content.sizeDelta;
        r.y += height;
        content.sizeDelta = r;

        go.SendMessage("SetData", data, SendMessageOptions.DontRequireReceiver);

        return go;
    }
    
}
