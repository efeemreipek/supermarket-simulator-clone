using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductList", menuName = "Scriptable Objects/Product List")]
public class ProductListSO : ScriptableObject
{
    public List<ProductSO> ProductList = new List<ProductSO>();
}
