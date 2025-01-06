using UnityEngine;
using TMPro;

public class OpenCloseSign : Selectable
{
    [SerializeField] private TextMeshPro signText;

    private bool isOpen = false;

    private const string OPEN_STRING = "OPEN";
    private const string CLOSED_STRING = "CLOSED";

    private void Start()
    {
        signText.SetText(isOpen ? OPEN_STRING : CLOSED_STRING);
    }
    public void ChangeState()
    {
        isOpen = !isOpen;
        signText.SetText(isOpen ?  OPEN_STRING : CLOSED_STRING);
        CustomerSpawner.Instance.ChangeCanSpawn(isOpen);
    }
}
