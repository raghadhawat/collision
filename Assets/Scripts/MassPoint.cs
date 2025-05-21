using UnityEngine;

public class MassPoint
{
    public Vector3 Position;
    public Vector3 Velocity;
    public float Mass;
    public bool isFixed = false;
    public Transform visual;
    private Vector3 force;

    public bool isOuter = false;


    public MassPoint(Vector3 offset, Vector3 position, float mass)
    {
        Position = position;
        Velocity = Vector3.zero;
        Mass = mass;
        force = Vector3.zero;
    }

    public void ApplyForce(Vector3 f)
    {
        force += f;
    }

    public void Integrate(float dt)
    {
        if (isFixed) return;

        Vector3 acceleration = force / Mass;
        Velocity += acceleration * dt;
        Position += Velocity * dt;

        if (visual != null)
            visual.position = Position;

        force = Vector3.zero;
    }
}
