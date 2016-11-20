using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    public GameObject inGamePanel;

    public Text resText;
    public Text prodText;
    public Text costText;
    public GameObject buildPanel;
    public ProgressBar progressBar;

    public Text logText;

    public Text winText;
    public Text loseText;
    public Text noticeText;

    public Button[] buildButtonList;
    private Dictionary<string, Action<string>> buttonActions = new Dictionary<string, Action<string>>();

    public Button doneButton;

    public bool EnableInGameUI
    {
        get
        {
            return inGamePanel.activeSelf;
        }
        set
        {
            inGamePanel.SetActive(value);
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

    public bool EnableDoneButton
    {
        get
        {
            return doneButton.gameObject.activeSelf;
        }
        set
        {
            doneButton.gameObject.SetActive(value);
        }
    }

    public void ShowWin(bool visible)
    {
        winText.gameObject.SetActive(visible);
    }

    public void ShowLose(bool visible)
    {
        loseText.gameObject.SetActive(visible);
    }

    void Awake()
    {
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

    public void SetProduction(int prod)
    {
        prodText.text = prod.ToString();
    }

    public void SetCost(int cost)
    {
        costText.text = cost.ToString();
    }

    public void SetBuildButtonText(int index, string text)
    {
        buildButtonList[index].GetComponentInChildren<Text>().text = text;
    }

    public void SetBuildButtonTexts(string[] text)
    {
        int count = Mathf.Min(text.Length, buildButtonList.Length);
        for (int i = 0; i < count; i++)
        {
            SetBuildButtonText(i, text[i]);
        }
    }

    public void RegisterButtonAction(string type, Action<string> action)
    {
        if (!buttonActions.ContainsKey(type))
            buttonActions.Add(type, action);
        else
            buttonActions[type] += action;
    }

    public void ClearButtonActions()
    {
        buttonActions = new Dictionary<string, Action<string>>();
    }

    public void OnButtonClicked(string type)
    {
        if (buttonActions.ContainsKey(type))
            buttonActions[type](type);
    }

    public void AddLog(string txt)
    {
        logText.text += "\n" + txt;

        float height = LayoutUtility.GetPreferredHeight(logText.rectTransform);
        float areaHeight = logText.rectTransform.rect.height;

        while (height > areaHeight)
        {
            logText.text = logText.text.Remove(0, logText.text.IndexOf('\n') + 1);

            height = LayoutUtility.GetPreferredHeight(logText.rectTransform);
        }
    }
}
