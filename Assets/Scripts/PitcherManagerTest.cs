using System.Collections;
using UnityEngine;

public class PitcherManagerTest : MonoBehaviour
{
    [SerializeField] private Vector3 targetLocation;
    [SerializeField] private PitcherTest[] pitchers;

    [Header("Reference")]
    [SerializeField] private Transform ballTransform;
    [SerializeField] private GameObject displayBall;

    Vector3 ballVelocity;
    Vector3 ballAcceleration;
    Vector3 ballJerk;

    float totalPitchTime;
    float deactivateBallTime;
    float currentPitchSpeed;

    private bool pitchLogged = false;
    private float pitchTime = 0.0f;

    private bool pitchFinished = false;

    private void Start()
    {
        PitcherTest pitcher = pitchers[0];
        (Vector3 releasePoint, Vector3 initialVelocity, float pitchSpeed, float timeToPlate, Vector3 accel, Vector3 jerk) = pitcher.GetPitch(targetLocation);

        Debug.Log("Init Velocity: " + initialVelocity);
        Debug.Log("Pitch Speed: " + pitchSpeed);
        Debug.Log("Accel: " + accel);
        Debug.Log("Jerk: " + jerk);

        ballTransform.position = releasePoint;

        currentPitchSpeed = Mathf.Abs(pitchSpeed);

        Debug.Log("TimeToPlate: " + timeToPlate);
        totalPitchTime = timeToPlate;

        Debug.Log("TotalPitchTime: " + totalPitchTime);

        deactivateBallTime = timeToPlate + 5 / currentPitchSpeed;

        Debug.Log("DeactivateBallTime: " + deactivateBallTime);

        ballVelocity = initialVelocity;
        ballAcceleration = accel;
        ballJerk = jerk;
    }

    private void Update()
    {
        if (!pitchFinished)
        {
            float deltaTime = Time.deltaTime;
            pitchTime += deltaTime;
            ballAcceleration += ballJerk * deltaTime;
            ballVelocity += ballAcceleration * deltaTime;
            ballTransform.position += ballVelocity * deltaTime;



            if (pitchTime >= totalPitchTime && !pitchLogged)
            {
                displayBall.SetActive(true);
                displayBall.transform.position = targetLocation;
                Debug.Log("Pitch Location Calculated At: " + pitchTime);
                Debug.Log("Pitch Location: " + ballTransform.position);
                Debug.Log("Pitch Velocity: " + ballVelocity);
                Debug.Log("Pitch Acceleration: " + ballAcceleration);
                pitchLogged = true;
                pitchFinished = true;
            }

            if (pitchTime >= deactivateBallTime)
            {
                pitchFinished = true;
                Debug.Log("Pitch finished.");
            }
        }
    }
}
