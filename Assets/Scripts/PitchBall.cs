using UnityEngine;

public class PitchBall : MonoBehaviour
{
    private float initialVelocity = 38.0f;
    private float spinRate = 3000.0f;
    private Vector3 spinAxis = new Vector3(0f, 1f, 0).normalized;
    private float airDensity = 1.225f;
    private float crossSectionalArea = 0.0044f;
    private float mass = 0.145f;
    private float diameter = 0.074f;
    private Vector3 gravity = new(0, -9.81f, 0);
    private float spinParameter;
    private float magnusForceConstants;
    private float dragConstants;
    private Vector3 gravityForce;

    private Vector3 velocity;
    private Vector3 magnusAcceleration;

    private bool movePitch;
    private bool pitchConcluded;
    private float pitchTime;

    private void Start()
    {
        velocity = new Vector3(0, 0, -initialVelocity);
        gravityForce = 9.81f * mass * Vector3.down;
        dragConstants = 0.5f * airDensity * 0.3f * crossSectionalArea;

        float spinRateRadPerSec = spinRate * (2 * Mathf.PI / 60);
        spinParameter = (spinRateRadPerSec * diameter) / (2 * initialVelocity);
        float liftCoefficient = 1.6f * spinParameter;
        liftCoefficient = Mathf.Clamp(liftCoefficient, -0.5f, 0.5f);
        magnusForceConstants = 0.5f * airDensity * crossSectionalArea * liftCoefficient;

        float magnusForceMagnitude = magnusForceConstants * (velocity.magnitude * velocity.magnitude);
        Vector3 vDir = velocity.normalized;
        Vector3 magnusDirection = Vector3.Cross(vDir, spinAxis.normalized).normalized;
        Vector3 magnusForce = magnusForceMagnitude * magnusDirection;
        Vector3 magnusAccel = magnusForce / mass;
        Debug.Log("Magnus Accel: " + magnusAccel);

        Invoke(nameof(MovePitch), 1.0f);
    }


    private void FixedUpdate()
    {
        if (movePitch)
        {
            pitchTime += Time.deltaTime;

            // get current velocity
            float v = velocity.magnitude;
            Vector3 vDir = velocity.normalized;

            // drag force;
            float dragForceMagnitude = dragConstants * v * v;
            Vector3 dragForce = -dragForceMagnitude * vDir;

            Vector3 magnusForce = Vector3.zero;

            if (pitchTime > 0.15f && pitchTime < 0.3f || pitchTime > 0.4f)
            {
                // magnus force
                float magnusForceMagnitude = magnusForceConstants * (v * v);
                Vector3 magnusDirection = Vector3.Cross(vDir, spinAxis.normalized).normalized;
                magnusForce = magnusForceMagnitude * magnusDirection;
            }
            else if (pitchTime >= 0.3f && pitchTime <= 0.4f)
            {
                float magnusForceMagnitude = 1.5f * magnusForceConstants * (v * v);
                Vector3 magnusDirection = Vector3.Cross(vDir, spinAxis.normalized).normalized;
                magnusForce = magnusForceMagnitude * magnusDirection;
            }

            // sum all forces and divide by mass
            Vector3 totalForce = dragForce + magnusForce + gravityForce;
            Vector3 acceleration = totalForce / mass;

            // update velocity with new acceleration
            velocity += acceleration * Time.fixedDeltaTime;

            // update position
            transform.position += velocity * Time.fixedDeltaTime;

            if (transform.position.z <= 0 && !pitchConcluded)
            {
                pitchConcluded = true;
                Debug.Log("Pitch Location: " + transform.position);
                Debug.Log("Pitch Time: " + pitchTime);
            }

            if (transform.position.z < -2)
            {
                movePitch = false;
            }
        }
    }

    private void MovePitch()
    {
        movePitch = true;
    }

}
