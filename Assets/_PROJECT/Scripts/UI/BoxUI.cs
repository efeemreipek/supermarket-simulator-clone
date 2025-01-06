using UnityEngine;
using TMPro;

public class BoxUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro productNameText;
    [SerializeField] private TextMeshPro productAmountText;

    private Box box;

    private void Awake()
    {
        box = GetComponent<Box>();
    }
    private void Start()
    {
        productNameText.SetText(box.Product.Name);
        productAmountText.SetText(box.ProductAmountLeft.ToString());
    }
    private void OnEnable()
    {
        box.OnProductAmountChanged += Box_OnProductAmountChanged;
        box.OnBoxInitialized += Box_OnBoxInitialized;
    }
    private void OnDisable()
    {
        box.OnProductAmountChanged -= Box_OnProductAmountChanged;
        box.OnBoxInitialized -= Box_OnBoxInitialized;
    }
    private void Box_OnBoxInitialized()
    {
        productNameText.SetText(box.Product.Name);
        productAmountText.SetText(box.ProductAmountLeft.ToString());
    }

    private void Box_OnProductAmountChanged(int obj)
    {
        productAmountText.SetText(obj.ToString());
    }
}
