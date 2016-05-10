using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartScreen : MonoBehaviour {
    
    public GameObject finalScoreText;
    
    internal void enable(double finalScore) {
        Time.timeScale = 0;
        finalScoreText.GetComponent<Text>().text = string.Format("{0:0.0000}.", finalScore);
        gameObject.SetActive(true);
    }
    
    void restartButtonClicked() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Level");
    }
    
}
