using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg
{
    static int SECTIONS = 3;

    GameObject[] section;
    List<Vector3[]> originals;

    int sAngle;

    float[] sectionRotZ;
    float[] deltaRotZ;
    float[] dirZ;

    // Start is called before the first frame update
    public Leg(int sAngle) // TODO: Send initial translation rotation.
    {
        this.sAngle = sAngle;

        section = new GameObject[SECTIONS];
        originals = new List<Vector3[]>();

        sectionRotZ = new float[SECTIONS];
        deltaRotZ = new float[SECTIONS];
        dirZ = new float[SECTIONS];

        for (int i = 0; i < SECTIONS; i++)
        {
            // Create sections angles;
            sectionRotZ[i] = 0;
            deltaRotZ[i] = 0.1f;
            dirZ[i] = -1.0f;

            // Create leg sections;
            section[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            section[i].name = "link"+i;
            ExtractVertices(section[i], originals);
        }
    }

    // Move leg
    public void Move()
    {
        // Update angles
        for (int i = 0; i < 3; i++)
        {
            int angle = sAngle - (sAngle / 4) * i; // Each section has a different angle.
            sectionRotZ[i] += dirZ[i] * deltaRotZ[i];
            if (sectionRotZ[i] < -angle || sectionRotZ[i] > 0) dirZ[i] = -dirZ[i];
        }

        // Create transformations.
        Matrix4x4 t = Transformations.TranslateM(0.5f, 0, 0);
        Matrix4x4 r1 = Transformations.RotateM(sectionRotZ[0], Transformations.AXIS.AX_Z);
        Matrix4x4 r2 = Transformations.RotateM(sectionRotZ[1], Transformations.AXIS.AX_Z);
        Matrix4x4 r3 = Transformations.RotateM(sectionRotZ[2], Transformations.AXIS.AX_Z);
        Matrix4x4 s = Transformations.ScaleM(1, 0.2f, 0.2f);

        // Create per link transformations.
        Matrix4x4 t1 = r1 * t;
        Matrix4x4 t2 = t1 * t * r2 * t;
        Matrix4x4 t3 = t2 * t * r3 * t;

        // Apply per link transformations.
        ApplyTransformations(section[0], t1*s, originals[0]);
        ApplyTransformations(section[1], t2*s, originals[1]);
        ApplyTransformations(section[2], t3*s, originals[2]);
    }

    // Extract vertices from a game objecto into a list storage.
    void ExtractVertices(GameObject go, List<Vector3[]> storage)
    {
        Mesh m = go.GetComponent<MeshFilter>().mesh;
        Vector3[] o = new Vector3[m.vertices.Length];
        for (int v = 0; v < m.vertices.Length; v++)
        {
            o[v] = new Vector3(m.vertices[v].x, m.vertices[v].y, m.vertices[v].z);
        }
        storage.Add(o);
    }

    // Apply transformation 't' of a game object 'go' using original points 'o'.
    void ApplyTransformations(GameObject go, Matrix4x4 t, Vector3[] o)
    {
        Mesh m = go.GetComponent<MeshFilter>().mesh;
        Vector3[] transformed = new Vector3[o.Length];
        for (int v = 0; v < transformed.Length; v++)
        {
            Vector4 temp = new Vector4(o[v].x, o[v].y, o[v].z, 1);
            transformed[v] = t * temp;
        }
        m.vertices = transformed;
        m.RecalculateNormals();
    }
}
