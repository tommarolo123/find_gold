using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberElement : SinleCoveredElement
{
    public bool needEffect = true;
    public override void Awake()
    {
        base.Awake();
        elementState = ElementState.Covered;
        elementContent = ElementContent.Number;
        
    }
    public override void OnMiddleMouseButton()
    {
        GameManager.Instance.UncoveredAdjacentElement(x, y);
    }
    public override void UnconveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        RemoveFlag();
        elementState = ElementState.Uncovered; 
        ClearShadow();
        if(needEffect == true)
        {
            Instantiate(GameManager.Instance.UncoverEffect, transform);
        }
        LoadSprite(GameManager.Instance.numberSprites[GameManager.Instance.CountAdjacentTraps(x, y)]);
        Debug.Log(GameManager.Instance.CountAdjacentTraps(x, y));
    }
    public override void OnUncovered()
    {
        GameManager.Instance.FloodFillElement(x, y, new bool[GameManager.Instance.w, GameManager.Instance.h]);
    }
}
