using System;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private float scanInterval = 0.1f;
    [SerializeField] private float spherecastRadius = 0.1f;
    [SerializeField] private LayerMask selectableLayer;

    private float scanIntervalTimer = 0f;
    private RaycastHit hit;
    private ISelectable currentSelectable;

    public ISelectable CurrentSelectable
    {
        get => currentSelectable;
        private set
        {
            if(currentSelectable != value)
            {
                currentSelectable = value;
                OnSelectableChanged?.Invoke(currentSelectable);
            }
        }
    }
    public Transform CameraTransform => cameraTransform;
    public float InteractionRange => interactionRange;
    public LayerMask SelectableLayer => selectableLayer;


    public event Action<ISelectable> OnSelectableChanged;

    private void Update()
    {
        if(scanIntervalTimer <= 0f)
        {
            scanIntervalTimer = scanInterval;
            CheckRaycast();
        }

        scanIntervalTimer -= Time.deltaTime;
    }

    private void CheckRaycast()
    {
        if(Physics.SphereCast(cameraTransform.position, spherecastRadius, cameraTransform.forward, out hit, interactionRange, selectableLayer))
        {
            ISelectable selectable = hit.collider.GetComponent<ISelectable>();
            if(selectable != null)
            {
                if(CurrentSelectable != selectable)
                {
                    if(CurrentSelectable != null)
                    {
                        CurrentSelectable.OnDeselect();
                    }

                    CurrentSelectable = selectable;
                    CurrentSelectable.OnSelect();
                }
            }
        }
        else
        {
            if(CurrentSelectable != null)
            {
                CurrentSelectable.OnDeselect();
                CurrentSelectable = null;
            }
        }
    }
}
