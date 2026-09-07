using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;
    [SerializeField] private GameObject menuPanel;

    private void OnEnable()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            Application.Quit();
#endif
#if UNITY_WEBGL
            menuPanel.SetActive(true);
#endif
        }

        if (_inputActions.UI.Quit.WasPressedThisFrame())
        {
            QuitGame();
        }

        if (_inputActions.UI.Restart.WasPressedThisFrame())
        {
            RestartGame();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
