using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    public Text resText;

    private Canvas canvas;
    
    public bool EnableUI
    {
        get
        {
            return canvas.enabled;
        }
        set
        {
            canvas.enabled = value;
        }
    }
    
    void OnEnable()
    {
        canvas = GetComponent<Canvas>();
        if (Instance != null)
            throw new System.Exception("UIController already exists");
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetResource(int res)
    {
        resText.text = res.ToString();
    }
}
