using Unity.MLAgents;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
/// 
/// When attached to a game object, provides a single location where all aspects of the environment
/// are controlled.
/// </summary>
public class LevelParameters : MonoBehaviour
{
    public static LevelParameters Instance { get; set; }

    [Header("Level Generation")] [SerializeField]
    private int mainPathLength = 2;

    [SerializeField] private int maxBranches;
    [SerializeField] private int branchLength;
    [SerializeField] private float smokeProbability = 0.5f;

    [Header("Agent Reward")] [SerializeField]
    private float exitReward = 1.0f;

    [SerializeField] private float unvisitedDoorReward = 0.5f;
    [SerializeField] private float wallPenalty = -1.0f;
    [SerializeField] private float obstaclePenalty = -0.5f;
    [SerializeField] private float smokePenalty = -0.1f;

    [Header("Debug / Mode Select")] [SerializeField]
    private bool generateProps;
    
    [SerializeField] private bool trainingMode;
    [SerializeField] private bool initaliseAgent;

    // Backing fields for public properties (Unity editor does allow for C# properties to be listed as
    // SerialisedFields.
    public int MainPathLength
    {
        get => mainPathLength;
        set => mainPathLength = value;
    }

    public int MaxBranches
    {
        get => maxBranches;
        set => maxBranches = value;
    }

    public int BranchLength
    {
        get => branchLength;
        set => branchLength = value;
    }

    public bool GenerateProps
    {
        get => generateProps;
        set => generateProps = value;
    }

    public bool InitialiseAgent
    {
        get => initaliseAgent;
        set => initaliseAgent = value;
    }

    private int LevelComplexity { get; set; }
    private const float smokePenaltyDelay = 0.5f;
    private const int resetAfterFailed = 20;

    // Counters.
    private int exitCounter;
    private int obstacleCollisions;
    private int wallCollisions;

    /// <summary>
    /// Called once, sets up the singleton instance.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (trainingMode)
        {
            // fetch level complexity from curriculum learning
            LevelComplexity =
                (int)Academy.Instance.EnvironmentParameters.GetWithDefault("level_complexity", LevelComplexity);
        }
    }

    /// <summary>
    /// Sets the level complexity during training using curriculum learning with ML-agents.
    /// </summary>
    public void SetComplexity()
    {
        LevelComplexity =
            (int)Academy.Instance.EnvironmentParameters.GetWithDefault("level_complexity", LevelComplexity);

        switch (LevelComplexity)
        {
            case 0:
                mainPathLength = 2;
                maxBranches = 0;
                branchLength = 0;
                break;
            case 1:
                mainPathLength = 3;
                maxBranches = 0;
                branchLength = 0;
                break;
            case 2:
                mainPathLength = 3;
                maxBranches = 1;
                branchLength = 1;
                break;
            case 3:
                mainPathLength = 4;
                maxBranches = 2;
                branchLength = 1;
                break;
            case 4:
                mainPathLength = 4;
                maxBranches = 2;
                branchLength = 2;
                break;
            case 5:
                mainPathLength = 4;
                maxBranches = 2;
                branchLength = 3;
                break;
        }
    }
    
    // Getters and setters.
    public bool IsTrainingMode() => trainingMode;
    public float GetSmokeProbability() => smokeProbability;
    public int GetExitCounter() => exitCounter;
    public string GetExitCounterString() => exitCounter.ToString();
    public void IncrementExitCounter() => exitCounter++;
    public float GetExitReward() => exitReward;
    public float GetUnvisitedDoorReward() => unvisitedDoorReward;
    public float GetWallPenalty() => wallPenalty;
    public float GetObstaclePenalty() => obstaclePenalty;
    public float GetSmokePenalty() => smokePenalty;
    public float GetSmokePenaltyDelay() => smokePenaltyDelay;
    public int GetResetAfterFailed() => resetAfterFailed;
}