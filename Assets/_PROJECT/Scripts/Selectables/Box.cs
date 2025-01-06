using System;
using UnityEngine;

public class Box : Selectable
{
    [SerializeField] private ProductSO product;
    [SerializeField] private int productAmount = 10;

    private int productLeft;

    public bool HasProduct => productLeft > 0;
    public ProductSO Product => product;
    public int ProductAmountLeft => productLeft;

    public event Action<int> OnProductAmountChanged;
    public event Action OnBoxInitialized;

    protected override void Awake()
    {
        base.Awake();

        productLeft = productAmount;
    }
    public void InitializeBox(ProductSO product)
    {
        this.product = product;
        OnBoxInitialized?.Invoke();
    }

    public bool RemoveProductAmount()
    {
        if(HasProduct)
        {
            productLeft--;
            OnProductAmountChanged?.Invoke(productLeft);
            return true;
        }
        return false;
    }
    public bool AddProductAmount()
    {
        if(productLeft < productAmount)
        {
            productLeft++;
            OnProductAmountChanged?.Invoke(productLeft);
            return true;
        }
        return false;
    }
}
