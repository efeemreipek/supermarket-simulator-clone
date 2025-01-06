using TMPro;
using UnityEngine;

public class ShelfUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro priceText;

    private Shelf shelf;

    private void Awake()
    {
        shelf = GetComponent<Shelf>();
        priceText.SetText("$" + shelf.ProductPrice.ToString());
    }
    private void OnEnable()
    {
        shelf.OnProductPriceChanged += Shelf_OnProductPriceChanged;
    }
    private void OnDisable()
    {
        shelf.OnProductPriceChanged -= Shelf_OnProductPriceChanged;
    }

    private void Shelf_OnProductPriceChanged()
    {
        priceText.SetText("$" + shelf.ProductPrice.ToString());
    }
}
