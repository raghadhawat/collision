using System.Collections.Generic;
using UnityEngine;

public class SoftBodyRubberBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public float radius = 2f;
    public int resolution = 20;
    public float pointMass = 1f;
    public float stiffness = 800f;
    public float damping = 10f;

    public Vector3 gravity = new Vector3(0, -9.81f, 0);

    private List<MassPoint> points = new List<MassPoint>();
    private List<Spring> springs = new List<Spring>();

    private bool hasTouchedGround = false;

    void Start()
    {
        // Automatically set the radius based on the GameObject's scale (assuming uniform scaling)
        radius = transform.localScale.x / 2f;
        radius -= radius / 35f;

        // Optional: Reset scale to 1 so visual points match the size better
        // transform.localScale = Vector3.one;

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
            stiffness *= 2f;  // تقوية الزنبركات بعد التصادم
            damping *= 2f;

            foreach (var s in springs)
            {
                s.UpdateStiffness(stiffness, damping);
            }
        }
    }

    void CreateSolidBall()
    {
        bool IsOuterPoint(Vector3 offset, float tolerance)
        {
            return Mathf.Abs(offset.magnitude - radius) < tolerance;
        }

        float step = (radius * 2) / resolution;
        Vector3 center = transform.position;

        for (float x = -radius; x <= radius; x += step)
        {
            for (float y = -radius; y <= radius; y += step)
            {
                for (float z = -radius; z <= radius; z += step)
                {
                    float distanceSqr = x * x + y * y + z * z;
                    if (distanceSqr <= radius * radius)
                    {
                        // Vector3 worldPos = center + new Vector3(x, y, z);
                        Vector3 offset = new Vector3(x, y, z); // already relative
                        Vector3 worldPos = center + offset;
                        GameObject pointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        pointObj.transform.position = worldPos;
                        pointObj.transform.localScale = Vector3.one * step * 0.6f;
                        Destroy(pointObj.GetComponent<Collider>());
                        pointObj.transform.SetParent(transform);

                        bool isOuter = IsOuterPoint(offset, step * 0.7f);
                        MassPoint mp = new MassPoint(offset, worldPos, pointMass);
                        mp.visual = pointObj.transform;
                        mp.isOuter = isOuter;
                        if (isOuter && offset.y < -radius * 0.8f)
                        {
                            // تثبيت بعض النقاط السفلية
                            mp.isFixed = true;
                        }
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
