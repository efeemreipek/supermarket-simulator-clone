using UnityEngine;
using TMPro;
using System.Text;
using System;

public class POSMachine : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;

    private StringBuilder priceString = new StringBuilder("$");

    private CashRegister cashRegister;

    public event Action OnTransactionCompleted;

    private void Awake()
    {
        cashRegister = GetComponentInParent<CashRegister>();

        UpdatePriceText();
    }

    public void NumberButton(int number)
    {
        priceString.Append(number);
        UpdatePriceText();
    }
    public void DeleteButton()
    {
        if(priceString.Length > 1)
        {
            priceString.Remove(priceString.Length - 1, 1);
            UpdatePriceText();
        }
    }
    public void ApplyButton()
    {
        string enteredPriceString = priceString.ToString().TrimStart('$');
        if(float.TryParse(enteredPriceString, out float enteredPrice))
        {
            if(Mathf.Approximately(enteredPrice, cashRegister.CurrentPrice))
            {
                OnTransactionCompleted?.Invoke();

                CameraManager.Instance.PrioritizeCamera(ECameraType.Main);
                priceString.Clear();
                priceString.Append("$");
                UpdatePriceText();
            }
            else
            {
                Debug.Log("Wrong price");
            }
        }
        else
        {
            Debug.Log("Invalid");
        }

        
    }
    public void CommaButton()
    {
        // Prevent multiple commas and ensure at least one number exists after $
        if(priceString.ToString().Contains(",") || priceString.Length == 1)
            return;

        priceString.Append(",");
        UpdatePriceText();
    }

    private void UpdatePriceText()
    {
        priceText.SetText(priceString);
    }
}
