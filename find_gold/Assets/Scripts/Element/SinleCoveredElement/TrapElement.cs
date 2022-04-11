using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapElement : SinleCoveredElement
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Trap;
    }

    public override void UnconveredElementSingle()
    {
        if (elementState == ElementState.Uncovered) return;
        RemoveFlag();
        elementState = ElementState.Uncovered;
        ClearShadow();
        LoadSprite(GameManager.Instance.trapsSprites[Random.Range(0, GameManager.Instance.trapsSprites.Length)]);
        Instantiate(GameManager.Instance.UncoverEffect, transform);
    }
    public override void OnUncovered()
    {
        //trap位置を見る
        GameManager.Instance.DisplayAppTraps(); 
    }

}

