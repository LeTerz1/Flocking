using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class Fish : MonoBehaviour
{
    public FishScriptableObjectScript FishScriptable;

    private void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        FishScriptable.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update()
    {
        var boidColliders = Physics2D.OverlapCircleAll(transform.position, FishScriptable.neighborhoodRadius);
        var boids = boidColliders.Select(boidCollider => boidCollider.GetComponent<Fish>()).ToList();
        boids.Remove(this);

        ComputeAcceleration(boids);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
    }

    private void ComputeAcceleration(IEnumerable<Fish> boids)
    {
        var alignment = Alignment(boids);
        var separation = Separation(boids);
        var cohesion = Cohesion(boids);

        FishScriptable.acceleration = FishScriptable.alignmentAmount * alignment + FishScriptable.cohesionAmount * cohesion + FishScriptable.separationAmount * separation;
    }

    public void UpdateVelocity()
    {
        FishScriptable.velocity += FishScriptable.acceleration;
        FishScriptable.velocity = LimitMagnitude(FishScriptable.velocity, FishScriptable.maxSpeed);
    }

    private void UpdatePosition()
    {
        transform.Translate(FishScriptable.velocity * Time.deltaTime, Space.World);
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(FishScriptable.velocity.y, FishScriptable.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private Vector2 Alignment(IEnumerable<Fish> boids)
    {
        var velocity = Vector2.zero;
        if (!boids.Any()) return velocity;

        foreach (var boid in boids)
        {
            velocity += boid.FishScriptable.velocity;
        }

        velocity /= boids.Count();
        var steer = Steer(velocity.normalized * FishScriptable.maxSpeed);
        return steer;
    }

    private Vector2 Cohesion(IEnumerable<Fish> boids)
    {
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (var boid in boids)
        {
            sumPositions += (Vector2)boid.transform.position;
        }

        var average = sumPositions / boids.Count();
        var direction = average - (Vector2)transform.position;
        var steer = Steer(direction.normalized * FishScriptable.maxSpeed);
        return steer;
    }

    private Vector2 Separation(IEnumerable<Fish> boids)
    {
        var direction = Vector2.zero;
        boids = boids.Where(boid => Vector2.Distance(transform.position, boid.transform.position) <= FishScriptable.separationRadius);
        if (!boids.Any()) return direction;

        foreach (var boid in boids)
        {
            Vector2 difference = transform.position - boid.transform.position;
            direction += difference.normalized;
        }

        direction /= boids.Count();
        var steer = Steer(direction.normalized * FishScriptable.maxSpeed);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - FishScriptable.velocity;
        steer = LimitMagnitude(steer, FishScriptable.maxForce);
        return steer;
    }

    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }

        return baseVector;
    }

    private void OnDrawGizmosSelected()
    {
        // Neighborhood radius.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, FishScriptable.neighborhoodRadius);

        // Separation radius.
        Gizmos.color = Color.salmon;
        Gizmos.DrawWireSphere(transform.position, FishScriptable.separationRadius);
    }
}