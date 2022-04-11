using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElement : CantCoveredElement
{
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Enemy;
        ClearShadow();
        LoadSprite(GameManager.Instance.enemySprites[Random.Range(0, GameManager.Instance.enemySprites.Length)]);
    }
}
