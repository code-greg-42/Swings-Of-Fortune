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

    private Vector3 velocity;
    private Vector3 targetPosition = new(0, 1, 0);

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.linearVelocity = new Vector3(-2.5f, 2.455f, distanceToPlate / timeToPlate);

        Vector3 computedVelocity = ComputeInitialVelocity(new Vector3(-1, 2.2f, 17), new Vector3(0, 1, 0), new Vector3(10f, -9.81f, 0), 0.5f);
        Debug.Log("Initial Computed Velocity: " + computedVelocity);

        velocity = computedVelocity; // -2.5, 2.455f, -34 for 0,0,0 from 0,0,17

        
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

            velocity += hMovement * Time.deltaTime;
            velocity += gravity * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;

            if (pitchTime >= timeToPlate)
            {
                Debug.Log("Pitch Location Calculated At: " + pitchTime);
                Debug.Log("Pitch Location: " + transform.position);
            }
        }

        if (!pitchLogged && transform.position.z < -3)
        {
            pitchLogged = true;
            //rb.linearVelocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
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
}
