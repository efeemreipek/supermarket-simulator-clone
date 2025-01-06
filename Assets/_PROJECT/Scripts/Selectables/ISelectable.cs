using UnityEngine;

public interface ISelectable
{
    public void OnSelect();
    public void OnDeselect();
    public GameObject GameObject { get; }
    public bool CanPick { get; }
}
