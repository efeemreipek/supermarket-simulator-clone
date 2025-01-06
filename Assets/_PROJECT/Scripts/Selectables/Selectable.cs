using UnityEngine;

public abstract class Selectable : MonoBehaviour, ISelectable
{
    [SerializeField] private bool canPick;

    protected Outline outline;

    public GameObject GameObject => gameObject;
    public bool CanPick => canPick;

    protected virtual void Awake()
    {
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
    }

    public void OnSelect()
    {
        outline.enabled = true;
    }
    public void OnDeselect()
    {
        if(outline != null)
        {
            outline.enabled = false;
        }
    }
}
