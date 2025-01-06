using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private Transform orderSpawnPoint;

    public void PlaceOrder(ProductSO product, int amount)
    {
        Debug.Log($"Ordered {amount} amount of {product.Name} boxes");
        for(int i = 0; i < amount; i++)
        {
            GameObject boxGO = Instantiate(boxPrefab, orderSpawnPoint.position, Quaternion.identity);
            boxGO.GetComponent<Box>().InitializeBox(product);
        }
    }
}
