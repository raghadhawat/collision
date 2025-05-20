using UnityEngine;

public class MassPoint
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Mass;
    public bool isFixed = false;
    public Transform visual;

    private Vector3 accumulatedForce;

    public MassPoint(Vector3 position, float mass)
    {
        Position = position;
        Velocity = Vector3.zero;
        Mass = mass;
    }

    public void ApplyForce(Vector3 force)
    {
        accumulatedForce += force;
    }

    public void Integrate(float dt)
    {
        if (isFixed) return;

        Vector3 acceleration = accumulatedForce / Mass;
        Velocity += acceleration * dt;
        Position += Velocity * dt;
        accumulatedForce = Vector3.zero;

        if (visual != null)
            visual.position = Position;
    }
}
