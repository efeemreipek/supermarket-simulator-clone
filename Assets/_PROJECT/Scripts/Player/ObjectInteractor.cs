using System;
using UnityEngine;

public class ObjectInteractor : MonoBehaviour
{
    [SerializeField] private InputValues inputValues;
    [SerializeField] private Transform itemHolderTransform;
    [SerializeField] private GameObject playerFollowCameraGO;
    [SerializeField] private LayerMask objectPutLayer;

    private ObjectSelector selector;
    private bool isHoldingBox = false;
    private Box currentHoldingBox;
    private bool canInteract = true;


    public bool IsHoldingBox => isHoldingBox;


    public static event Action OnCreditCardTaken;

    private void Awake()
    {
        selector = GetComponent<ObjectSelector>();
    }
    private void OnEnable()
    {
        UIManager.OnUIPanelOpened += ChangeCanInteractFalse;
        UIManager.OnUIPanelClosed += ChangeCanInteractTrue;
    }
    private void OnDisable()
    {
        UIManager.OnUIPanelOpened -= ChangeCanInteractFalse;
        UIManager.OnUIPanelClosed -= ChangeCanInteractTrue;
    }
    private void Update()
    {
        if(!canInteract) return;

        // when left mouse button is clicked
        if(inputValues.PrimaryAction)
        {
            inputValues.PrimaryAction = false;

            // pick box from ground
            if(selector.CurrentSelectable != null && !isHoldingBox && selector.CurrentSelectable.CanPick)
            {
                currentHoldingBox = selector.CurrentSelectable.GameObject.GetComponent<Box>();
                Transform selectableTransform = currentHoldingBox.transform;
                selectableTransform.SetParent(itemHolderTransform);
                selectableTransform.localPosition = Vector3.zero;
                Quaternion rotation = Quaternion.Euler(-playerFollowCameraGO.transform.eulerAngles.x, 0f, 0f);
                selectableTransform.localRotation = rotation * Quaternion.identity;

                isHoldingBox = true;
            }
            // place product on shelf
            if(currentHoldingBox != null && isHoldingBox)
            {
                if(Physics.Raycast(selector.CameraTransform.position, selector.CameraTransform.forward, out RaycastHit hitInfo, selector.InteractionRange, selector.SelectableLayer))
                {
                    if(hitInfo.collider.TryGetComponent(out Shelf shelf))
                    {
                        shelf.PlaceProduct(currentHoldingBox);
                    }
                }
            }
            // check products at register
            if(selector.CurrentSelectable != null && !isHoldingBox && selector.CurrentSelectable.GameObject.TryGetComponent(out BaseProduct product))
            {
                if(!product.IsAtRegister) return;

                Transform productParent = product.transform.parent;
                CashRegister cashRegister = productParent.GetComponentInParent<CashRegister>();

                cashRegister.RemoveProductFromPoint(productParent);
            }
            // take credit card
            if(selector.CurrentSelectable != null && !isHoldingBox && selector.CurrentSelectable.GameObject.TryGetComponent(out CreditCard card))
            {
                CameraManager.Instance.PrioritizeCamera(ECameraType.POSMachine);
                OnCreditCardTaken?.Invoke();
                Destroy(card);
            }
            // open computer
            if(selector.CurrentSelectable != null && !isHoldingBox && selector.CurrentSelectable.GameObject.TryGetComponent(out Computer computer))
            {
                CameraManager.Instance.PrioritizeCamera(ECameraType.Computer, computer.CameraGO);
            }
            // open-close sign
            if(selector.CurrentSelectable != null && !isHoldingBox && selector.CurrentSelectable.GameObject.TryGetComponent(out OpenCloseSign openCloseSign))
            {
                openCloseSign.ChangeState();
            }
        }

        // when alternate interact button is pressed
        if(inputValues.AlternateInteractAction)
        {
            inputValues.AlternateInteractAction = false;

            UIManager.Instance.OpenCloseInstructions();
        }


        // when right mouse button is clicked
        if(inputValues.SecondaryAction)
        {
            inputValues.SecondaryAction = false;

            // put product back on the box
            if(selector.CurrentSelectable != null && isHoldingBox && selector.CurrentSelectable.GameObject.TryGetComponent(out BaseProduct product))
            {
                if(currentHoldingBox.Product != product.Product) return;

                product.AccordingShelf.RemoveProduct(currentHoldingBox, product);
            }

            if(currentHoldingBox != null && isHoldingBox)
            {
                // Check for dumpster first
                if(Physics.Raycast(selector.CameraTransform.position, selector.CameraTransform.forward, out RaycastHit dumpsterHit, selector.InteractionRange, selector.SelectableLayer))
                {
                    if(dumpsterHit.collider.TryGetComponent(out Dumpster dumpster))
                    {
                        foreach(Transform t in itemHolderTransform)
                        {
                            Destroy(t.gameObject);
                        }
                        currentHoldingBox = null;
                        isHoldingBox = false;
                        return;
                    }
                }

                // If no dumpster, try placing on ground
                if(Physics.Raycast(selector.CameraTransform.position, selector.CameraTransform.forward, out RaycastHit groundHit, selector.InteractionRange))
                {
                    if(((1 << groundHit.collider.gameObject.layer) & objectPutLayer) == 0) return;

                    currentHoldingBox.transform.position = groundHit.point;
                    currentHoldingBox.transform.rotation = Quaternion.identity;
                    itemHolderTransform.DetachChildren();

                    currentHoldingBox = null;
                    isHoldingBox = false;
                }
            }
        }


        // when interact button is pressed
        if(inputValues.InteractAction)
        {
            inputValues.InteractAction = false;

            if(selector.CurrentSelectable == null) return;

            // open UI for shelf price 
            if(selector.CurrentSelectable.GameObject.TryGetComponent(out Shelf shelf))
            {
                UIManager.Instance.OpenShelfPricePanel(shelf);
            }
        }
    }

    private void ChangeCanInteractTrue() => canInteract = true;
    private void ChangeCanInteractFalse() => canInteract = false;
}
