using UnityEngine;

[System.Serializable]
public class PitcherTest
{
    [SerializeField] private SpecificPitchTest[] pitches;

    [SerializeField] private Vector3 releasePoint;

    public (Vector3, Vector3, float, float, Vector3, Vector3) GetPitch(Vector3 targetLocation)
    {
        SpecificPitchTest pitch = pitches[0];

        (Vector3 initialVelocity, float pitchSpeed, float timeToPlate, Vector3 accel, Vector3 jerk) = pitch.GetPitch(releasePoint, targetLocation);

        return (releasePoint, initialVelocity, pitchSpeed, timeToPlate, accel, jerk);
    }
}
