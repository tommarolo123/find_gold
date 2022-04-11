using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseElement : MonoBehaviour
{
    public int x;
    public int y;
    public ElementState elementState;
    public ElementType elementType;
    public ElementContent elementContent;


    public virtual void Awake()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
        name = "(" + x + "," + y + ")";
    }

    public virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(2)&&elementState == ElementState.Uncovered)
        {
            OnMiddleMouseButton();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButton();
        } 
        else if (Input.GetMouseButtonUp(1)){
            OnRightMouseButton();
        }
    }

    public virtual void OnPlayerStand() { }

    public virtual void OnLeftMouseButton() { OnPlayerStand(); }

    public virtual void OnMiddleMouseButton() { }

    public virtual void OnRightMouseButton() { }


    public void LoadSprite(Sprite sprite)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;

    }
    public void ClearShadow()
    {
        Transform shadow = transform.Find("shadow");
        if(shadow != null)
        {
            Destroy(shadow.gameObject);
        }
    }

    public void ToNumberElement(bool needEffect)
    {
        GameManager.Instance.mapArray[x,y] = gameObject.AddComponent<NumberElement>();
        ((NumberElement)GameManager.Instance.mapArray[x, y]).needEffect = needEffect;
        ((NumberElement)GameManager.Instance.mapArray[x, y]).UncoveredElement();
        Destroy(this);
    }
}
