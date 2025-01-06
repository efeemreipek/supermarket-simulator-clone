using System;
using UnityEngine;

public class BalanceManager : Singleton<BalanceManager>
{
    [SerializeField] private float startBalance = 10f;
    private decimal balance = 0m;

    private void Start()
    {
        balance = Convert.ToDecimal(startBalance);
        UIManager.instance.ChangeBalanceText(balance);
    }
    public void ChangeBalance(float amount)
    {
        balance = Math.Max(balance + Convert.ToDecimal(amount), 0);
        UIManager.instance.ChangeBalanceText(balance);
    }
    public void ChangeBalance(decimal amount)
    {
        balance = Math.Max(balance + amount, 0);
        UIManager.instance.ChangeBalanceText(balance);
    }
}
