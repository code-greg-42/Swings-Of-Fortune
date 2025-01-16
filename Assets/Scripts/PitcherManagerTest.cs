using System.Collections;
using UnityEngine;

public class PitcherManagerTest : MonoBehaviour
{
    [SerializeField] private Vector3 targetLocation;
    [SerializeField] private PitcherTest[] pitchers;

    [Header("Reference")]
    [SerializeField] private Transform ballTransform;

    Vector3 ballVelocity;
    Vector3 ballAcceleration;
    Vector3 ballJerk;

    Vector3 releasePoint;

    private bool pitchLogged = false;
    private float pitchTime = 0.0f;

    private void Start()
    {
        PitcherTest pitcher = pitchers[0];
        (Vector3 point, Vector3 initialVelocity, float pitchSpeed, Vector3 accel, Vector3 jerk) = pitcher.GetPitch(targetLocation);

        Debug.Log("Init Velocity: " + initialVelocity);
        Debug.Log("Pitch Speed: " + pitchSpeed);
        Debug.Log("Accel: " + accel);
        Debug.Log("Jerk: " + jerk);

        ballVelocity = initialVelocity;
    }

    private void Update()
    {
        if (!pitchLogged)
        {
            float deltaTime = Time.deltaTime;
            pitchTime += deltaTime;
            ballAcceleration += ballJerk * deltaTime;
            ballVelocity += ballAcceleration * deltaTime;
            ballTransform.position += ballVelocity * deltaTime;

            if (pitchTime >= 0.5f)
            {
                Debug.Log("Pitch Location Calculated At: " + pitchTime);
                Debug.Log("Pitch Location: " + ballTransform.position);
                Debug.Log("Pitch Velocity: " + ballVelocity);
                pitchLogged = true;
            }
        }
    }

    private IEnumerator ThrowBallCoroutine()
    {
        PitcherTest pitcher = pitchers[0];

        while (true)
        {
            (Vector3 releasePoint, Vector3 initialVelocity, float pitchSpeed, Vector3 accel, Vector3 jerk) = pitcher.GetPitch(targetLocation);

            Debug.Log("InitialVelocity: " + initialVelocity);

            Debug.Log("Resetting ball to: " + releasePoint);
            ballTransform.position = releasePoint;

            Debug.Log("Waiting 1 second...");
            yield return new WaitForSeconds(1.0f);

            float pitchTime = Mathf.Abs(ballTransform.position.z / pitchSpeed);
            Debug.Log("Upcoming pitch time of: " + pitchTime);

            float elapsedTime = 0.0f;

            ballVelocity = initialVelocity;

            Debug.Log("Entering Movement Loop");
            while (elapsedTime <= pitchTime)
            {
                float deltaTime = Time.deltaTime;
                elapsedTime += deltaTime;

                Debug.Log("Moving ball");
                ballAcceleration += jerk * deltaTime;
                ballVelocity += ballAcceleration * deltaTime;
                ballTransform.position += ballVelocity * deltaTime;
                yield return null;
            }

            Debug.Log("Resetting Ball");
            ballJerk = Vector3.zero;
            ballAcceleration = Vector3.zero;
            ballVelocity = Vector3.zero;

            yield return new WaitForSeconds(1.0f);
        }
    }
    
}
