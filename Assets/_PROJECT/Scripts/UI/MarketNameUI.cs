using UnityEngine;
using TMPro;

public class MarketNameUI : MonoBehaviour
{
    private TextMeshPro marketNameText;

    private void Awake()
    {
        marketNameText = GetComponent<TextMeshPro>();
    }

    public void SetName(string name, Color color, bool bold, bool italic)
    {
        marketNameText.SetText(name);

        marketNameText.color = color;

        FontStyles fontStyle = FontStyles.Normal;
        if(bold) fontStyle |= FontStyles.Bold;
        if(italic) fontStyle |= FontStyles.Italic;

        marketNameText.fontStyle = fontStyle;

    }
}
