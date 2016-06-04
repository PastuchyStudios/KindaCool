using UnityEngine;

public class ExitOnEscape : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && Cursor.visible) {
            Application.Quit();
        }
    }
}
