using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleCoveredElement : SinleCoveredElement
{
    public bool isHide = true;
    public override void Awake()
    {
        base.Awake();
        elementType = ElementType.DoubleCovered;
        if(Random.value < GameManager.Instance.uncoProbability)
        {
            Debug.Log("yes");
            UnconveredElementSingle();
        }
        
    }

    public override void OnPlayerStand()
    {
        switch (elementState)
        {
            case ElementState.Covered:

               if(isHide == true)
                {
                    UnconveredElementSingle();
                }
                else
                {
                    UncoveredElement();
                }
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                if (isHide == true)
                {
                    RemoveFlag();
                }    
                break;
        }
    }

    public override void OnMiddleMouseButton()
    {
        GameManager.Instance.UncoveredAdjacentElement(x, y);
    }

    public override void OnRightMouseButton()
    {
        switch(elementState)
        {
            case ElementState.Covered:
                if (isHide == true)
                {
                    AddFlag();
                }
               
                break;

            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                if (isHide == true)
                {
                    RemoveFlag();
                }
                break;
        }
    }

    public override void UnconveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        isHide = false;
        RemoveFlag();
        ClearShadow();
        ConfirmSprite();

    }

    public override void OnUncovered()
    {
        elementState = ElementState.Uncovered;
        ToNumberElement(false);

    }

    public virtual void ConfirmSprite()
    {

    }
}