using System;
using UnityEngine;

public class BalanceManager : Singleton<BalanceManager>
{
    [SerializeField] private float startBalance = 10f;
    private decimal balance = 0m;

    private decimal moneyEarned = 0m;
    private decimal moneySpent = 0m;

    private void Start()
    {
        balance = Convert.ToDecimal(startBalance);
        UIManager.instance.ChangeBalanceText(balance);
    }
    private void OnEnable()
    {
        TimeManager.OnDayChanged += TimeManager_OnDayChanged;
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= TimeManager_OnDayChanged;
    }

    private void TimeManager_OnDayChanged()
    {
        PauseManager.Instance.UnpauseGame();

        UIManager.Instance.OpenCloseEndOfDay();

        string moneyEarnedString = $"Money Earned: ${moneyEarned}";
        string moneySpentString = $"Money Spent: ${moneySpent}";
        string totalProfitString;
        decimal totalProfit = moneyEarned + moneySpent;
        if(totalProfit < 0) totalProfitString = $"TOTAL PROFIT: -${totalProfit * -1}";
        else totalProfitString = $"TOTAL PROFIT: ${totalProfit}";

        UIManager.Instance.UpdateEndOfDayText(moneyEarnedString, moneySpentString, totalProfitString);

        moneyEarned = 0m;
        moneySpent = 0m;
    }

    public void ChangeBalance(float amount)
    {
        if(amount < 0f) moneySpent += Convert.ToDecimal(amount);
        else moneyEarned += Convert.ToDecimal(amount);

        balance = Math.Max(balance + Convert.ToDecimal(amount), 0);
        UIManager.instance.ChangeBalanceText(balance);
    }
    public void ChangeBalance(decimal amount)
    {
        if(amount < 0m) moneySpent += amount;
        else moneyEarned += amount;

        balance = Math.Max(balance + amount, 0);
        UIManager.instance.ChangeBalanceText(balance);
    }
}
