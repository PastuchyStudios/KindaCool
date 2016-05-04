using UnityEngine;
using System.Collections;

public class AppliedForce {
    public Vector3 point { get; set; }
    public float force { get; set; }

    public AppliedForce(Vector3 point, float force) {
        this.point = point;
        this.force = force;
    }

    public override string ToString() {
        return point + " " + force;
    }
}
