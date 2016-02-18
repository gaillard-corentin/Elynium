using UnityEngine;
using System.Collections;

public class CameraMiniMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public float moveSpeedx;
    public float moveSpeedy;
    public double minX, maxX, minY, maxY;

    private int border = 5;
    // Update is called once per frame
    void Update ()
    {
        maxX = (Screen.width / 8.89);
        maxY = (Screen.height / 4.60);
        minX = (Screen.width- Screen.width*0.9775);
        minY = (Screen.height- Screen.height*0.9475);
        moveSpeedx = (float)(Screen.width / 35.56);
        moveSpeedy = (float)(Screen.height / 18.4);

        var mouseX = Input.mousePosition.x;
        var mouseY = Input.mousePosition.y;

        var previousPossition = Vector3.zero;
        var currentPossition = Vector3.zero;

        previousPossition = transform.position;

        if (mouseX < border)
        {
            transform.Translate(Vector3.right * -moveSpeedx * Time.deltaTime);
        }
        if (mouseX >= Screen.width - border)
        {
            transform.Translate(Vector3.right * moveSpeedx * Time.deltaTime);
        }
        if (mouseY < border)
        {
            transform.Translate(Vector3.up * -moveSpeedy * Time.deltaTime);
        }
        if (mouseY >= Screen.height - border)
        {
            transform.Translate(Vector3.up * moveSpeedy * Time.deltaTime);
        }

        currentPossition = transform.position;

        if (currentPossition.y < minY || currentPossition.y > maxY)
        {
            transform.position = previousPossition;
        }
        if (currentPossition.x < minX || currentPossition.x > maxX)
        {
            transform.position = previousPossition;
        }
    }
}
