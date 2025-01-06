using UnityEngine;

[CreateAssetMenu(fileName = "InputValues", menuName = "Scriptable Objects/InputValues")]
public class InputValues : ScriptableObject
{
    public Vector2 Move;
    public Vector2 Look;
    public bool Run;
    public bool Jump;
    public bool PrimaryAction;
    public bool SecondaryAction;
    public bool InteractAction;
    public bool AlternateInteractAction;
}
