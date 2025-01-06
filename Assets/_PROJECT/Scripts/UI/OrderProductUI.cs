using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class OrderProductUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI productNameText;
    [SerializeField] private TextMeshProUGUI productPriceText;
    [SerializeField] private TextMeshProUGUI orderedAmountText;
    [SerializeField] private Image productImage;

    private int orderedAmount = 0;
    private ProductSO product;

    public static event Action<ProductSO> OnOrderAmountAdded;
    public static event Action<ProductSO> OnOrderAmountRemoved;


    private void OnEnable()
    {
        ComputerUI.OnApplyButtonClicked += ComputerUI_OnApplyButtonClicked;
    }
    private void OnDisable()
    {
        ComputerUI.OnApplyButtonClicked -= ComputerUI_OnApplyButtonClicked;
    }
    private void ComputerUI_OnApplyButtonClicked()
    {
        if(orderedAmount > 0)
        {
            OrderManager.Instance.PlaceOrder(product, orderedAmount);

            orderedAmount = 0;
            UpdateOrderedAmountText();
        }
    }

    public void InitializeProduct(ProductSO product)
    {
        this.product = product;

        productNameText.SetText(this.product.Name.ToUpperInvariant());
        productPriceText.SetText($"Price: <b><size=0.03>${this.product.OrderPrice:F1}</size></b>");
        productImage.sprite = this.product.DisplayImage;
    }
    private void UpdateOrderedAmountText()
    {
        orderedAmountText.SetText(orderedAmount.ToString());
    }
    public void AddButton()
    {
        orderedAmount++;
        UpdateOrderedAmountText();

        OnOrderAmountAdded?.Invoke(product);
    }
    public void RemoveButton()
    {
        if(orderedAmount > 0)
        {
            orderedAmount--;
            UpdateOrderedAmountText();

            OnOrderAmountRemoved?.Invoke(product);
        }
    }
}
