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
    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;
    [Header("Main Screen")]
    [SerializeField] private GameObject mainScreenPanel;
    [Header("Market Name")]
    [SerializeField] private GameObject marketNamePanel;
    [SerializeField] private TextMeshProUGUI currentMarketNameText;
    [SerializeField] private TMP_InputField newMarketNameInput;
    [SerializeField] private Toggle boldToggle;
    [SerializeField] private Toggle italicToggle;
    [SerializeField] private Slider redSlider;
    [SerializeField] private TextMeshProUGUI redSliderValueText;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private TextMeshProUGUI greenSliderValueText;
    [SerializeField] private Slider blueSlider;
    [SerializeField] private TextMeshProUGUI blueSliderValueText;
    [SerializeField] private Image colorPreviewImage;

    private Color previewColor = Color.black;
    private float redSliderValue, greenSliderValue, blueSliderValue;

    public bool IsInstructionsPanelActive => instructionsPanel.activeSelf;

    public static event Action OnUIPanelOpened;
    public static event Action OnUIPanelClosed;
    public static event Action<string, Color, bool, bool> OnMarketNameChanged;

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
        currentShelf.ProductPrice = newPrice;
        currentShelf.SetSameProductShelvesPrice(currentShelf.ProductPrice);

        currentShelf = null;
        OnUIPanelClosed?.Invoke();
    }

    public void OpenMarketNamePanel(MarketName marketName)
    {
        marketNamePanel.SetActive(true);

        currentMarketNameText.SetText($"Current Name: {marketName.Name}");

        OnUIPanelOpened?.Invoke();
    }
    public void CloseMarketNamePanel()
    {
        marketNamePanel.SetActive(false);

        OnMarketNameChanged?.Invoke(newMarketNameInput.text, previewColor, boldToggle.isOn, italicToggle.isOn);
        newMarketNameInput.SetTextWithoutNotify(string.Empty);

        OnUIPanelClosed?.Invoke();
    }

    public void OpenCloseInstructions()
    {
        instructionsPanel.SetActive(!instructionsPanel.activeSelf);
    }
    public void OpenClosePause()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public void OnRedSliderValueChanged(float value)
    {
        redSliderValue = value;
        redSliderValueText.SetText(Mathf.RoundToInt(redSliderValue).ToString());

        UpdatePreviewColor();
    }
    public void OnGreenSliderValueChanged(float value)
    {
        greenSliderValue = value;
        greenSliderValueText.SetText(Mathf.RoundToInt(greenSliderValue).ToString());

        UpdatePreviewColor();
    }
    public void OnBlueSliderValueChanged(float value)
    {
        blueSliderValue = value;
        blueSliderValueText.SetText(Mathf.RoundToInt(blueSliderValue).ToString());

        UpdatePreviewColor();
    }
    private void UpdatePreviewColor()
    {
        previewColor = new Color(redSliderValue / 255f, greenSliderValue / 255f, blueSliderValue / 255f);
        colorPreviewImage.color = previewColor;
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

    // MAIN SCREEN
    public void PlayButton()
    {
        mainScreenPanel.SetActive(false);
        PauseManager.Instance.IsPaused = false;
    }
    public void QuitButton()
    {
        Application.Quit();
    }
}
