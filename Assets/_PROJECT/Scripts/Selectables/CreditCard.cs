using System;
using UnityEngine;

public class CreditCard : Selectable
{
    public event Action OnCreditCardTaken;

    private void OnEnable()
    {
        ObjectInteractor.OnCreditCardTaken += ObjectInteractor_OnCreditCardTaken;
    }
    private void OnDisable()
    {
        ObjectInteractor.OnCreditCardTaken -= ObjectInteractor_OnCreditCardTaken;
    }

    private void ObjectInteractor_OnCreditCardTaken() => OnCreditCardTaken?.Invoke();
}
