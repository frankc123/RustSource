using Facepunch.Progress;
using System;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public dfRichTextLabel infoText;
    private static string labelString;
    public static readonly ProgressBar Operations = new ProgressBar();
    public dfPanel progressBar;
    public dfSprite progressIndicator;
    private static bool showing;
    private static LoadingScreen singleton;

    private void Awake()
    {
        singleton = this;
        if (showing)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public static void Hide()
    {
        showing = false;
        if (singleton != null)
        {
            singleton.GetComponent<dfPanel>().Hide();
        }
    }

    public void LateUpdate()
    {
        float num;
        if (Operations.Update(out num))
        {
            if (!this.progressBar.IsVisible)
            {
                this.progressBar.Show();
            }
            this.progressIndicator.FillAmount = num;
        }
        else if (this.progressBar.IsVisible)
        {
            this.progressBar.Hide();
        }
    }

    public static void Show()
    {
        showing = true;
        if (singleton != null)
        {
            singleton.GetComponent<dfPanel>().Show();
        }
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(labelString) && (this.infoText != null))
        {
            this.infoText.Text = labelString;
        }
    }

    public static void Update(string strText)
    {
        Operations.Clean();
        Debug.Log("LoadingScreen: " + strText);
        labelString = strText;
        if (singleton != null)
        {
            singleton.infoText.Text = strText;
        }
    }
}

