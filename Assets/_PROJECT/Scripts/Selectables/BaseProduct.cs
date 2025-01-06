using UnityEngine;

public class BaseProduct : Selectable
{
    [SerializeField] private bool isAtRegister = false;
    [SerializeField] private ProductSO product;

    private Shelf accordingShelf;

    private float price;

    public bool IsAtRegister { get => isAtRegister; set => isAtRegister = value; }
    public ProductSO Product => product;
    public Shelf AccordingShelf { get => accordingShelf; set => accordingShelf = value; }
    public float Price {  get => price; set => price = value;}
}
