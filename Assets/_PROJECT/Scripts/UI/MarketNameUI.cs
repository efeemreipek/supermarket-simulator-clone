using UnityEngine;
using TMPro;

public class MarketNameUI : MonoBehaviour
{
    private TextMeshPro marketNameText;

    private void Awake()
    {
        marketNameText = GetComponent<TextMeshPro>();
    }

    public void SetName(string name)
    {
        marketNameText.SetText(name);
    }
}
