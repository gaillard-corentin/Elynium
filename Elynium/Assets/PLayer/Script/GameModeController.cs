using UnityEngine;
using System.Collections;

public class GameModeController : MonoBehaviour
{
    #region Variable
    RaycastHit hit;


    public static GameObject currentlySelectedUnit;
    public static ArrayList currentlySelectedUnits = new ArrayList();
    public static ArrayList unitsOnScreen = new ArrayList();
    public static ArrayList unitsInDrag = new ArrayList();
    public static bool userIsDragging;
    public static Vector2 boxStart;
    public static Vector2 boxFinish;
    public static Vector3 rightClickPoint;

    public GameObject target;
    public GUIStyle mouseDragSkin;


    private static Vector3 mouseDownPoint_;
    private static Vector3 currentMousePoint;
    private static Vector2 mouseDragStart_;
    private static float timeLimiteBeforeDeclareDrag_ = 1f;
    private static float timLeftBeforeDeclareDrag_;
    private static float clickDragZone_ = 1.3f;
    private static bool finishedDragOnThisFrame_;

    private float boxWidth_;
    private float boxHeight_;
    private float boxTop_;
    private float boxLeft_;
    #endregion

    void Update ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            currentMousePoint = hit.point;

            if(Input.GetMouseButtonDown(0))
            {
                mouseDownPoint_ = hit.point;
                timLeftBeforeDeclareDrag_ = timeLimiteBeforeDeclareDrag_;
                mouseDragStart_ = Input.mousePosition;
            }
            else if(Input.GetMouseButton(0))
            {
                if(!userIsDragging)
                {
                    timLeftBeforeDeclareDrag_ = Time.deltaTime;
                    userIsDragging = (timLeftBeforeDeclareDrag_ <= 0f || UserDraggingByPosition(mouseDragStart_, Input.mousePosition));
                }
            }
            else if(Input.GetMouseButtonUp(0))
            {
                if(userIsDragging)
                {
                    finishedDragOnThisFrame_ = true;
                }
                
                userIsDragging = false;
            }

            if(!userIsDragging)
            {
                if (hit.collider.name == "Terrain")
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        GameObject targetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
                        targetObj.name = "Target Instantiated";
                        rightClickPoint = hit.point;
                    }
                    else if ((Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint_)) && !Commun.ShiftKeyDown())
                    {
                        DeselectedGameObjectIfSelected();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint_))
                    {
                        if (hit.collider.gameObject.GetComponent<UnitScript>())
                        {
                            if (!UnitAlReadyInCurrentlySelectedUnits(hit.collider.gameObject))
                            {
                                if (!Commun.ShiftKeyDown())
                                {
                                    DeselectedGameObjectIfSelected();
                                }

                                GameObject selectedObj = hit.collider.transform.FindChild("Selected").gameObject;
                                selectedObj.SetActive(true);
                                hit.collider.GetComponent<UnitScript>().selected = true;

                                currentlySelectedUnits.Add(hit.collider.gameObject);
                            }
                            else
                            {
                                if (Commun.ShiftKeyDown())
                                {
                                    RemoveUnitFrontCurrentlySelectedUnits(hit.collider.gameObject);
                                }
                                else
                                {
                                    DeselectedGameObjectIfSelected();

                                    GameObject selectedObj = hit.collider.transform.FindChild("Selected").gameObject;
                                    selectedObj.SetActive(true);
                                    hit.collider.GetComponent<UnitScript>().selected = true;

                                    currentlySelectedUnits.Add(hit.collider.gameObject);
                                }
                            }
                        }
                        else
                        {
                            if (!Commun.ShiftKeyDown())
                            {
                                DeselectedGameObjectIfSelected();
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint_))
            {
                if (!Commun.ShiftKeyDown())
                {
                    DeselectedGameObjectIfSelected();
                }
            }

        }

        if(userIsDragging)
        {
            boxWidth_ = Camera.main.WorldToScreenPoint(mouseDownPoint_).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            boxHeight_ = Camera.main.WorldToScreenPoint(mouseDownPoint_).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;
            boxLeft_ = Input.mousePosition.x;
            boxTop_ = Screen.height - Input.mousePosition.y - boxHeight_;

            if(Commun.FloatToBool(boxWidth_))
            {
                if (Commun.FloatToBool(boxHeight_))
                {
                    boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight_);
                }
                else
                {
                    boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            }
            else if (!Commun.FloatToBool(boxWidth_))
            {
                if(Commun.FloatToBool(boxHeight_))
                {
                    boxStart = new Vector2(Input.mousePosition.x + boxWidth_, Input.mousePosition.y + boxHeight_);
                }
                else
                {
                    boxStart = new Vector2(Input.mousePosition.x + boxWidth_, Input.mousePosition.y);
                }
            }
        }

        boxFinish = new Vector2(boxStart.x + Commun.Unsigned(boxWidth_), boxStart.y - Commun.Unsigned(boxHeight_));
    }

    void LateUpdate()
    {
        unitsInDrag.Clear();

        if((userIsDragging || finishedDragOnThisFrame_) && unitsOnScreen.Count > 0)
        {
            for(int i = 0; i < unitsOnScreen.Count; i++)
            {
                GameObject unitObj = unitsOnScreen[i] as GameObject;
                UnitScript unitScript = unitObj.GetComponent<UnitScript>();
                GameObject selectedObj = unitObj.transform.FindChild("Selected").gameObject;

                if(!UnitAlreadyInDraggedUnits(unitObj))
                {
                    if(UnitInsideDrag(unitScript.ScreenPossition))
                    {
                        selectedObj.SetActive(true);
                        unitObj.GetComponent<UnitScript>().selected = true;
                        unitsInDrag.Add(unitObj);
                    }
                    else
                    {
                        if(!UnitAlReadyInCurrentlySelectedUnits(unitObj))
                        {
                            selectedObj.SetActive(false);
                            unitObj.GetComponent<UnitScript>().selected = false;
                        }
                    }
                }
            }
        }

        if(finishedDragOnThisFrame_)
        {
            finishedDragOnThisFrame_ = false;
            PutDraggedUnitsInCurrentlySelectedUnits();
        }
    }

    void OnGUI()
    {
        if(userIsDragging)
        {
            GUI.Box(new Rect(boxLeft_, boxTop_, boxWidth_, boxHeight_), "", mouseDragSkin);
        }
    }

    #region Function

    public bool UserDraggingByPosition(Vector2 _dragStartPoint, Vector2 _newPoint)
    {
        return ((_newPoint.x > _dragStartPoint.x + clickDragZone_ || _newPoint.x < _dragStartPoint.x - clickDragZone_) ||
                (_newPoint.y > _dragStartPoint.y + clickDragZone_ || _newPoint.y < _dragStartPoint.y - clickDragZone_));
    }//true

    public bool DidUserClickLeftMouse(Vector3 _hitPoint)
    {
        return ((mouseDownPoint_.x < _hitPoint.x + clickDragZone_ && mouseDownPoint_.x > _hitPoint.x - clickDragZone_) &&
                (mouseDownPoint_.y < _hitPoint.y + clickDragZone_ && mouseDownPoint_.y > _hitPoint.y - clickDragZone_) &&
                (mouseDownPoint_.z < _hitPoint.z + clickDragZone_ && mouseDownPoint_.z > _hitPoint.z - clickDragZone_));
    }//true

    public static void DeselectedGameObjectIfSelected()
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for(int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
                arrayListUnit.transform.FindChild("Selected").gameObject.SetActive(false);
                arrayListUnit.GetComponent<UnitScript>().selected = false;
            }
            currentlySelectedUnits.Clear();
        }
    }//true

    public static bool UnitAlReadyInCurrentlySelectedUnits(GameObject _unit)//true
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject unitObj = currentlySelectedUnits[i] as GameObject;
                if (unitObj == _unit)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void RemoveUnitFrontCurrentlySelectedUnits(GameObject _unit)
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
                if (arrayListUnit == _unit)
                {
                    currentlySelectedUnits.RemoveAt(i);
                    arrayListUnit.transform.FindChild("Selected").gameObject.SetActive(false);
                    arrayListUnit.GetComponent<UnitScript>().selected = false;
                }
            }
        }
    }//true

    public static bool UnitsWithinScreenSpace(Vector2 _unitScreenPos)
    {
        return ((_unitScreenPos.x < Screen.width && _unitScreenPos.y < Screen.height) && (_unitScreenPos.x > 0f && _unitScreenPos.y > 0f));
    }//true

    public static void RemoveFromOnScreenUnits(GameObject _unit)
    {
        for(int i = 0; i < unitsOnScreen.Count; i++)
        {
            GameObject unitObj = unitsOnScreen[i] as GameObject;
            if(_unit == unitObj)
            {
                unitsOnScreen.RemoveAt(i);
                unitObj.GetComponent<UnitScript>().onScreen = false;
                return;
            }
        }
        return;
    }//true

    public static bool UnitInsideDrag(Vector2 _unitScreenPos)
    {
        return ((_unitScreenPos.x > boxStart.x && _unitScreenPos.y < boxStart.y) &&
                (_unitScreenPos.x < boxFinish.x && _unitScreenPos.y > boxFinish.y));
    }//true

    public static bool UnitAlreadyInDraggedUnits(GameObject _unit)
    {
        if (unitsInDrag.Count > 0)
        {
            for (int i = 0; i < unitsInDrag.Count; i++)
            {
                GameObject unitObj = unitsInDrag[i] as GameObject;
                if (unitObj == _unit)
                {
                    return true;
                }
            }
        }
        return false;
    }//true

    public static void PutDraggedUnitsInCurrentlySelectedUnits()
    {
        if (!Commun.ShiftKeyDown())
        {
            DeselectedGameObjectIfSelected();
        }

        if (unitsInDrag.Count > 0)
        {
            for(int i = 0; i < unitsInDrag.Count; i++)
            {
                GameObject unitObj = unitsInDrag[i] as GameObject;

                if(!UnitAlReadyInCurrentlySelectedUnits(unitObj))
                {
                    currentlySelectedUnits.Add(unitObj);
                    unitObj.GetComponent<UnitScript>().selected = true;
                }
            }

            unitsInDrag.Clear();
        }
    }//true
    #endregion
}
