using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;

public class CashRegister : MonoBehaviour
{
    [SerializeField] private Transform customerQueueStartPoint;
    [SerializeField] private Transform productPlacePointsTransform;
    [SerializeField] private float productCheckDistance = 2f;
    [SerializeField] private float productCheckTime = 0.5f;

    private List<Transform> productPlacePoints = new List<Transform>();
    private List<ProductSO> productsOnRegister = new List<ProductSO>();
    private float currentPrice = 0f;
    private Queue<Customer> customerQueue = new Queue<Customer>();
    private Dictionary<Customer, Vector3> customerPositions = new Dictionary<Customer, Vector3>();
    private bool isProcessingProducts = false;

    private POSMachine posMachine;

    public Transform CustomerQueueStartPoint => customerQueueStartPoint;
    public float CurrentPrice => currentPrice;
    public int QueueAmount => customerQueue.Count;
    public bool IsEmpty => QueueAmount == 0;
    public bool IsProcessingProducts => isProcessingProducts;

    public event Action<BaseProduct> OnProductChecked;
    public event Action<float> OnProductCheckEnded;
    public static event Action<CashRegister> OnTransactionCompleted;

    private void Awake()
    {
        posMachine = GetComponentInChildren<POSMachine>();

        foreach(Transform t in productPlacePointsTransform)
        {
            productPlacePoints.Add(t);
        }
    }
    private void OnEnable()
    {
        Customer.OnWaitingTransaction += Customer_OnWaitingTransaction;
        posMachine.OnTransactionCompleted += POSMachine_OnTransactionCompleted;
    }
    private void OnDisable()
    {
        Customer.OnWaitingTransaction -= Customer_OnWaitingTransaction;
        posMachine.OnTransactionCompleted -= POSMachine_OnTransactionCompleted;
    }

    public void StartProcessingProducts()
    {
        isProcessingProducts = true;
    }
    public void AddProduct(ProductSO product)
    {
        Transform productPlace = FindAvailablePoint();
        if(productPlace == null) return;

        productsOnRegister.Add(product);

        GameObject productGO = Instantiate(product.Prefab, productPlace.position, Quaternion.identity);
        productGO.transform.SetParent(productPlace);
        productGO.GetComponent<BaseProduct>().IsAtRegister = true;
    }
    public async void RemoveProductFromPoint(Transform point)
    {
        if(point.childCount > 0)
        {
            Transform productTransform = point.GetChild(0);
            BaseProduct product = productTransform.GetComponent<BaseProduct>();
            OnProductChecked?.Invoke(product);

            await productTransform.DOLocalMoveZ(productTransform.localPosition.z + productCheckDistance, productCheckTime)
                .SetEase(Ease.OutQuint)
                .ToUniTask();
            Destroy(productTransform.gameObject);
        }
    }
    private void Customer_OnWaitingTransaction(CashRegister cashRegister, float price)
    {
        if(cashRegister != this) return;

        currentPrice = price;
        OnProductCheckEnded?.Invoke(currentPrice);
    }
    private void POSMachine_OnTransactionCompleted()
    {
        isProcessingProducts = false;
        OnTransactionCompleted?.Invoke(this);
        BalanceManager.Instance.ChangeBalance(currentPrice);
        currentPrice = 0f;
    }

    private Transform FindAvailablePoint()
    {
        foreach(Transform t in productPlacePoints)
        {
            if(t.childCount > 0) continue;
            return t;
        }
        return null;
    }
    public bool AreAllPointsOccupied()
    {
        foreach(Transform t in productPlacePoints)
        {
            if(t.childCount == 0) return false;
        }
        return true;
    }
    public bool AreAllPointsEmpty()
    {
        foreach(Transform t in productPlacePoints)
        {
            if(t.childCount > 0) return false;
        }
        return true;
    }
    public Vector3 FindAvailableQueuePoint()
    {
        Vector3 queuePoint = customerQueueStartPoint.position;
        for(int i = 0; i < QueueAmount - 1; i++)
        {
            queuePoint.z -= 1.5f;
        }
        return queuePoint;
    }
    public void AddToCustomerQueue(Customer customer)
    {
        customerQueue.Enqueue(customer);
        UpdateQueuePositions();
    }
    public void RemoveFromCustomerQueue()
    {
        customerQueue.Dequeue();
        UpdateQueuePositions();
    }
    public Customer PeekCustomerQueue()
    {
        return customerQueue.Peek();
    }

    private void UpdateQueuePositions()
    {
        Vector3 queuePoint = customerQueueStartPoint.position;
        Customer[] queueArray = customerQueue.ToArray();
        customerPositions.Clear();

        for(int i = 0; i < queueArray.Length; i++)
        {
            Vector3 position = customerQueueStartPoint.position;
            if(i > 0)
            {
                position += i * 1.5f * transform.right;
            }
            customerPositions[queueArray[i]] = position;
            queueArray[i].UpdateQueuePosition(position);
        }
    }
    public Vector3 GetCustomerQueuePosition(Customer customer)
    {
        return customerPositions.TryGetValue(customer, out Vector3 position) ? position : customerQueueStartPoint.position;
    }
}
