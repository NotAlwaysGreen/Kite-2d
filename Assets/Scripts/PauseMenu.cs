using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject Container;
     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Container.SetActive(true);
            Time.timeScale = 0;
        }

    }

    public void Resume()
    {
        Container.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void MainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;

    }
}
