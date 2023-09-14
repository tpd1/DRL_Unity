using TMPro;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
/// 
/// Provides a basic UI that updates a text label with the current exit count during training. 
/// </summary>
public class TrainingUI : MonoBehaviour
{
    public TextMeshProUGUI exitCounterText;
   
    /// <summary>
    /// Called once at the start. Calls the UpdateText function periodically.
    /// </summary>
    private void Start()
    {
        InvokeRepeating(nameof(UpdateText), 1, 1.0f * Time.timeScale);
    }

    /// <summary>
    /// Updates the UI text label with the current exit count.
    /// </summary>
    void UpdateText()
    {
        exitCounterText.SetText("Exit Counter:  " + LevelParameters.Instance.GetExitCounterString());
    }
}