using System.Diagnostics;
using TMPro;
using UnityEngine;

public class CashRegisterUI : MonoBehaviour
{
    [SerializeField] private GameObject checkedProductTextPrefab;
    [SerializeField] private Transform productsSectionTransform;
    [SerializeField] private TextMeshProUGUI totalPriceAmountText;

    private CashRegister cashRegister;

    private void Awake()
    {
        cashRegister = GetComponent<CashRegister>();

        totalPriceAmountText.SetText("$");
    }

    private void OnEnable()
    {
        cashRegister.OnProductChecked += CashRegister_OnProductChecked;
        cashRegister.OnProductCheckEnded += CashRegister_OnProductCheckEnded;
        CashRegister.OnTransactionCompleted += CashRegister_OnTransactionCompleted;
    }
    private void OnDisable()
    {
        cashRegister.OnProductChecked -= CashRegister_OnProductChecked;
        cashRegister.OnProductCheckEnded -= CashRegister_OnProductCheckEnded;
        CashRegister.OnTransactionCompleted -= CashRegister_OnTransactionCompleted;
    }

    private void CashRegister_OnProductChecked(BaseProduct product)
    {
        GameObject checkedProductTextGO = Instantiate(checkedProductTextPrefab, productsSectionTransform.position, productsSectionTransform.rotation);
        checkedProductTextGO.GetComponent<CheckedProductUI>().SetText(product.Product.Name);
        checkedProductTextGO.transform.SetParent(productsSectionTransform);
    }
    private void CashRegister_OnProductCheckEnded(float price)
    {
        totalPriceAmountText.SetText($"${price}");
    }
    private void CashRegister_OnTransactionCompleted(CashRegister obj)
    {
        if(obj != cashRegister) return;

        for(int i = productsSectionTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(productsSectionTransform.GetChild(i).gameObject);
        }

        totalPriceAmountText.SetText("$");
    }
}
