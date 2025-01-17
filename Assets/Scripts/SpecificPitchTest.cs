using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class SpecificPitchTest
{
    [SerializeField] private float pitchSpeed; // m/s
    [SerializeField] private float horizontalBreak; // m/s
    [SerializeField] private float verticalBreak; // m/s --- does not include gravity
    [SerializeField] private float sharpBreakPct; // % of acceleration increase by end of pitch
    [SerializeField] private float customGravity = -9.81f;
    [SerializeField] private float customDragPct = -0.1f;
    // ADD MIN AND MAX RANGES FOR SPEED AND BREAK LATER

    public (Vector3, Vector3, float, float, Vector3, Vector3) GetPitch(Vector3 releasePoint, Vector3 targetLocation)
    {
        float distanceToPlate = Mathf.Abs(releasePoint.z - targetLocation.z);

        (float timeToPlate, Vector3 accel, Vector3 jerk) = GetInitialCalculations(distanceToPlate);

        // distance
        double dx = targetLocation.x - releasePoint.x;
        double dy = targetLocation.y - releasePoint.y;
        double dz = targetLocation.z - releasePoint.z;

        // time
        double t2 = timeToPlate * timeToPlate;
        double t3 = t2 * timeToPlate;

        // initial velocities
        double v0x = (dx - 0.5f * accel.x * t2 - (1f / 6f) * jerk.x * t3) / timeToPlate;
        double v0y = (dy - 0.5f * accel.y * t2 - (1f / 6f) * jerk.y * t3) / timeToPlate;
        double v0z = (dz - 0.5f * accel.z * t2 - (1f / 6f) * jerk.z * t3) / timeToPlate;

        Vector3 initialVelocity = new((float)v0x, (float)v0y, (float)v0z);
        Vector3 actualTarget = CalculateActualTrajectory(releasePoint, initialVelocity, accel, jerk, timeToPlate);
        return (actualTarget, initialVelocity, pitchSpeed, timeToPlate, accel, jerk);
    }

    private (float, Vector3, Vector3) GetInitialCalculations(float distanceToPlate)
    {
        if (pitchSpeed == 0) Debug.LogError("Pitch Speed has not been set.");

        // ensure negativity
        pitchSpeed = -Mathf.Abs(pitchSpeed);
        customGravity = -Mathf.Abs(customGravity);
        customDragPct = -Mathf.Abs(customDragPct);

        // ensure positive time to plate
        float timeToPlate = Mathf.Abs(distanceToPlate / pitchSpeed);
        float t2 = timeToPlate * timeToPlate;
        float t3 = t2 * timeToPlate;

        float dragJerk = (customDragPct * pitchSpeed) * 2f / (t2);
        float horizontalJerk = (sharpBreakPct * horizontalBreak) * 6f / t3;
        float verticalJerk = (sharpBreakPct * verticalBreak) * 6f / t3;

        Vector3 accel = new(horizontalBreak, verticalBreak, 0);
        Vector3 gravity = new(0, customGravity, 0);
        accel += gravity;
        Vector3 jerk = new(horizontalJerk, verticalJerk, dragJerk);

        return (timeToPlate, accel, jerk);
    }

    private Vector3 CalculateActualTrajectory(Vector3 releasePoint, Vector3 initialVelocity, Vector3 accel, Vector3 jerk, float timeToPlate)
    {
        Vector3 position = releasePoint;
        Vector3 velocity = initialVelocity;
        Vector3 acceleration = accel;

        float timeStep = Time.fixedDeltaTime;
        float currentTime = 0.0f;

        Vector3 previousPosition = position;

        while (currentTime <= timeToPlate)
        {
            acceleration += jerk * timeStep;
            velocity += acceleration * timeStep;
            position += velocity * timeStep;

            if (position.z == 0)
            {
                return position;
            }

            if (position.z <= 0 && previousPosition.z > 0)
            {
                float zDiff = previousPosition.z - position.z;
                float tFraction = previousPosition.z / zDiff;
                Vector3 crossingPoint = Vector3.Lerp(previousPosition, position, tFraction);

                Debug.Log($"Ball crossed Z=0 at: {crossingPoint}");
                return crossingPoint;
            }

            previousPosition = position;

            currentTime += timeStep;
        }

        Debug.Log("Z-0 was not crossed during simulation.");

        return position;
    }
}
