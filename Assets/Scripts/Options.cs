using UnityEngine;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{


    //restart current scene
    public void restartScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
