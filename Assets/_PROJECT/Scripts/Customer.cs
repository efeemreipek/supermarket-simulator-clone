using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;

public enum ECustomerState
{
    None,
    Spawn,
    MovingToShelf,
    GatheringProduct,
    MovingToCashRegister,
    WaitingQueue,
    PlacingProductsOnRegister,
    WaitingCheck,
    WaitingTransaction,
    MovingToExit
}

public class Customer : MonoBehaviour
{
    [SerializeField] private List<ProductListSO> possibleProductListsList;
    [SerializeField] private float productGatheringTime = 2f;
    [SerializeField] private GameObject handGO;

    private NavMeshAgent navMeshAgent;
    private ECustomerState state;
    private ShoppingList shoppingList;
    private ShoppingCart shoppingCart;
    private bool isTargetReached = false;
    private float targetReachCheckInterval = 0.05f;
    private float targetReachCheckTimer = 0f;
    private int currentShoppingListIndex = 0;
    private float priceOfProducts = 0f;
    private bool isTransactionCompleted = false;
    private Vector3 currentQueuePosition;
    private bool isMovingInQueue = false;
    private bool hasStartedPlacingProducts = false;
    private int satisfactionLevel = 0;

    private Shelf[] allShelves;
    private CashRegister[] allCashRegisters;
    private Shelf selectedShelf;
    private CashRegister selectedCashRegister;
    private CreditCard creditCard;
    private Shelf nextShelf;
    private ProductListSO productList;

    private ProductSO lastProductOfShoppingList => shoppingList.Elements[shoppingList.Elements.Count - 1].Product;

    public static event Action<CashRegister, float> OnWaitingTransaction;

    private void Awake()
    {
        handGO.SetActive(false);

        navMeshAgent = GetComponent<NavMeshAgent>();
        creditCard = handGO.GetComponentInChildren<CreditCard>();

        allShelves = FindObjectsByType<Shelf>(FindObjectsSortMode.None);
        allCashRegisters = FindObjectsByType<CashRegister>(FindObjectsSortMode.None);
    }
    private void OnEnable()
    {
        creditCard.OnCreditCardTaken += CreditCard_OnCreditCardTaken;
        CashRegister.OnTransactionCompleted += CashRegister_OnTransactionCompleted;
    }
    private void OnDisable()
    {
        creditCard.OnCreditCardTaken -= CreditCard_OnCreditCardTaken;
        CashRegister.OnTransactionCompleted -= CashRegister_OnTransactionCompleted;
    }
    private void Start()
    {
        SetState(ECustomerState.Spawn);
    }
    private void Update()
    {
        if(targetReachCheckTimer <= 0f)
        {
            targetReachCheckTimer = targetReachCheckInterval;
            CheckTargetReached();
        }

        targetReachCheckTimer -= Time.deltaTime;
    }

    private void CheckTargetReached()
    {
        if(navMeshAgent.pathPending)
        {
            // If path is still pending, do not check
            return;
        }

        // Check if we've reached the destination
        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if(!isTargetReached)
            {
                isTargetReached = true;
            }
        }
        else
        {
            isTargetReached = false;
        }
    }
    private void UpdateState()
    {
        switch(state)
        {
            case ECustomerState.Spawn:
                UpdateState_Spawn();
                break;
            case ECustomerState.MovingToShelf:
                UpdateState_MovingToShelf().Forget();
                break;
            case ECustomerState.GatheringProduct:
                UpdateState_GatheringProduct().Forget();
                break;
            case ECustomerState.MovingToCashRegister:
                UpdateState_MovingToCashRegister().Forget();
                break;
            case ECustomerState.WaitingQueue:
                UpdateState_WaitingQueue().Forget();
                break;
            case ECustomerState.PlacingProductsOnRegister:
                UpdateState_PlacingProductsOnRegister().Forget();
                break;
            case ECustomerState.WaitingCheck:
                UpdateState_WaitingCheck().Forget();
                break;
            case ECustomerState.WaitingTransaction:
                UpdateState_WaitingTransaction().Forget();
                break;
            case ECustomerState.MovingToExit:
                UpdateState_MovingToExit().Forget();
                break;
            default:
                break;
        }
    }
    private void UpdateState_Spawn()
    {
        // make shopping list
        productList = possibleProductListsList[UnityEngine.Random.Range(0, possibleProductListsList.Count)];
        shoppingList = new ShoppingList(productList);
        shoppingCart = new ShoppingCart();
        currentShoppingListIndex = 0;

        // change state
        SetState(ECustomerState.MovingToShelf);
    }
    private async UniTaskVoid UpdateState_MovingToShelf()
    {
        if(currentShoppingListIndex >= shoppingList.Elements.Count)
        {
            if(priceOfProducts > 0f)
            {
                SetState(ECustomerState.MovingToCashRegister);
            }
            else
            {
                SetState(ECustomerState.MovingToExit);
            }
            return;
        }

        ProductSO currentProduct = shoppingList.Elements[currentShoppingListIndex].Product;
        nextShelf = allShelves
            .FirstOrDefault(shelf => shelf.Product == currentProduct);

        if(nextShelf == null)
        {
            // Handle case where product is not found
            Debug.LogWarning($"No shelf found for product: {currentProduct.name}");

            // pick a random shelf
            nextShelf = allShelves
                .OrderBy(_ => UnityEngine.Random.value)
                .FirstOrDefault();

            Transform shelfStopPoint = nextShelf.CustomerStopPoint;
            navMeshAgent.SetDestination(shelfStopPoint.position);
            isTargetReached = false;

            await UniTask.WaitUntil(() => isTargetReached);
            RotateTowards(shelfStopPoint);
            await UniTask.Delay((int)productGatheringTime * 1000);

            currentShoppingListIndex++;
            satisfactionLevel--;
            SetState(ECustomerState.MovingToShelf);
            return;
        }

        Transform nextShelfStopPoint = nextShelf.CustomerStopPoint;
        navMeshAgent.SetDestination(nextShelfStopPoint.position);
        isTargetReached = false;

        await UniTask.WaitUntil(() => isTargetReached);

        selectedShelf = nextShelf;
        RotateTowards(nextShelfStopPoint);

        SetState(ECustomerState.GatheringProduct);
    }
    private async UniTaskVoid UpdateState_GatheringProduct()
    {
        for(int i = 0; i < shoppingList.Elements[currentShoppingListIndex].Amount; i++)
        {
            if(selectedShelf.RemoveProduct())
            {
                shoppingCart.AddToCart(shoppingList.Elements[currentShoppingListIndex].Product, selectedShelf.ProductPrice);
                priceOfProducts += selectedShelf.ProductPrice;
                await UniTask.Delay((int)productGatheringTime * 1000);
            }
        }

        currentShoppingListIndex++;
        satisfactionLevel++;
        selectedShelf = null;

        SetState(ECustomerState.MovingToShelf);
    }
    private async UniTaskVoid UpdateState_MovingToCashRegister()
    {
        selectedCashRegister = allCashRegisters.OrderBy(cashRegister => cashRegister.QueueAmount).FirstOrDefault();
        selectedCashRegister.AddToCustomerQueue(this);

        //Transform cashRegisterStopPoint = selectedCashRegister.CustomerQueueStartPoint;
        currentQueuePosition = selectedCashRegister.GetCustomerQueuePosition(this);
        navMeshAgent.SetDestination(currentQueuePosition);
        isTargetReached = false;

        await UniTask.WaitUntil(() => isTargetReached);

        RotateTowards(selectedCashRegister.transform.position);

        SetState(ECustomerState.WaitingQueue);
    }
    private async UniTaskVoid UpdateState_WaitingQueue()
    {
        await UniTask.WaitUntil(() => selectedCashRegister.PeekCustomerQueue() == this && !selectedCashRegister.IsProcessingProducts);

        SetState(ECustomerState.PlacingProductsOnRegister);
    }
    private async UniTaskVoid UpdateState_PlacingProductsOnRegister()
    {
        if(!hasStartedPlacingProducts)
        {
            isTransactionCompleted = false;
            hasStartedPlacingProducts = true;
            selectedCashRegister.StartProcessingProducts();
        }

        // Place all products from shopping cart on the cash register
        for(int i = 0; i < shoppingCart.Elements.Count; i++)
        {
            ShoppingCartElement element = shoppingCart.Elements[i];
            for(int j = 0; j < element.Amount; j++)
            {
                ProductSO product = element.Product;

                while(selectedCashRegister.AreAllPointsOccupied())
                {
                    Debug.Log("Waiting for an available product place point...");
                    await UniTask.WaitUntil(() => !selectedCashRegister.AreAllPointsOccupied());
                }

                selectedCashRegister.AddProduct(product);

                await UniTask.Delay(500);
            }
        }

        SetState(ECustomerState.WaitingCheck);
    }
    private async UniTaskVoid UpdateState_WaitingCheck()
    {
        // all purchased products placed on the register
        // if there are no products left at the register switch state to waiting transaction

        await UniTask.WaitUntil(() => selectedCashRegister.AreAllPointsEmpty());
        SetState(ECustomerState.WaitingTransaction);
    }
    private async UniTaskVoid UpdateState_WaitingTransaction()
    {
        // customer pays the price either with cash or card
        // we do stuff 
        
        OnWaitingTransaction?.Invoke(selectedCashRegister, priceOfProducts);
        handGO.SetActive(true);

        await UniTask.WaitUntil(() => isTransactionCompleted);
        selectedCashRegister.RemoveFromCustomerQueue();
        SetState(ECustomerState.MovingToExit);
    }
    private async UniTaskVoid UpdateState_MovingToExit()
    {
        hasStartedPlacingProducts = false;
        handGO.SetActive(false);

        Transform exitPointTransform = GameObject.FindGameObjectWithTag("Exit Point").transform;
        navMeshAgent.SetDestination(exitPointTransform.position);
        isTargetReached = false;

        await UniTask.WaitUntil(() => isTargetReached);
        Debug.Log($"{gameObject.name} has completed its cycle.");
        Destroy(gameObject);
    }
    private void SetState(ECustomerState newState)
    {
        if(newState == ECustomerState.MovingToShelf)
        {
            hasStartedPlacingProducts = false;
        }

        state = newState;
        UpdateState();
    }
    private void CreditCard_OnCreditCardTaken()
    {
        handGO.SetActive(false);
    }
    private void CashRegister_OnTransactionCompleted(CashRegister obj)
    {
        if(obj != selectedCashRegister) return;

        isTransactionCompleted = true;
        hasStartedPlacingProducts = false;
    }
    private void RotateTowards(Vector3 targetPosition, float time = 0.5f)
    {
        // For debugging
        Debug.DrawLine(transform.position, targetPosition, Color.red, 2f);

        if(state == ECustomerState.MovingToCashRegister ||
            state == ECustomerState.WaitingQueue ||
            state == ECustomerState.PlacingProductsOnRegister ||
            state == ECustomerState.WaitingCheck ||
            state == ECustomerState.WaitingTransaction)
        {
            // For cash register, rotate to face towards the register
            // Find direction from customer to cash register
            Vector3 directionToCashRegister = (selectedCashRegister.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToCashRegister);
            transform.DOKill(true);
            transform.DORotateQuaternion(lookRotation, time).SetEase(Ease.OutQuad);
        }
        else if(state == ECustomerState.MovingToShelf && nextShelf != null)
        {
            Vector3 directionToFace = -nextShelf.transform.forward;
            Quaternion lookRotation = Quaternion.LookRotation(directionToFace);
            transform.DOKill(true);
            transform.DORotateQuaternion(lookRotation, time).SetEase(Ease.OutQuad);
        }
        else
        {
            // For shelves and other cases, use the position-based rotation
            Vector3 flatTargetPosition = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            Vector3 direction = (flatTargetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.DOKill(true);
            transform.DORotateQuaternion(lookRotation, time).SetEase(Ease.OutQuad);
        }
    }
    private void RotateTowards(Transform target, float time = 0.5f)
    {
        RotateTowards(target.position, time);
    }
    public void UpdateQueuePosition(Vector3 newPosition)
    {
        if(state == ECustomerState.WaitingQueue)
        {
            currentQueuePosition = newPosition;
            MoveToQueuePosition().Forget();
        }
    }
    private async UniTaskVoid MoveToQueuePosition()
    {
        if(isMovingInQueue) return;

        isMovingInQueue = true;
        navMeshAgent.SetDestination(currentQueuePosition);
        isTargetReached = false;

        await UniTask.WaitUntil(() => isTargetReached);

        RotateTowards(selectedCashRegister.transform.position);
        isMovingInQueue = false;
    }
}
