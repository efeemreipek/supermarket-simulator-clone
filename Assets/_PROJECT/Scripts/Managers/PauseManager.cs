using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private InputValues inputValues;

    private bool wasInstructionsPanelActiveBeforePause = false;

    public static bool IsPaused = false;

    private void Update()
    {
        if(inputValues.Escape)
        {
            inputValues.Escape = false;

            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0f : 1f;

            Debug.Log("IsPaused: " + IsPaused);

            if(IsPaused)
            {
                wasInstructionsPanelActiveBeforePause = UIManager.Instance.IsInstructionsPanelActive;
                if(wasInstructionsPanelActiveBeforePause)
                {
                    UIManager.Instance.OpenCloseInstructions();
                }
            }
            else
            {
                if(wasInstructionsPanelActiveBeforePause)
                {
                    UIManager.Instance.OpenCloseInstructions();
                }
            }

            UIManager.Instance.OpenClosePause();
        }
    }
}
