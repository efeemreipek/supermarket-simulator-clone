using UnityEngine;
using TMPro;

public class CheckedProductUI : MonoBehaviour
{
    private TextMeshProUGUI checkedProductText;

    private void Awake()
    {
        checkedProductText = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string text)
    {
        checkedProductText.SetText(text);
    }
}
