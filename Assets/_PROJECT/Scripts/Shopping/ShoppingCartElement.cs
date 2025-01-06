[System.Serializable]
public struct ShoppingCartElement
{
    public ProductSO Product;
    public int Amount;
    public float Price;

    public ShoppingCartElement(ProductSO product, int amount, float price)
    {
        Product = product;
        Amount = amount;
        Price = price;
    }
    public void IncreaseAmount()
    {
        Amount++;
    }
}

