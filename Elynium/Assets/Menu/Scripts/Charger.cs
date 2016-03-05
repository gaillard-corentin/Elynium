using UnityEngine;
using System.Collections;

public class Charger : MonoBehaviour {

	public void Charger_scene()
    {
        //Application.LoadLevel(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}
