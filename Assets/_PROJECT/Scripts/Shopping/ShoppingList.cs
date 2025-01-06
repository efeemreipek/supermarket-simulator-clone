using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShoppingList
{
    public List<ShoppingListElement> Elements = new List<ShoppingListElement>();

    public ShoppingList(ProductListSO productList)
    {
        foreach(ProductSO product in productList.ProductList)
        {
            int amount = Random.Range(0, 3);
            if(amount > 0)
            {
                Elements.Add(new ShoppingListElement(product, amount));
            }
        }

        if(Elements.Count == 0)
        {
            Elements.Add(new ShoppingListElement(productList.ProductList[Random.Range(0, productList.ProductList.Count)], 1));
        }
    }
}
