using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(FirstPersonController))]
public abstract class GravityChangingAction : MonoBehaviour {

    public float effectGravityMultiplier;
    public float fadeDuration;
    public float fullStrengthDuration;

    public State state { get; set; }

    public enum State { Stopped, Running }

    private FirstPersonController fpsController;

    private float normalGravityMultiplier;
    private DateTime startTime;

    void Start() {
        fpsController = GetComponent<FirstPersonController>();
        normalGravityMultiplier = fpsController.gravityMultiplier;
        state = State.Stopped;
    }

    public void FixedUpdate() {
        if (state == State.Running) {
            DateTime now = DateTime.Now;
            float deltaSeconds = (float) (now - startTime).TotalSeconds;
            if (deltaSeconds < fadeDuration) {
                fpsController.gravityMultiplier = Mathf.Lerp(normalGravityMultiplier, effectGravityMultiplier, deltaSeconds / fadeDuration);
            } else if (deltaSeconds > 2 * fadeDuration + fullStrengthDuration) {
                fpsController.gravityMultiplier = normalGravityMultiplier;
                state = State.Stopped;
                Debug.Log("Antigravity stopped");
            } else if (deltaSeconds > fadeDuration + fullStrengthDuration) {
                deltaSeconds -= fadeDuration + fullStrengthDuration;
                fpsController.gravityMultiplier = Mathf.Lerp(effectGravityMultiplier, normalGravityMultiplier, deltaSeconds / fadeDuration);
            } else {
                fpsController.gravityMultiplier = effectGravityMultiplier;
            }
        }
    }

    public void Run() {
        if (state == State.Stopped) {
            Debug.Log("Antigravity fired");
            startTime = DateTime.Now;
            state = State.Running;
        }
    }
}
