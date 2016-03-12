using UnityEngine;
using System.Collections;

public class GameModeController : MonoBehaviour
{
    #region Variable
    RaycastHit hit;


    public static GameObject canSelectedUnit;
    public static ArrayList currentlySelectedUnits = new ArrayList();
    public static ArrayList unitsOnScreen = new ArrayList();
    public static ArrayList unitsInDrag = new ArrayList();
    public static bool userIsDragging;
    public static Vector2 boxStart;
    public static Vector2 boxFinish;
    public static Vector3 rightClickPoint;

    public GameObject target;
    public GUIStyle mouseDragSkin;
    public GUIStyle mouseDragSkinBorder;
    public LayerMask selectMeshLayerask;


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
                if (hit.collider.gameObject.GetComponent<UnitScript>())
                {
                    GameObject canSelectedObj = hit.collider.transform.FindChild("CanSelected").gameObject;
                    canSelectedObj.SetActive(true);
                    canSelectedUnit = hit.collider.gameObject;
                }
                else if(hit.collider.gameObject.layer == LayerMask.NameToLayer("SelectMesh"))
                {
                    GameObject canSelectedObj = hit.collider.transform.parent.FindChild("CanSelected").gameObject;
                    canSelectedObj.SetActive(true);
                    canSelectedUnit = hit.collider.transform.parent.gameObject;
                }
                else if(canSelectedUnit != null)
                {
                    canSelectedUnit.transform.FindChild("CanSelected").gameObject.SetActive(false);
                }

                if (hit.collider.name == "Terrain")
                {
                    //quand on fait un clic droit de la souris, creation d'un target
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
                        if (hit.collider.gameObject.GetComponent<UnitScript>() || hit.collider.gameObject.layer == LayerMask.NameToLayer("SelectMesh"))
                        {
                            Transform uniGameOject;
                            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("SelectMesh"))
                            {
                                uniGameOject = hit.collider.transform.parent.transform;
                            }
                            else
                            {
                                uniGameOject = hit.collider.transform;
                            }

                            if (!UnitAlReadyInCurrentlySelectedUnits(uniGameOject.gameObject))
                            {
                                if (!Commun.ShiftKeyDown())
                                {
                                    DeselectedGameObjectIfSelected();
                                }

                                GameObject selectedObj = uniGameOject.transform.FindChild("Selected").gameObject;
                                selectedObj.SetActive(true);
                                uniGameOject.GetComponent<UnitScript>().selected = true;

                                currentlySelectedUnits.Add(uniGameOject.gameObject);
                            }
                            else
                            {
                                if (Commun.ShiftKeyDown())
                                {
                                    RemoveUnitFrontCurrentlySelectedUnits(uniGameOject.gameObject);
                                }
                                else
                                {
                                    DeselectedGameObjectIfSelected();

                                    GameObject selectedObj = uniGameOject.transform.FindChild("Selected").gameObject;
                                    selectedObj.SetActive(true);
                                    uniGameOject.GetComponent<UnitScript>().selected = true;

                                    currentlySelectedUnits.Add(uniGameOject.gameObject);
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
        if (userIsDragging)
        {
            float boxHeight_y = 0;
            float boxWidth_x = 0;
            float x = 0;
            float y = 0;

            if (boxHeight_ < 0)
            {
                boxHeight_y = boxHeight_ + boxTop_ + 2;
                y = -2;
            }
            else
            {
                boxHeight_y = boxHeight_ + boxTop_ - 2;
                y = 2;
            }

            if (boxWidth_ < 0)
            {
                boxWidth_x = boxWidth_ + boxLeft_ + 2;
                x = -2;
            }
            else
            {
                boxWidth_x = boxWidth_ + boxLeft_ - 2;
                x = 2;
            }

            GUI.Box(new Rect(boxLeft_, boxTop_, boxWidth_, boxHeight_), "", mouseDragSkin);

            GUI.Box(new Rect(boxLeft_, boxTop_, boxWidth_, y), "", mouseDragSkinBorder);
            GUI.Box(new Rect(boxLeft_, boxTop_, x, boxHeight_), "", mouseDragSkinBorder);
            GUI.Box(new Rect(boxLeft_, boxHeight_y, boxWidth_, y), "", mouseDragSkinBorder);
            GUI.Box(new Rect(boxWidth_x, boxTop_, x, boxHeight_), "", mouseDragSkinBorder);
        }
    }

    #region Function

    public bool UserDraggingByPosition(Vector2 _dragStartPoint, Vector2 _newPoint)
    {
        return ((_newPoint.x > _dragStartPoint.x + clickDragZone_ || _newPoint.x < _dragStartPoint.x - clickDragZone_) ||
                (_newPoint.y > _dragStartPoint.y + clickDragZone_ || _newPoint.y < _dragStartPoint.y - clickDragZone_));
    }

    //verifie si l'utilisateur a cliqué sur la souris
    public bool DidUserClickLeftMouse(Vector3 _hitPoint)
    {
        return ((mouseDownPoint_.x < _hitPoint.x + clickDragZone_ && mouseDownPoint_.x > _hitPoint.x - clickDragZone_) &&
                (mouseDownPoint_.y < _hitPoint.y + clickDragZone_ && mouseDownPoint_.y > _hitPoint.y - clickDragZone_) &&
                (mouseDownPoint_.z < _hitPoint.z + clickDragZone_ && mouseDownPoint_.z > _hitPoint.z - clickDragZone_));
    }

    //deselectionne l'objet selectionné
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
    }

    public static bool UnitAlReadyInCurrentlySelectedUnits(GameObject _unit)
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
    }

    public static bool UnitsWithinScreenSpace(Vector2 _unitScreenPos)
    {
        return ((_unitScreenPos.x < Screen.width && _unitScreenPos.y < Screen.height) && (_unitScreenPos.x > 0f && _unitScreenPos.y > 0f));
    }

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
    }

    public static bool UnitInsideDrag(Vector2 _unitScreenPos)
    {
        return ((_unitScreenPos.x > boxStart.x && _unitScreenPos.y < boxStart.y) &&
                (_unitScreenPos.x < boxFinish.x && _unitScreenPos.y > boxFinish.y));
    }

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
    }

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
    }
    #endregion
}
