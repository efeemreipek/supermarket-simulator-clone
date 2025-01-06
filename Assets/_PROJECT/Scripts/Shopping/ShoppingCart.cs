using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ShoppingCart
{
    public List<ShoppingCartElement> Elements = new List<ShoppingCartElement>();

    public void AddToCart(ProductSO product, float price, int amount = 1)
    {
        // Find the index of the existing product in the cart
        int index = Elements.FindIndex(element => element.Product == product);

        if(index >= 0)
        {
            // Update the existing element's amount and price
            ShoppingCartElement existingElement = Elements[index];
            existingElement.IncreaseAmount();
            Elements[index] = existingElement; // Update the list with the modified struct
        }
        else
        {
            // Add a new element to the cart
            ShoppingCartElement element = new ShoppingCartElement(product, amount, price);
            Elements.Add(element);
        }
    }
}
