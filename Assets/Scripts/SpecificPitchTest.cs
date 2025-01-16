using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class SpecificPitchTest
{
    [SerializeField] private float pitchSpeed; // m/s
    [SerializeField] private float horizontalBreak; // m/s
    [SerializeField] private float verticalBreak; // m/s --- does not include gravity
    [SerializeField] private float horizontalJerk;
    [SerializeField] private float verticalJerk;

    // ADD MIN AND MAX RANGES FOR SPEED AND BREAK LATER

    private const float dragPct = 0.1f;
    private Vector3 gravity = new(0, -9.81f, 0);

    public (Vector3, float, Vector3, Vector3) GetPitch(Vector3 releasePoint, Vector3 targetLocation)
    {
        float distanceToPlate = Mathf.Abs(releasePoint.z - targetLocation.z);

        (float timeToPlate, Vector3 accel, Vector3 jerk) = GetInitialCalculations(distanceToPlate);

        // distance
        float dx = targetLocation.x - releasePoint.x;
        float dy = targetLocation.y - releasePoint.y;
        float dz = targetLocation.z - releasePoint.z;

        // time
        float t2 = timeToPlate * timeToPlate;
        float t3 = t2 * timeToPlate;

        // initial velocities
        float v0x = (dx - 0.5f * accel.x * t2 - (1f / 6f) * jerk.x * t3) / timeToPlate;
        float v0y = (dy - 0.5f * accel.y * t2 - (1f / 6f) * jerk.y * t3) / timeToPlate;
        float v0z = (dz - 0.5f * accel.z * t2 - (1f / 6f) * jerk.z * t3) / timeToPlate;

        Vector3 initialVelocity = new(v0x, v0y, v0z);

        return (initialVelocity, pitchSpeed, accel, jerk);
    }

    private (float, Vector3, Vector3) GetInitialCalculations(float distanceToPlate)
    {
        if (pitchSpeed == 0) Debug.LogError("Pitch Speed has not been set.");

        float timeToPlate = Mathf.Abs(distanceToPlate / pitchSpeed);
        float dragJerk = (dragPct * pitchSpeed) * 2f / (timeToPlate * timeToPlate);

        Vector3 accel = new(horizontalBreak, verticalBreak, 0);
        accel += gravity;
        Vector3 jerk = new(horizontalJerk, verticalJerk, dragJerk);

        return (timeToPlate, accel, jerk);
    }
}
