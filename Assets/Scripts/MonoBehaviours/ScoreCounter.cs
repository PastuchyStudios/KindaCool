using UnityEngine;

public class ScoreCounter : MonoBehaviour 
{
    internal double score;
    
    string additionalText = "Maximum depth: ";
    
    Rect rect;
    GUIStyle style = new GUIStyle();
    
    void Start() {
        style.normal.textColor = Color.white;
        int fontSize = style.fontSize;
        int textWidth = score.ToString().Length * fontSize - additionalText.Length * fontSize;
        rect = new Rect(Screen.width - textWidth, 0, textWidth, style.lineHeight);
        style.alignment = TextAnchor.UpperRight;
    }
    
    void OnGUI() {
        GUI.Label(rect, additionalText + string.Format("{0:0.0000}", score), style);
    }
    
    internal void updateScore() {
        float currentDepth = transform.position.y;
        if (currentDepth < score) {
            score = currentDepth;  
        }      
    }
    
}