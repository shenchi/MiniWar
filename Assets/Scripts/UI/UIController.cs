using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public Text resText;
    public GameObject buildPanel;
    public ProgressBar progressBar;

    private Canvas canvas;
    private Dictionary<string, Action> buttonActions = new Dictionary<string, Action>();

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

    public bool EnableBuildingPanel
    {
        get
        {
            return buildPanel.activeSelf;
        }
        set
        {
            buildPanel.SetActive(value);
        }
    }
    
    public float RemainingTime
    {
        get
        {
            return progressBar.Value;
        }
        set
        {
            progressBar.Value = value;
        }
    }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (Instance != null)
            throw new System.Exception("UIController already exists");
        Instance = this;
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    public void SetResource(int res)
    {
        resText.text = res.ToString();
    }

    public void RegisterButtonAction(string type, Action action)
    {
        if (!buttonActions.ContainsKey(type))
            buttonActions.Add(type, action);
        else
            buttonActions[type] += action;
    }

    public void ClearButtonActions()
    {
        buttonActions = new Dictionary<string, Action>();
    }

    public void OnButtonClicked(string type)
    {
        if (buttonActions.ContainsKey(type))
            buttonActions[type]();
    }
}
