using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void PlayMineralMaker()
    {
        // Mineral Maker
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void PlayMineralOrganizer()
    {
        // Mixed-up Minerals 
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            Application.Quit();
#endif
        }
    }
}
