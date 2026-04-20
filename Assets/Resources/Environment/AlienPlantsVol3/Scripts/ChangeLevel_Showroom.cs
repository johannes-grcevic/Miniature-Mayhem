using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeLevel_Showroom : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown (KeyCode.L))
        {
            SceneManager.LoadScene("Scene_Showroom");
        }
    }
}