using UnityEngine;

public class MarketName : Selectable
{
    private string _name = "Your Market";
    public string Name { get => _name; private set => _name = value; }

    private MarketNameUI marketNameUI;

    protected override void Awake()
    {
        base.Awake();

        marketNameUI = GetComponentInChildren<MarketNameUI>();
        marketNameUI.SetName(Name, Color.black, false, false);
    }
    private void OnEnable()
    {
        UIManager.OnMarketNameChanged += UIManager_OnMarketNameChanged;
    }
    private void OnDisable()
    {
        UIManager.OnMarketNameChanged -= UIManager_OnMarketNameChanged;
    }

    private void UIManager_OnMarketNameChanged(string name, Color color, bool bold, bool italic)
    {
        Name = name;
        marketNameUI.SetName(Name, color, bold, italic);
    }
}
