using UnityEngine;
using System.Collections;

public class CameraPlayer : MonoBehaviour
{
    public float moveSpeed;
    public Terrain terrain;
    public int lengthResolution;
    public int widthResolution;

    
    private int border = 5;


    int minX;
    int maxX;
    int minZ;
    int maxZ;
    float currentDistance_;

    void Start()
    {
        minX = (int)terrain.transform.position.x + widthResolution;
        maxX = (int)terrain.terrainData.size.x - widthResolution;

        minZ = (int)terrain.transform.position.z + 5;
        maxZ = (int)terrain.terrainData.size.z - lengthResolution;
    }

    void Update()
    {
        var mouseX = Input.mousePosition.x;
        var mouseY = Input.mousePosition.y;

        var previousPossition = Vector3.zero;
        var currentPossition = Vector3.zero;

        previousPossition = transform.position;

        if ((mouseX < border || Input.GetKey(KeyCode.LeftArrow)) && !GameModeController.userIsDragging)
        {
            transform.Translate(Vector3.right * -moveSpeed * Time.deltaTime);
        }
        if ((mouseX >= Screen.width - border || Input.GetKey(KeyCode.RightArrow)) && !GameModeController.userIsDragging)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        if ((mouseY < border || Input.GetKey(KeyCode.DownArrow)) && !GameModeController.userIsDragging)
        {
            transform.Translate(Vector3.forward * -moveSpeed * Time.deltaTime);
        }
        if ((mouseY >= Screen.height - border || Input.GetKey(KeyCode.UpArrow)) && !GameModeController.userIsDragging)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, 100))
        {
            currentDistance_ = Vector3.Distance(transform.position, hit.point);
        }

        if(currentDistance_ != 35)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, 35 - currentDistance_, 0), Time.deltaTime);
        }

        currentPossition = transform.position;
        if ((currentPossition.z < minZ || currentPossition.z > maxZ) ||
            (currentPossition.x < minX || currentPossition.x > maxX) ||
            (currentPossition.y < 35))
        {
            transform.position = previousPossition;
        }
    }
}
