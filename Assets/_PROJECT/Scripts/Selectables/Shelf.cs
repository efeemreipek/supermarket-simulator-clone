using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Shelf : Selectable
{
    [SerializeField] private Transform pointsTransform;
    [SerializeField] private Transform customerStopPoint;

    private List<Transform> productPoints = new List<Transform>();
    private ProductSO shelfProduct = null;
    private int productAmount;
    private float productPrice;

    public bool CanPlaceProduct => productAmount < productPoints.Count;
    public bool HasProduct => shelfProduct != null;
    public ProductSO Product => shelfProduct;
    public float ProductPrice { get => productPrice; 
                                set
                                { 
                                    productPrice = value;
                                    OnProductPriceChanged?.Invoke();
                                } }
    public Transform CustomerStopPoint => customerStopPoint;

    public event Action OnProductPriceChanged;

    protected override void Awake()
    {
        base.Awake();

        foreach(Transform t in pointsTransform)
        {
            productPoints.Add(t);
        }
    }

    public void PlaceProduct(Box box)
    {
        if(!CanPlaceProduct) return;
        if(!box.HasProduct) return;
        if(shelfProduct != null && shelfProduct != box.Product) return;

        Transform point = FindAvailablePoint();
        GameObject productGO = Instantiate(box.Product.Prefab, point.position, point.rotation);
        productGO.transform.SetParent(point);
        BaseProduct baseProduct = productGO.GetComponent<BaseProduct>();
        baseProduct.AccordingShelf = this;
        box.RemoveProductAmount();
        productAmount++;
        if(shelfProduct == null)
        {
            shelfProduct = box.Product;
            SetShelfPriceSameAsOthers();
        }
    }
    public void RemoveProduct(Box box, BaseProduct product)
    {
        if(!box.AddProductAmount()) return;

        product.transform.parent.DetachChildren();
        Destroy(product.GameObject);
        productAmount--;
        if(productAmount == 0)
        {
            shelfProduct = null;
            ProductPrice = 0f;
        }
    }
    public bool RemoveProduct(out ProductSO product, out float productPrice)
    {
        if(productAmount <= 0)
        {
            product = null;
            productPrice = 0f;
            return false;
        }

        Transform point = FindLastOccupiedPoint();
        Destroy(point.GetChild(0).gameObject);
        productAmount--;

        product = Product;
        productPrice = ProductPrice;

        if(productAmount == 0)
        {
            shelfProduct = null;
            ProductPrice = 0f;
        }

        return true;
    }
    private Transform FindAvailablePoint()
    {
        foreach(Transform t in productPoints)
        {
            if(t.childCount > 0) continue;
            return t;
        }
        return null;
    }
    private Transform FindLastOccupiedPoint()
    {
        for(int i = productPoints.Count - 1; i >= 0; i--)
        {
            Transform t = productPoints[i];
            if(t.childCount > 0) return t;
        }
        return null;
    }
    private void SetShelfPriceSameAsOthers()
    {
        List<Shelf> sameProductShelves = FindObjectsByType<Shelf>(FindObjectsSortMode.None)
            .Where(shelf => shelf.Product == Product)
            .ToList();

        ProductPrice = sameProductShelves.FirstOrDefault().ProductPrice;

    }

    public void SetSameProductShelvesPrice(float price)
    {
        List<Shelf> sameProductShelves = FindObjectsByType<Shelf>(FindObjectsSortMode.None)
            .Where(shelf => shelf.Product == Product)
            .ToList();

        foreach(Shelf s in sameProductShelves)
        {
            s.ProductPrice = price;
        }
    }
}
