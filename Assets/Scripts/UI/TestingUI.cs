using TMPro;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
/// 
/// Provides a basic UI that updates a text label with the current exit count during training. 
/// </summary>
public class TestingUI : MonoBehaviour
{
    public TextMeshProUGUI exitCounterText;
    private void Start()
    {
        InvokeRepeating(nameof(UpdateText), 1, 1.0f * Time.timeScale);
    }

    void UpdateText()
    {
        exitCounterText.SetText("Exit Counter:  " + LevelParameters.Instance.GetExitCounterString());
    }
}