using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputHandler : MonoBehaviour
{
    [SerializeField] private InputValues inputValues;

    private InputAction actionMove;
    private InputAction actionLook;
    private InputAction actionRun;
    private InputAction actionJump;
    private InputAction actionPrimary;
    private InputAction actionSecondary;
    private InputAction actionInteract;
    private InputAction actionAlternateInteract;
    private InputAction actionEscape;

    private void OnEnable()
    {
        actionMove = InputSystem.actions.FindAction("Move");
        actionLook = InputSystem.actions.FindAction("Look");
        actionRun = InputSystem.actions.FindAction("Run");
        actionJump = InputSystem.actions.FindAction("Jump");
        actionPrimary = InputSystem.actions.FindAction("Primary Action");
        actionSecondary = InputSystem.actions.FindAction("Secondary Action");
        actionInteract = InputSystem.actions.FindAction("Interact Action");
        actionAlternateInteract = InputSystem.actions.FindAction("Alternate Interact Action");
        actionEscape = InputSystem.actions.FindAction("Escape");

        actionMove.performed += Move_Performed;
        actionMove.canceled += Move_Canceled;

        actionLook.performed += Look_Performed;
        actionLook.canceled += Look_Canceled;

        actionRun.performed += Run_Performed;
        actionRun.canceled += Run_Canceled;

        actionJump.performed += Jump_Performed;
        actionJump.canceled += Jump_Canceled;

        actionPrimary.performed += Primary_Performed;
        actionPrimary.canceled += Primary_Canceled;

        actionSecondary.performed += Secondary_Performed;
        actionSecondary.canceled += Secondary_Canceled;

        actionInteract.performed += Interact_Performed;
        actionInteract.canceled += Interact_Canceled;

        actionAlternateInteract.performed += AlternateInteract_Performed;
        actionAlternateInteract.canceled += AlternateInteract_Canceled;

        actionEscape.performed += Escape_Performed;
        actionEscape.canceled += Escape_Canceled;

        InputSystem.actions.Enable();
    }
    private void OnDisable()
    {
        actionMove.performed -= Move_Performed;
        actionMove.canceled -= Move_Canceled;

        actionLook.performed -= Look_Performed;
        actionLook.canceled -= Look_Canceled;

        actionRun.performed -= Run_Performed;
        actionRun.canceled -= Run_Canceled;

        actionJump.performed -= Jump_Performed;
        actionJump.canceled -= Jump_Canceled;

        actionPrimary.performed -= Primary_Performed;
        actionPrimary.canceled -= Primary_Canceled;

        actionSecondary.performed -= Secondary_Performed;
        actionSecondary.canceled -= Secondary_Canceled;

        actionInteract.performed -= Interact_Performed;
        actionInteract.canceled -= Interact_Canceled;

        actionAlternateInteract.performed -= AlternateInteract_Performed;
        actionAlternateInteract.canceled -= AlternateInteract_Canceled;

        actionEscape.performed -= Escape_Performed;
        actionEscape.canceled -= Escape_Canceled;

        InputSystem.actions.Disable();
    }

    private void Move_Performed(InputAction.CallbackContext obj) => inputValues.Move = obj.ReadValue<Vector2>();
    private void Move_Canceled(InputAction.CallbackContext obj) => inputValues.Move = Vector2.zero;
    private void Look_Performed(InputAction.CallbackContext obj) => inputValues.Look = obj.ReadValue<Vector2>();
    private void Look_Canceled(InputAction.CallbackContext obj) => inputValues.Look = Vector2.zero;
    private void Run_Performed(InputAction.CallbackContext obj) => inputValues.Run = obj.ReadValueAsButton();
    private void Run_Canceled(InputAction.CallbackContext obj) => inputValues.Run = false;
    private void Jump_Performed(InputAction.CallbackContext obj) => inputValues.Jump = obj.ReadValueAsButton();
    private void Jump_Canceled(InputAction.CallbackContext obj) => inputValues.Jump = false;
    private void Primary_Performed(InputAction.CallbackContext obj) => inputValues.PrimaryAction = obj.ReadValueAsButton();
    private void Primary_Canceled(InputAction.CallbackContext obj) => inputValues.PrimaryAction = false;
    private void Secondary_Performed(InputAction.CallbackContext obj) => inputValues.SecondaryAction = obj.ReadValueAsButton();
    private void Secondary_Canceled(InputAction.CallbackContext obj) => inputValues.SecondaryAction = false;
    private void Interact_Performed(InputAction.CallbackContext obj) => inputValues.InteractAction = obj.ReadValueAsButton();
    private void Interact_Canceled(InputAction.CallbackContext obj) => inputValues.InteractAction = false;
    private void AlternateInteract_Performed(InputAction.CallbackContext obj) => inputValues.AlternateInteractAction = obj.ReadValueAsButton();
    private void AlternateInteract_Canceled(InputAction.CallbackContext obj) => inputValues.AlternateInteractAction = false;
    private void Escape_Performed(InputAction.CallbackContext obj) => inputValues.Escape = obj.ReadValueAsButton();
    private void Escape_Canceled(InputAction.CallbackContext obj) => inputValues.Escape = false;
}
