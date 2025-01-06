using UnityEngine;
using TMPro;

public class InstructionTextUI : MonoBehaviour
{
    private TextMeshProUGUI instructionText;

    private void Awake()
    {
        instructionText = GetComponent<TextMeshProUGUI>();
    }

    public void InitializeInstructionText(string text)
    {
        instructionText.SetText(text);
    }

}
