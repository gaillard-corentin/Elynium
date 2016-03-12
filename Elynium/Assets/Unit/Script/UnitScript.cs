using UnityEngine;
using System.Collections;

public class UnitScript : MonoBehaviour
{
    public bool onScreen;
    public bool selected = false;
    public bool isWalkable = true;


    private Vector2 screenPos_;
    private GameObject dragSelected_;

    void Awake()
    {
        if(transform.FindChild("Selected") != null)
        {
            transform.FindChild("Selected").gameObject.SetActive(false);
        }

        if (transform.FindChild("CanSelected") != null)
        {
            transform.FindChild("CanSelected").gameObject.SetActive(false);
        }

        if (transform.FindChild("DragSelected") != null)
        {
            dragSelected_ = transform.FindChild("DragSelected").gameObject;
        }
    }

    void Update()
    {
        if(!selected)
        {
            screenPos_ = Camera.main.WorldToScreenPoint(this.transform.position);
        }

        if(GameModeController.UnitsWithinScreenSpace(screenPos_))
        {
            if (!onScreen)
            {
                GameModeController.unitsOnScreen.Add(this.gameObject);
                onScreen = true;
            }
        }
        else
        {
            if (onScreen)
            {
                GameModeController.RemoveFromOnScreenUnits(this.gameObject);
            }
        }
    }

    public Vector2 ScreenPossition
    {
        get
        {
            return screenPos_;
        }
    }
}
