using UnityEngine;

public class Spring
{
    private MassPoint a, b;
    private float restLength;
    private float stiffness, damping;

    public Spring(MassPoint a, MassPoint b, float stiffness, float damping)
    {
        this.a = a;
        this.b = b;
        this.restLength = Vector3.Distance(a.Position, b.Position);
        this.stiffness = stiffness;
        this.damping = damping;
    }

    public void UpdateStiffness(float newStiffness, float newDamping)
    {
        stiffness = newStiffness;
        damping = newDamping;
    }

    public void Apply(bool reinforce = false)
    {
        Vector3 delta = b.Position - a.Position;
        float dist = delta.magnitude;
        if (dist == 0) return;

        Vector3 dir = delta / dist;
        float stretch = dist - restLength;
        float forceMag = stiffness * stretch;

        Vector3 relativeVelocity = b.Velocity - a.Velocity;
        float dampForce = damping * Vector3.Dot(relativeVelocity, dir);

        Vector3 totalForce = (forceMag + dampForce) * dir;

        a.ApplyForce(totalForce);
        b.ApplyForce(-totalForce);
    }
}
