using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ComputerUI : MonoBehaviour
{
    [SerializeField] private GameObject orderProductUIPrefab;
    [SerializeField] private ProductListSO productList;
    [SerializeField] private Transform productsListTransform;
    [SerializeField] private TextMeshProUGUI totalPriceAmountText;
    [SerializeField] private GameObject mainPagePanel;
    [SerializeField] private GameObject orderProductPanel;
    [SerializeField] private GameObject payRentPanel;
    [SerializeField] private TextMeshProUGUI amountToPayRentText;
    [SerializeField] private TextMeshProUGUI additionalInfoText;

    private decimal totalPriceAmount = 0m;
    private decimal baseRentAmount = 20m;
    private bool isRentPaid = false;

    public static event Action OnApplyButtonClicked;

    private void Start()
    {
        InitializeProducts();
        UpdateTotalPriceText();

        amountToPayRentText.SetText($"Amount to pay: ${baseRentAmount}");
    }
    private void OnEnable()
    {
        OrderProductUI.OnOrderAmountAdded += OrderProductUI_OnOrderAmountAdded;
        OrderProductUI.OnOrderAmountRemoved += OrderProductUI_OnOrderAmountRemoved;
        TimeManager.OnDayChanged += TimeManager_OnDayChanged;
    }
    private void OnDisable()
    {
        OrderProductUI.OnOrderAmountAdded -= OrderProductUI_OnOrderAmountAdded;
        OrderProductUI.OnOrderAmountRemoved -= OrderProductUI_OnOrderAmountRemoved;
        TimeManager.OnDayChanged -= TimeManager_OnDayChanged;
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
    private void TimeManager_OnDayChanged()
    {
        baseRentAmount = 20m;
        isRentPaid = false;
    }
    private void UpdateTotalPriceText()
    {
        totalPriceAmountText.SetText($"${totalPriceAmount:F1}");
    }
    public void ApplyOrdersButton()
    {
        OnApplyButtonClicked?.Invoke();

        BalanceManager.Instance.ChangeBalance(-totalPriceAmount);

        totalPriceAmount = 0m;
        UpdateTotalPriceText();
    }
    public void PayRentButton()
    {
        if(BalanceManager.Instance.CurrentBalance < baseRentAmount)
        {
            additionalInfoText.SetText("You don't have enough money.");
        }
        else
        {
            BalanceManager.Instance.ChangeBalance(-baseRentAmount);
            baseRentAmount = 0m;
            amountToPayRentText.SetText($"Amount to pay: ${baseRentAmount}");
            additionalInfoText.SetText("You paid your rent today");
            isRentPaid = true;
        }
    }
    public void OpenOrderProductMenu()
    {
        mainPagePanel.SetActive(false);
        orderProductPanel.SetActive(true);
    }
    public void OpenPayRentMenu()
    {
        amountToPayRentText.SetText($"Amount to pay: ${baseRentAmount}");
        additionalInfoText.SetText(isRentPaid ? "You paid your rent today" : string.Empty);

        mainPagePanel.SetActive(false);
        payRentPanel.SetActive(true);
    }
    public void BackButton()
    {
        if(orderProductPanel.activeSelf) orderProductPanel.SetActive(false);
        else payRentPanel.SetActive(false);

        mainPagePanel.SetActive(true);
    }
    public void CloseButton()
    {
        CameraManager.Instance.PrioritizeCamera(ECameraType.Main);
    }
}
