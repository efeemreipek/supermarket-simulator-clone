using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [Header("Shelf")]
    [SerializeField] private GameObject shelfPricePanel;
    [SerializeField] private TextMeshProUGUI shelfProductNameText;
    [SerializeField] private Image shelfProductDisplayImage;
    [SerializeField] private TextMeshProUGUI shelfProductCurrentPriceText;
    [SerializeField] private TextMeshProUGUI shelfProductSuggestedPriceText;
    [SerializeField] private TMP_InputField shelfProductNewPriceInput;
    [Header("Balance")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [Header("Instructions")]
    [SerializeField] private GameObject instructionsPanel;

    public static event Action OnUIPanelOpened;
    public static event Action OnUIPanelClosed;

    private Shelf currentShelf;

    public void OpenShelfPricePanel(Shelf shelf)
    {
        if(!shelf.HasProduct) return;

        currentShelf = shelf;

        if(!shelfPricePanel.activeSelf)
        {
            shelfPricePanel.SetActive(true);
        }

        shelfProductNameText.SetText(currentShelf.Product.Name);
        shelfProductDisplayImage.sprite = currentShelf.Product.DisplayImage;
        shelfProductCurrentPriceText.SetText($"Current Price: ${currentShelf.ProductPrice.ToString()}");
        shelfProductSuggestedPriceText.SetText($"Suggested Price: ${currentShelf.Product.SuggestedPrice}");
        shelfProductNewPriceInput.SetTextWithoutNotify("$" + currentShelf.ProductPrice.ToString());

        OnUIPanelOpened?.Invoke();
    }
    public void CloseShelfPricePanel()
    {
        string priceString = shelfProductNewPriceInput.text.Trim('$');
        float price = float.Parse(priceString);
        if(price <= 0f) return;

        if(shelfPricePanel.activeSelf)
        {
            shelfPricePanel.SetActive(false);
        }

        priceString = shelfProductNewPriceInput.text.Trim('$');
        float newPrice = float.Parse(priceString);
        if(newPrice != currentShelf.ProductPrice)
        {
            currentShelf.ProductPrice = newPrice;
            currentShelf.SetSameProductShelvesPrice(currentShelf.ProductPrice);
        }

        currentShelf = null;
        OnUIPanelClosed?.Invoke();
    }

    public void OpenCloseInstructions()
    {
        instructionsPanel.SetActive(!instructionsPanel.activeSelf);
    }

    public void ChangeAmount(float amount)
    {
        string priceString = shelfProductNewPriceInput.text.Trim('$');
        float price = float.Parse(priceString);
        price = Mathf.Max(price + amount, 0f);
        shelfProductNewPriceInput.SetTextWithoutNotify("$" + price.ToString());
    }
    public void ChangeBalanceText(decimal newBalance)
    {
        balanceText.SetText($"${newBalance:F1}");
    }
}
