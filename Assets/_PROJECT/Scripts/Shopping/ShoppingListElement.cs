[System.Serializable]
public struct ShoppingListElement
{
    public ProductSO Product;
    public int Amount;

    public ShoppingListElement(ProductSO product, int amount)
    {
        Product = product;
        Amount = amount;
    }
}
