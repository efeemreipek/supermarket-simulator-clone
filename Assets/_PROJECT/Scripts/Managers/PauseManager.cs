using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private InputValues inputValues;

    private bool isPaused = true;
    private bool wasInstructionsPanelActiveBeforePause = false;

    public bool IsPaused
    {
        get => isPaused;
        set
        {
            isPaused = value;            
            SetCursorLock(!value);
        }
    }
    public bool IsGameStarted { get; set; } = false;

    private void Update()
    {
        if(inputValues.Escape && !IsPaused)
        {
            inputValues.Escape = false;

            UnpauseGameMenu();
        }
    }
    public void UnpauseGameMenu()
    {
        UnpauseGame();

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
    public void UnpauseGame()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
    }

    private void SetCursorLock(bool locked)
    {
        Cursor.visible = !locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
