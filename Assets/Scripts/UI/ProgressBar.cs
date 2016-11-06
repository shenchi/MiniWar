using UnityEngine;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform fill;

    [SerializeField]
    private float value;

    public float Value
    {
        set
        {
            this.value = Mathf.Clamp01(value);
        }
        get
        {
            return this.value;
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (null == fill)
            return;

        value = Mathf.Clamp01(value);
        var anc = fill.anchorMax;
        anc.x = value;
        fill.anchorMax = anc;
    }
}
