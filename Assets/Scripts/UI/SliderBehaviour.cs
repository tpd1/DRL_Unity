using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Controls the functionality of the UI within the demonstration environment.
/// </summary>
public class SliderBehaviour : MonoBehaviour
{
    [SerializeField] private DemoManager demoManager;

    [SerializeField] private Slider mainPathSlider;
    [SerializeField] private TextMeshProUGUI mainPathText;

    [SerializeField] private Slider numBranchesSlider;
    [SerializeField] private TextMeshProUGUI numBranchesText;

    [SerializeField] private Slider branchLengthSlider;
    [SerializeField] private TextMeshProUGUI branchLengthText;

    [SerializeField] private Toggle genPropsToggle;
    [SerializeField] private Button regenButton;


    /// <summary>
    /// Called Once. Sets up click listeners for the UI components.
    /// </summary>
    private void Start()
    {
        mainPathSlider.onValueChanged.AddListener(val =>
        {
            mainPathText.text = val.ToString(CultureInfo.InvariantCulture);
            LevelParameters.Instance.MainPathLength = (int)val;
        });

        numBranchesSlider.onValueChanged.AddListener(val =>
        {
            numBranchesText.text = val.ToString(CultureInfo.InvariantCulture);
            LevelParameters.Instance.MaxBranches = (int)val;
        });

        branchLengthSlider.onValueChanged.AddListener(val =>
        {
            branchLengthText.text = val.ToString(CultureInfo.InvariantCulture);
            LevelParameters.Instance.BranchLength = (int)val;
        });

        genPropsToggle.onValueChanged.AddListener(val => { LevelParameters.Instance.GenerateProps = val; });
        regenButton.onClick.AddListener(() => { demoManager.RegenTrainingLevel(); });
    }
}