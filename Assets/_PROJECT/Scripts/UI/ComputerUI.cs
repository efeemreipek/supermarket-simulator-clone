using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ComputerUI : MonoBehaviour
{
    [SerializeField] private GameObject orderProductUIPrefab;
    [SerializeField] private ProductListSO productList;
    [SerializeField] private Transform productsListTransform;
    [SerializeField] private Button applyButton;
    [SerializeField] private TextMeshProUGUI totalPriceAmountText;

    private decimal totalPriceAmount = 0m;

    public static event Action OnApplyButtonClicked;

    private void Start()
    {
        InitializeProducts();
        UpdateTotalPriceText();
    }
    private void OnEnable()
    {
        OrderProductUI.OnOrderAmountAdded += OrderProductUI_OnOrderAmountAdded;
        OrderProductUI.OnOrderAmountRemoved += OrderProductUI_OnOrderAmountRemoved;
    }
    private void OnDisable()
    {
        OrderProductUI.OnOrderAmountAdded -= OrderProductUI_OnOrderAmountAdded;
        OrderProductUI.OnOrderAmountRemoved -= OrderProductUI_OnOrderAmountRemoved;
    }

    private void InitializeProducts()
    {
        if(productsListTransform.childCount > 0)
        {
            foreach(Transform t in productsListTransform)
            {
                Destroy(t.gameObject);
            }
        }

        foreach(ProductSO product in productList.ProductList)
        {
            GameObject orderProductGO = Instantiate(orderProductUIPrefab, productsListTransform, false);
            orderProductGO.GetComponent<OrderProductUI>().InitializeProduct(product);
        }
    }
    private void OrderProductUI_OnOrderAmountAdded(ProductSO product)
    {
        totalPriceAmount = Math.Max(totalPriceAmount + Convert.ToDecimal(product.OrderPrice), 0m);
        UpdateTotalPriceText();
    }
    private void OrderProductUI_OnOrderAmountRemoved(ProductSO product)
    {
        totalPriceAmount = Math.Max(totalPriceAmount - Convert.ToDecimal(product.OrderPrice), 0m);
        UpdateTotalPriceText();
    }
    private void UpdateTotalPriceText()
    {
        totalPriceAmountText.SetText($"${totalPriceAmount:F1}");
    }
    public void ApplyButton()
    {
        OnApplyButtonClicked?.Invoke();

        BalanceManager.Instance.ChangeBalance(-totalPriceAmount);

        totalPriceAmount = 0m;
        UpdateTotalPriceText();

        CameraManager.Instance.PrioritizeCamera(ECameraType.Main);
    }
}
