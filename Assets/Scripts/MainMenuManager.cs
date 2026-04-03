using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public void PlayMineralMaker()
    {
        // Load the level selection scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("v.05");
    }

        public void PlayMineralOrganizer()
    {
        // Load the level selection scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("OrganizerGame");
    }
}
