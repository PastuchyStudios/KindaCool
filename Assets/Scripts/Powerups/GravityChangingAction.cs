using UnityEngine;
using System.Collections;

public abstract class GravityChangingAction : MonoBehaviour
{

    public Vector3 newGravity;
    public float fadeDuration;
    public float fullStrengthDuration;
    public float timeResolution = 0.1f;
    public State state = State.Stopped;

     public enum State { Stopped, Waiting, Running }

    void Update() {
        if (state == State.Waiting) {
            state = State.Running;
            StartCoroutine("PlayGravityChanges");
        }
    }

    public void Run() {
        if (state == State.Stopped) {
            state = State.Running;
        }
    }

    private IEnumerator PlayGravityChanges() {
        for (float t = 0.0f; t <= fadeDuration; t += timeResolution) {
            Physics.gravity = (newGravity * t + Vector3.down * (fadeDuration - t)) / fadeDuration;
            yield return new WaitForSeconds(timeResolution);
        }
        yield return new WaitForSeconds(fullStrengthDuration);
        for (float t = timeResolution; t >= 0.0f; t -= timeResolution) {
            Physics.gravity = (newGravity * t + Vector3.down * (fadeDuration - t)) / fadeDuration;
            yield return new WaitForSeconds(timeResolution);
        }
        state = State.Stopped;
    }
}
