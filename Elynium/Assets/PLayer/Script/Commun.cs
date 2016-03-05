using UnityEngine;
using System.Collections;

public class Commun : MonoBehaviour
{
    public static bool FloatToBool(float value)
    {
        return value >= 0f;
    }

    public static float Unsigned(float value)
    {
        if (value < 0f)
            value *= -1f;
        return value;
    }

    public static bool ShiftKeyDown()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }
}
