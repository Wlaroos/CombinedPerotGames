using UnityEngine;
using UnityEngine.SceneManagement;

public class OrganizerGameManager : MonoBehaviour
{
    public static OrganizerGameManager Instance;

    [Header("Win UI")]
    public GameObject winPanel;
    
    public DisplayOrderType displayOrder;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
    
    public void ShowWin()
    {
        winPanel.SetActive(true);
        //Time.timeScale = 0f; // pause game
    }
    
    public void SubmitCheck()
    {
        if (displayOrder.win)
            ShowWin();
        else
            Debug.Log("Not organized correctly yet.");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
