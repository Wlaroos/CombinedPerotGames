using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InactiveCanvas : MonoBehaviour
{
    [Header("Inactive Settings")]
    [SerializeField] private float _inactiveTime; // Time before warning
    [SerializeField] private float _timeoutTime; // Extra time before returning to menu
    [Header("Refs")]
    [SerializeField] private GameObject _inactivePanel; // Panel that activates after inactiveTime gets triggered
    [SerializeField] private Slider _timerBar; // Timer bar that fills up as time goes on
    private float _idleTimer = 0f;
    private bool _isIdle = false;

    private void Update()
    {
#if !UNITY_EDITOR
        Inactive();
#endif
    }

    bool HasInput()
    {
        return Input.anyKeyDown || Input.GetMouseButtonDown(0);
    }

    private void Inactive()
    {
        // If the player has any input, reset the idle timer and hide the inactive panel
        if (HasInput())
        {
            InactivePress();
        }
        
        _idleTimer += Time.deltaTime;

        if (_idleTimer < _inactiveTime)
        {
            // Timer bar stays full during the initial inactive time
            _timerBar.value = 1;
        }
        else if (_idleTimer < _inactiveTime + _timeoutTime)
        {
            // Timer bar decreases from 1 to 0 during the timeout time
            float elapsedTimeout = _idleTimer - _inactiveTime;
            _timerBar.value = 1 - (elapsedTimeout / _timeoutTime);
        }
        else
        {
            // Timer bar is empty when timeout is complete
            // Return to the main menu
            _timerBar.value = 0;
            SceneManager.LoadScene(0);
        }

        // Activate the inactive panel if the player has been idle for the inactive time
        if (!_isIdle && _idleTimer >= _inactiveTime)
        {
            _inactivePanel.SetActive(true);
            _isIdle = true;
        }
    }

    public void InactivePress()
    {
        _idleTimer = 0f;
        _timerBar.value = 1; // Reset the timer bar to full
        _inactivePanel.SetActive(false);
        _isIdle = false;
    }
}
