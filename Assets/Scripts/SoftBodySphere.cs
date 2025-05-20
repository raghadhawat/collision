using System.Collections.Generic;
using UnityEngine;

public class SoftBodyRubberBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public float radius = 1f;
    public int resolution = 5;
    public float pointMass = 1f;
    public float stiffness = 800f;
    public float damping = 10f;
    
    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    private List<MassPoint> points = new List<MassPoint>();
    private List<Spring> springs = new List<Spring>();

    private bool hasTouchedGround = false;

    void Start()
    {
        CreateSolidBall();
        ConnectSprings();
    }

    void FixedUpdate()
    {
        bool justTouchedGround = false;

        // تطبيق القوى على النقاط
        foreach (var p in points)
        {
            if (!p.isFixed)
            {
                p.ApplyForce(gravity * pointMass);

                // تصادم مع الأرض
                if (p.Position.y < 0)
                {
                    float penetration = -p.Position.y;
                    Vector3 restoring = Vector3.up * penetration * stiffness * 0.5f;
                    p.ApplyForce(restoring);

                    if (p.Velocity.y < 0)
                        p.Velocity = new Vector3(p.Velocity.x, -p.Velocity.y * 0.3f, p.Velocity.z);

                    justTouchedGround = true;
                }
            }
        }

        // تطبيق الزنبركات
        foreach (var s in springs)
        {
            s.Apply(hasTouchedGround); // قوة أقوى بعد التصادم
        }

        // تحديث المواقع
        foreach (var p in points)
            p.Integrate(Time.fixedDeltaTime);

        // عند أول تصادم، نقوي الروابط
        if (!hasTouchedGround && justTouchedGround)
        {
            hasTouchedGround = true;
            Debug.Log("✅ الكرة لامست الأرض لأول مرة: تم تقوية الروابط.");
        }
    }

    void CreateSolidBall()
    {
        float step = (radius * 2) / resolution;
        Vector3 center = transform.position;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    Vector3 offset = new Vector3(
                        x * step - radius,
                        y * step - radius,
                        z * step - radius);

                    if (offset.magnitude <= radius)
                    {
                        Vector3 worldPos = center + offset;
                        GameObject pointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        pointObj.transform.position = worldPos;
                        pointObj.transform.localScale = Vector3.one * 0.1f;
                        Destroy(pointObj.GetComponent<Collider>());
                        pointObj.transform.SetParent(transform);

                        MassPoint mp = new MassPoint(worldPos, pointMass);
                        mp.visual = pointObj.transform;
                        points.Add(mp);
                    }
                }
            }
        }
    }

    void ConnectSprings()
    {
        float maxDistance = (radius * 2) / resolution * 1.2f;

        for (int i = 0; i < points.Count; i++)
        {
            for (int j = i + 1; j < points.Count; j++)
            {
                float dist = Vector3.Distance(points[i].Position, points[j].Position);
                if (dist <= maxDistance)
                {
                    springs.Add(new Spring(points[i], points[j], stiffness, damping));
                }
            }
        }
    }
}
