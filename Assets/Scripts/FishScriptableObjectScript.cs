using UnityEngine;

[CreateAssetMenu(fileName = "FishScriptableObjectScript", menuName = "Scriptable Objects/FishScriptableObjectScript")]
public class FishScriptableObjectScript : ScriptableObject
{
    [Range(0, 10)]
    public float maxSpeed = 1f;

    [Range(.1f, .5f)]
    public float maxForce = .03f;

    [Range(1, 10)]
    public float neighborhoodRadius = 3f;

    [Range(0.1f, 10f)]
    public float separationRadius = 1f;

    [Range(0, 3)]
    public float separationAmount = 1f;

    [Range(0, 3)]
    public float cohesionAmount = 1f;

    [Range(0, 3)]
    public float alignmentAmount = 1f;

    public Vector2 acceleration;
    public Vector2 velocity;
}
