using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SinleCoveredElement : BaseElement
{
    public override void Awake()
    {
        base.Awake();
        elementType = ElementType.SingleCovered;
        elementState = ElementState.Covered;
        LoadSprite(GameManager.Instance.coverTilesSprites[Random.Range(0, GameManager.Instance.coverTilesSprites.Length)]);
        
    }

    public override void OnPlayerStand()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                UncoveredElement();
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                RemoveFlag();
                break;
        }
    }

    public override void OnRightMouseButton()
    {
        switch (elementState)
        {
            case ElementState.Covered:
                AddFlag();
                break;
            case ElementState.Uncovered:
                return;
            case ElementState.Marked:
                RemoveFlag();
                break;
        }
    }

    public virtual void UncoveredElement()
    {
        if (elementState == ElementState.Uncovered) return;
        UnconveredElementSingle();
        OnUncovered();
    }

    public virtual void UnconveredElementSingle()
    {

    }

    public virtual void OnUncovered()
    {

    }

    public void AddFlag()
    {
        elementState = ElementState.Marked;
        GameObject flag = Instantiate(GameManager.Instance.flagElement, transform);
        flag.name = "flagElement";
        flag.transform.DOLocalMoveY(0f, 0.1f);
        Instantiate(GameManager.Instance.smokeEffect,transform);
    }

    public void RemoveFlag()
    {
        Transform flag = transform.Find("flagElement");
        if (flag !=  null)
        {
            elementState = ElementState.Covered;

        }
        elementState = ElementState.Covered;
        flag.DOLocalMoveY(0.15f, 0.1f).onComplete += () =>
        {
            Destroy(flag.gameObject);
        };
    }

}

