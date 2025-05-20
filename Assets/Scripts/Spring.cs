using UnityEngine;

public class Spring
{
    public MassPoint A, B;
    private float restLength;
    private float baseStiffness;
    private float damping;

    public Spring(MassPoint a, MassPoint b, float stiffness, float damping)
    {
        A = a;
        B = b;
        this.baseStiffness = stiffness;
        this.damping = damping;
        restLength = Vector3.Distance(A.Position, B.Position);
    }

    public void Apply(bool stronger = false)
    {
        Vector3 delta = B.Position - A.Position;
        float dist = delta.magnitude;
        Vector3 dir = delta.normalized;

        float k = stronger ? baseStiffness * 2f : baseStiffness;

        Vector3 springForce = k * (dist - restLength) * dir;
        Vector3 dampingForce = damping * (B.Velocity - A.Velocity);

        A.ApplyForce(springForce + dampingForce);
        B.ApplyForce(-springForce - dampingForce);
    }
}
