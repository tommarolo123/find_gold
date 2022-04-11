using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorElement : CantCoveredElement
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Door;
        LoadSprite(GameManager.Instance.doorSprite);
    }
}

