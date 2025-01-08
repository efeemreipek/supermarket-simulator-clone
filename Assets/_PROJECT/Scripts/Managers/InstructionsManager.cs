using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum EInstructionType
{
    None,
    PickBox,
    PutBoxInDump,
    PutBoxOnGround,
    PlaceProductOnShelf,
    PutProductInBox,
    TakeCreditCard,
    OpenComputer,
    CheckProduct,
    OpenCloseSign,
    ChangePriceShelf,
    ChangeMarketName
}

public class InstructionsManager : MonoBehaviour
{
    [SerializeField] private Transform instructionsContainerTransform;
    [SerializeField] private GameObject instructionTextPrefab;

    private ObjectInteractor interactor;
    private ObjectSelector selector;

    private Dictionary<EInstructionType, string> instructionTextDictionary = new Dictionary<EInstructionType, string>()
    {
        {EInstructionType.None, "None"},
        {EInstructionType.PickBox, "[LMB]-Pick up box"},
        {EInstructionType.PutBoxInDump, "[RMB]-Put box in dumpster"},
        {EInstructionType.PutBoxOnGround, "[RMB]-Put box on ground"},
        {EInstructionType.PlaceProductOnShelf, "[LMB]-Place product on shelf"},
        {EInstructionType.PutProductInBox, "[RMB]-Put product back in box"},
        {EInstructionType.TakeCreditCard, "[LMB]-Take credit card"},
        {EInstructionType.OpenComputer, "[LMB]-Open computer"},
        {EInstructionType.CheckProduct, "[LMB]-Check product from register"},
        {EInstructionType.OpenCloseSign, "[LMB]-Open/Close Market"},
        {EInstructionType.ChangePriceShelf, "[E]-Change shelf price"},
        {EInstructionType.ChangeMarketName, "[LMB]-Change market name"},
    };

    private void Awake()
    {
        interactor = GameObject.FindGameObjectWithTag("Player").GetComponent<ObjectInteractor>();
        selector = GameObject.FindGameObjectWithTag("Player").GetComponent<ObjectSelector>();
    }
    private void OnEnable()
    {
        selector.OnSelectableChanged += UpdateInstructions;
    }
    private void OnDisable()
    {
        selector.OnSelectableChanged -= UpdateInstructions;
    }

    private void UpdateInstructions(ISelectable newSelectable)
    {
        // clear previous instructions
        for(int i = instructionsContainerTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(instructionsContainerTransform.GetChild(i).gameObject);
        }

        if(newSelectable == null)
        {
            if(interactor.IsHoldingBox)
            {
                AddInstruction(EInstructionType.PutBoxOnGround);
            }
            return;
        }
        if(newSelectable.GameObject.TryGetComponent(out Box box))
        {
            if(interactor.IsHoldingBox) return;

            AddInstruction(EInstructionType.PickBox);
        }
        if(newSelectable.GameObject.TryGetComponent(out Shelf shelf))
        {
            if(interactor.IsHoldingBox)
            {
                AddInstruction(EInstructionType.PlaceProductOnShelf);
            }
            if(shelf.HasProduct)
            {
                AddInstruction(EInstructionType.ChangePriceShelf);
            }
        }
        if(newSelectable.GameObject.TryGetComponent(out BaseProduct product))
        {
            if(interactor.IsHoldingBox)
            {
                AddInstruction(EInstructionType.PutProductInBox);
            }
            if(product.IsAtRegister)
            {
                AddInstruction(EInstructionType.CheckProduct);
            }
        }
        if(newSelectable.GameObject.TryGetComponent(out CreditCard card))
        {
            if(interactor.IsHoldingBox) return;

            AddInstruction(EInstructionType.TakeCreditCard);
        }
        if(newSelectable.GameObject.TryGetComponent(out Computer computer))
        {
            if(interactor.IsHoldingBox) return;

            AddInstruction(EInstructionType.OpenComputer);
        }
        if(newSelectable.GameObject.TryGetComponent(out Dumpster dumpster))
        {
            if(interactor.IsHoldingBox)
            {
                AddInstruction(EInstructionType.PutBoxInDump);
            }
        }
        if(newSelectable.GameObject.TryGetComponent(out OpenCloseSign openCloseSign))
        {
            if(interactor.IsHoldingBox) return;

            AddInstruction(EInstructionType.OpenCloseSign);
        }
        if(newSelectable.GameObject.TryGetComponent(out MarketName marketName))
        {
            if(interactor.IsHoldingBox) return;

            AddInstruction(EInstructionType.ChangeMarketName);
        }
    }

    private void AddInstruction(EInstructionType instruction)
    {
        GameObject instructionTextGO = Instantiate(instructionTextPrefab, instructionsContainerTransform);
        instructionTextGO.GetComponent<InstructionTextUI>().InitializeInstructionText(instructionTextDictionary[instruction]);
    }
}
