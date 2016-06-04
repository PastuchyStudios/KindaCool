using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartScreen : MonoBehaviour {
    
    public GameObject finalScoreText;
    
    public ScoreCounter scoreCounter;
   
    // public FirstPersonController firstPersonController;
   
    void Update() {
        if (Input.GetKeyUp(KeyCode.Return)) {
            restartLevel();
        }
        
        // Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }
    
    void OnEnable() {
        Time.timeScale = 0;
        scoreCounter.enabled = false;
        // firstPersonController.enabled = false;
        finalScoreText.GetComponent<Text>().text = string.Format("{0:0.00}.", scoreCounter.score);
        gameObject.SetActive(true);
    }
    
    void restartLevel() {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    
}
