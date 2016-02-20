using UnityEngine;
using System.Collections;

public class GameModeController : MonoBehaviour
{
    RaycastHit hit;


    public static GameObject currentlySelectedUnit;
    public static ArrayList currentlySelectedUnits = new ArrayList();

    public static bool userIsDragging;
    public GameObject target;
    public GUIStyle mouseDragSkin;


    private static Vector3 mouseDownPoint_;
    private static Vector3 mouseUpPoint_;
    private static Vector3 currentMousePoint;
    private static Vector2 mouseDragStart_;
    private static float timeLimiteBeforeDeclareDrag_ = 1f;
    private static float timLeftBeforeDeclareDrag_;
    private static float clickDragZone_ = 1.3f;

    private float boxWidth_;
    private float boxHeight_;
    private float boxTop_;
    private float boxLeft_;
    public Vector2 boxStart_;
    public Vector2 boxFinish_;

    void Awake()
    {
        mouseDownPoint_ = Vector3.zero;
    }

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
                timLeftBeforeDeclareDrag_ = 0f;
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
                    }
                    else if ((Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint_)) && !ShiftKeyDown())
                    {
                        DeselectedGameObjectIfSelected();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint_))
                    {
                        if (hit.collider.transform.FindChild("Selected"))
                        {
                            if (!UnitAlReadyInCurrentlySelectedUnits(hit.collider.gameObject))
                            {
                                if (!ShiftKeyDown())
                                {
                                    DeselectedGameObjectIfSelected();
                                }

                                GameObject selectedObj = hit.collider.transform.FindChild("Selected").gameObject;
                                selectedObj.SetActive(true);

                                currentlySelectedUnits.Add(hit.collider.gameObject);
                            }
                            else
                            {
                                if (ShiftKeyDown())
                                {
                                    RemoveUnitFrontCurrntlySelectedUnits(hit.collider.gameObject);
                                }
                                else
                                {
                                    DeselectedGameObjectIfSelected();

                                    GameObject selectedObj = hit.collider.transform.FindChild("Selected").gameObject;
                                    selectedObj.SetActive(true);

                                    currentlySelectedUnits.Add(hit.collider.gameObject);
                                }
                            }
                        }
                        else
                        {
                            if (!ShiftKeyDown())
                            {
                                DeselectedGameObjectIfSelected();
                            }
                        }
                    }
                }// end big else
            } // end userIsDragging
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint_))
            {
                if (!ShiftKeyDown())
                {
                    DeselectedGameObjectIfSelected();
                }
            }

        }

        Debug.DrawRay(ray.origin, ray.direction * 500, Color.yellow);

        if(userIsDragging)
        {
            boxWidth_ = Camera.main.WorldToScreenPoint(mouseDownPoint_).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            boxHeight_ = Camera.main.WorldToScreenPoint(mouseDownPoint_).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;
            boxLeft_ = Input.mousePosition.x;
            boxTop_ = Screen.height - Input.mousePosition.y - boxHeight_;

            if(FloatToBool(boxWidth_))
            {
                if (FloatToBool(boxHeight_))
                {
                    boxStart_ = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight_);
                }
                else
                {
                    boxStart_ = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            }
            else if (!FloatToBool(boxWidth_))
            {
                if(FloatToBool(boxHeight_))
                {
                    boxStart_ = new Vector2(Input.mousePosition.x + boxWidth_, Input.mousePosition.y + boxHeight_);
                }
                else
                {
                    boxStart_ = new Vector2(Input.mousePosition.x + boxWidth_, Input.mousePosition.y);
                }
            }
        }

        boxFinish_ = new Vector2(boxStart_.x + Unsigned(boxWidth_), boxStart_.y - Unsigned(boxHeight_));
    }

    void OnGUI()
    {
        if(userIsDragging)
        {
            GUI.Box(new Rect(boxLeft_, boxTop_, boxWidth_, boxHeight_), "", mouseDragSkin);
        }
    }

    #region Function

    public static bool FloatToBool (float value)
    {
        return value >= 0f;
    }

    public static float Unsigned(float value)
    {
        if (value < 0f)
            value *= -1f;
        return value;
    }

    public bool UserDraggingByPosition(Vector2 _dragStartPoint, Vector2 _newPoint)
    {
        return ((_newPoint.x > _dragStartPoint.x + clickDragZone_ || _newPoint.x < _dragStartPoint.x - clickDragZone_) ||
                (_newPoint.y > _dragStartPoint.y + clickDragZone_ || _newPoint.y < _dragStartPoint.y - clickDragZone_));
    }

    public bool DidUserClickLeftMouse(Vector3 _hitPoint)
    {
        return ((mouseDownPoint_.x < _hitPoint.x + clickDragZone_ && mouseDownPoint_.x > _hitPoint.x - clickDragZone_) &&
                (mouseDownPoint_.y < _hitPoint.y + clickDragZone_ && mouseDownPoint_.y > _hitPoint.y - clickDragZone_) &&
                (mouseDownPoint_.z < _hitPoint.z + clickDragZone_ && mouseDownPoint_.z > _hitPoint.z - clickDragZone_));
    }

    public static void DeselectedGameObjectIfSelected()
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for(int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
                arrayListUnit.transform.FindChild("Selected").gameObject.SetActive(false);
            }
            currentlySelectedUnits.Clear();
        }
    }

    public static bool UnitAlReadyInCurrentlySelectedUnits(GameObject _Unit)
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
                if (arrayListUnit == _Unit)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public void RemoveUnitFrontCurrntlySelectedUnits(GameObject _Unit)
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject arrayListUnit = currentlySelectedUnits[i] as GameObject;
                if (arrayListUnit == _Unit)
                {
                    currentlySelectedUnits.RemoveAt(i);
                    arrayListUnit.transform.FindChild("Selected").gameObject.SetActive(false);
                }
            }
        }
    }

    public static bool ShiftKeyDown()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }
    #endregion
}
