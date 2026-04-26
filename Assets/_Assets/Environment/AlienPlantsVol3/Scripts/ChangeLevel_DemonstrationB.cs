using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeLevel_DemonstrationB : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown (KeyCode.L))
        {
            SceneManager.LoadScene("Scene_Demonstration_B");
        }
    }
}