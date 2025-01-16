using UnityEngine;

public class SimplerPitchBall : MonoBehaviour
{
    private float timeToPlate = 0.5f;
    private const float distanceToPlate = -17.0f;
    private Vector3 hMovement = new(10f, 0, 0);
    private Vector3 gravity = new(0, -9.81f, 0);

    private Rigidbody rb;
    private bool pitchLogged = false;
    private float pitchTime;
    private float dragPct = -0.1f;

    private Vector3 velocity;
    private Vector3 targetPosition = new(0, 1, 0);
    private float dragJerk;
    private Vector3 jerk;

    private Vector3 acceleration;

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.linearVelocity = new Vector3(-2.5f, 2.455f, distanceToPlate / timeToPlate);

        Vector3 computedVelocity = ComputeInitialVelocity(new Vector3(-1, 2.2f, 17), new Vector3(0, 1, 0), new Vector3(10f, -9.81f, 0), 0.5f);
        Debug.Log("Initial Computed Velocity: " + computedVelocity);

        dragJerk = (dragPct * computedVelocity.z) * 2f / (timeToPlate * timeToPlate);
        Debug.Log("Drag Jerk: " + dragJerk);

        jerk = new Vector3(0, 0, dragJerk);

        Vector3 computedVelocityWithDrag = SecondTestFunction(new Vector3(-1, 2.2f, 17), new Vector3(0, 1, 0),
            new Vector3(10f, -9.81f, 0), jerk, 0.5f);

        Debug.Log("Computed Velocity With Drag: " + computedVelocityWithDrag);

        velocity = computedVelocityWithDrag; // -2.5, 2.455f, -34 for 0,0,0 from 0,0,17

        acceleration = hMovement + gravity;
    }

    private void FixedUpdate()
    {
        //rb.linearVelocity += hMovement * Time.fixedDeltaTime;
        //rb.linearVelocity += gravity * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (!pitchLogged)
        {
            pitchTime += Time.deltaTime;

            acceleration += jerk * Time.deltaTime;

            //velocity += hMovement * Time.deltaTime;
            //velocity += gravity * Time.deltaTime;

            velocity += acceleration * Time.deltaTime;

            transform.position += velocity * Time.deltaTime;

            if (pitchTime >= timeToPlate)
            {
                Debug.Log("Pitch Location Calculated At: " + pitchTime);
                Debug.Log("Pitch Location: " + transform.position);
                Debug.Log("Pitch Velocity: " + velocity);
                pitchLogged = true;
            }
        }
    }

    private Vector3 ComputeInitialVelocity(Vector3 startPos, Vector3 endPos, Vector3 constantAccel, float timeToTarget)
    {
        float dx = endPos.x - startPos.x;
        float dy = endPos.y - startPos.y;
        float dz = endPos.z - startPos.z;

        float ax = constantAccel.x;
        float ay = constantAccel.y;
        float az = constantAccel.z;

        float t2 = timeToTarget * timeToTarget;

        float v0x = (dx - 0.5f * ax * t2) / timeToTarget;
        float v0y = (dy - 0.5f * ay * t2) / timeToTarget;
        float v0z = (dz - 0.5f * az * t2) / timeToTarget;

        return new Vector3(v0x, v0y, v0z);
    }

    private Vector3 TestFunction(Vector3 releasePoint, Vector3 targetPos, Vector3[] additionalAccel, float timeToTarget)
    {
        return Vector3.zero;
    }

    private Vector3 SecondTestFunction(
        Vector3 releasePoint,
        Vector3 targetPos,
        Vector3 constantAccel,
        Vector3 jerk,
        float timeToTarget)
    {
        float dx = targetPos.x - releasePoint.x;
        float dy = targetPos.y - releasePoint.y;
        float dz = targetPos.z - releasePoint.z;

        float ax = constantAccel.x;
        float ay = constantAccel.y;
        float az = constantAccel.z;

        float jx = jerk.x;
        float jy = jerk.y;
        float jz = jerk.z;

        float t = timeToTarget;
        float t2 = t * t;
        float t3 = t2 * t;

        float v0x = (dx - 0.5f * ax * t2 - (1f / 6f) * jx * t3) / t;
        float v0y = (dy - 0.5f * ay * t2 - (1f / 6f) * jy * t3) / t;
        float v0z = (dz - 0.5f * az * t2 - (1f / 6f) * jz * t3) / t;

        return new Vector3(v0x, v0y, v0z);
    }

}
