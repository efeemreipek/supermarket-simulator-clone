using UnityEngine;

public class ReputationManager : Singleton<ReputationManager>
{
    [SerializeField] private int maxReputation = 100;
    [SerializeField] private int startReputation = 20;

    private int currentReputation;

    protected override void Awake()
    {
        base.Awake();

        currentReputation = startReputation;
        UIManager.Instance.UpdateReputation((float)currentReputation / maxReputation);
    }

    public void ChangeReputation(int amount)
    {
        currentReputation = Mathf.Clamp(currentReputation + amount, 0, maxReputation);
        UIManager.Instance.UpdateReputation((float)currentReputation / maxReputation);
    }
}
