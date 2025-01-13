using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Product", menuName = "Scriptable Objects/Product")]
public class ProductSO : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public Sprite DisplayImage;
    public float OrderPrice;

    private float profitPercentage = 0.2f;
    private float maxPricePercantage = 0.25f;

    public float SuggestedPrice => Mathf.Round((OrderPrice + OrderPrice * profitPercentage) / 0.1f) * 0.1f;
    public float MaxPrice => Mathf.Round((OrderPrice + OrderPrice * maxPricePercantage) / 0.1f) * 0.1f;
}
