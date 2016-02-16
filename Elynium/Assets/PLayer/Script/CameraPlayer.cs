using UnityEngine;
using System.Collections;

public class CameraPlayer : MonoBehaviour
{
    public float moveSpeed;
    public int length, width;


    int minX = 75;
    int minZ = 5;

    private int border = 5;

    void Update()
    {
        int maxX = width - 75;
        int maxZ = length - 60;
        var mouseX = Input.mousePosition.x;
        var mouseY = Input.mousePosition.y;

        var previousPossition = Vector3.zero;
        var currentPossition = Vector3.zero;

        previousPossition = transform.position;

        if (mouseX < border)
        {
            transform.Translate(Vector3.right * -moveSpeed * Time.deltaTime);
        }
        if (mouseX >= Screen.width - border)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        if (mouseY < border)
        {
            transform.Translate(Vector3.forward * -moveSpeed * Time.deltaTime);
        }
        if (mouseY >= Screen.height - border)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }

        currentPossition = transform.position;

        if (currentPossition.z < minZ || currentPossition.z > maxZ)
        {
            transform.position = previousPossition;
        }
        if (currentPossition.x < minX || currentPossition.x > maxX)
        {
            transform.position = previousPossition;
        }
    }
}
