using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolElement : DoubleCoveredElement
{
    public ToolType toolType;

    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Tool;
    }

    public override void OnUncovered()
    {
        // TODO 获得道具
        Debug.Log("get tools");
        base.OnUncovered();
    }

    public override void ConfirmSprite()
    {
        LoadSprite(GameManager.Instance.toolSprites[(int)toolType]);
    }

}