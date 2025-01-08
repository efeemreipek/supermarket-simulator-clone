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
        marketNameUI.SetName(Name);
    }
    private void OnEnable()
    {
        UIManager.OnMarketNameChanged += UIManager_OnMarketNameChanged;
    }
    private void OnDisable()
    {
        UIManager.OnMarketNameChanged -= UIManager_OnMarketNameChanged;
    }

    private void UIManager_OnMarketNameChanged(string obj)
    {
        Name = obj;
        marketNameUI.SetName(Name);
    }
}
