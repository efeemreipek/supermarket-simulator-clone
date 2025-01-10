using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    [Header("Settings")]
    [SerializeField] private SettingsSO settings;
    [SerializeField] private Volume volume;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TextMeshProUGUI mouseSensitivitySliderValueText;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessSliderValueText;
    [Header("Time")]
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;

    private Color previewColor = Color.black;
    private float redSliderValue, greenSliderValue, blueSliderValue;
    private float mouseSensitivitySliderValue, brightnessSliderValue;
    private bool isFullscreen = true;
    private Resolution[] resolutions;
    private Resolution defaultResolution;
    private int defaultResolutionIndex;
    private int defaultQualityLevel;
    private ColorAdjustments colorAdjustments;

    public bool IsInstructionsPanelActive => instructionsPanel.activeSelf;

    public static event Action OnUIPanelOpened;
    public static event Action OnUIPanelClosed;
    public static event Action<string, Color, bool, bool> OnMarketNameChanged;

    private Shelf currentShelf;

    private void Start()
    {
        LoadSettings();
    }

    #region Shelf
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
    #endregion
    #region Market Name
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
    #endregion
    #region Main Screen
    public void PlayButton()
    {
        mainScreenPanel.SetActive(false);
        PauseManager.Instance.IsPaused = false;
        PauseManager.Instance.IsGameStarted = true;
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void SettingsButton()
    {
        if(PauseManager.Instance.IsGameStarted)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            mainScreenPanel.SetActive(false);
        }
        settingsPanel.SetActive(true);
    }
    public void ResumeButton()
    {
        PauseManager.Instance.UnpauseGame();
    }
    #endregion
    #region Settings
    private void LoadSettings()
    {
        // load mouse sensitivity
        mouseSensitivitySliderValue = settings.IsSet ? settings.MouseSensitivity : mouseSensitivitySlider.value;
        mouseSensitivitySlider.value = mouseSensitivitySliderValue;
        mouseSensitivitySliderValueText.SetText(mouseSensitivitySliderValue.ToString("F2"));

        // load brightness
        volume.profile.TryGet(out colorAdjustments);
        brightnessSliderValue = settings.IsSet ? settings.Brightness : brightnessSlider.value;
        brightnessSlider.value = brightnessSliderValue;
        brightnessSliderValueText.SetText(brightnessSliderValue.ToString());
        colorAdjustments.postExposure.value = settings.IsSet ? settings.Brightness / 25 : brightnessSliderValue / 25;

        // initalize and load quality dropdown
        defaultQualityLevel = settings.IsSet ? settings.QualityIndex : QualitySettings.GetQualityLevel();
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.SetValueWithoutNotify(defaultQualityLevel);

        // initialize resolution dropdown
        resolutions = Screen.resolutions;
        List<string> resolutionDropdownOptions = new List<string>();
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + "@" + (int)resolutions[i].refreshRateRatio.value;
            resolutionDropdownOptions.Add(option);
        }

        // load resolution
        if(settings.IsSet)
        {
            defaultResolutionIndex = settings.ResolutionIndex;
        }
        else
        {
            // get current resolution
            string currentResolutionString = Screen.currentResolution.width + "x" + Screen.currentResolution.height
                                             + " @" + (int)Screen.currentResolution.refreshRateRatio.value;
            defaultResolutionIndex = resolutionDropdownOptions.IndexOf(currentResolutionString);
            // if current resolution is not in the list then add it 
            if(defaultResolutionIndex < 0)
            {
                resolutionDropdownOptions.Add(currentResolutionString);
                defaultResolutionIndex = resolutionDropdownOptions.Count - 1;
                defaultResolution = Screen.currentResolution;
            }
            else
            {
                defaultResolution = resolutions[defaultResolutionIndex];
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionDropdownOptions);
        resolutionDropdown.SetValueWithoutNotify(defaultResolutionIndex);

        //load fullscreen
        isFullscreen = settings.IsSet ? settings.IsFullscreen : fullscreenToggle.isOn;
        fullscreenToggle.isOn = isFullscreen;
    }
    public void BackButton()
    {
        settingsPanel.SetActive(false);
        if(PauseManager.Instance.IsGameStarted)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            mainScreenPanel.SetActive(true);
        }
    }
    public void ApplyButton()
    {
        settings.IsSet = true;

        settings.MouseSensitivity = mouseSensitivitySliderValue;
        
        settings.QualityIndex = defaultQualityLevel;
        QualitySettings.SetQualityLevel(defaultQualityLevel);

        settings.ResolutionIndex = defaultResolutionIndex;
        if(defaultResolutionIndex >= resolutions.Length)
        {
            SetResolution(defaultResolution);
        }
        else
        {
            SetResolution(resolutions[defaultResolutionIndex]);
        }

        settings.IsFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;

        if(colorAdjustments != null)
        { 
            settings.Brightness = brightnessSliderValue;
            colorAdjustments.postExposure.value = brightnessSliderValue / 25;
        }
    }
    public void OnMouseSensitivitySliderValueChanged(float value)
    {
        mouseSensitivitySliderValue = value;
        mouseSensitivitySliderValueText.SetText(mouseSensitivitySliderValue.ToString("F2"));
    }
    public void OnBrightnessSliderValueChanged(float value)
    {
        brightnessSliderValue = value;
        brightnessSliderValueText.SetText(brightnessSliderValue.ToString());
    }
    public void SetFullscreen(bool toggle) => isFullscreen = toggle;
    public void OnQualityChanged(int qualityIndex) => defaultQualityLevel = qualityIndex;
    public void OnResolutionChanged(int resolutionIndex) => defaultResolutionIndex = resolutionIndex;
    private void SetResolution(Resolution resolution) => Screen.SetResolution(resolution.width, resolution.height, 
        Screen.fullScreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed, resolution.refreshRateRatio);
    #endregion
    #region Misc
    public void OpenCloseInstructions()
    {
        instructionsPanel.SetActive(!instructionsPanel.activeSelf);
    }
    public void OpenClosePause()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
    }
    #endregion
    public void UpdateDayTimeText(string dayString, string timeString)
    {
        dayText.SetText(dayString);
        timeText.SetText(timeString);
    }
}
