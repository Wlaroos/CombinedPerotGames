using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private InputSystem_Actions _inputActions;

    [Header("Inactive")]
    [SerializeField] private GameObject inactivePanel; // panel that activates after inactiveTime gets triggered
    [SerializeField] private float inactiveTime; // time before warning
    [SerializeField] private float timeoutTime; // extra time before returning to menu

    private float idleTimer = 0f;
    private bool isIdle = false;

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
        if (_inputActions.UI.Quit.WasPressedThisFrame())
        {
            QuitGame();
        }

        if (_inputActions.UI.Restart.WasPressedThisFrame())
        {
            RestartGame();
        }

        Inactive();
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

    bool HasInput()
    {
        return Input.anyKeyDown || Input.GetMouseButtonDown(0);
    }

    private void Inactive()
    {
        if (HasInput())
        {
            idleTimer = 0f;
            return;
        }
        
        idleTimer += Time.deltaTime;

        if (!isIdle && idleTimer >= inactiveTime)
        {
            inactivePanel.SetActive(true);
            isIdle = true;
        }

        if (idleTimer >= inactiveTime + timeoutTime)
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public void InactivePress()
    {
        inactivePanel.SetActive(false);
        isIdle = false;
    }
}
