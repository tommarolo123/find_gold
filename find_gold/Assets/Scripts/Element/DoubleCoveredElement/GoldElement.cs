using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldElement : DoubleCoveredElement
{
    public GoldType goldType;
    public override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.Gold;
    }
    public override void OnUncovered()
    {
        Transform goldEffect = transform.Find("GoldEffect");
        if (goldEffect != null)
        {
            Destroy(goldEffect.gameObject);
        }
        //获得金币
        base.OnUncovered();
    }
    public override void ConfirmSprite()
    {
        Transform goldEffect = transform.Find("GoldEffect");
        if (goldEffect == null)
        {
            Instantiate(GameManager.Instance.goldEffect,transform).name = "GoldEffect";
        }
        LoadSprite(GameManager.Instance.goldSprites[(int)goldType]);
    }
}
