// Computer grafics
// September 4, 2021
// Animation Assignment Spider

// Javier Flores - A01651678
// Enrique Orduna - A01027318
// Jose Javier Tlacuilo - A01420128

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg
{
    static int SECTIONS = 3;
    static int leg_count = 1;

    GameObject[] section; // Array of sections
    List<Vector3[]> originals; // Array of original position of section

    float sAngle; // Max angle of leg z rotation
    float width; // Section width
    float length;// Section length
    Vector3 position; // Initial position of the leg.
    int posAngle; // Original angle of the leg in z.
    bool isForward; // Whether the leg starts in a foward position

    // Keeps changes in rotation.
    float sectionRotZ;
    float dirZ;

    // Initializer
    public Leg(float sAngle, float width, float length, Vector3 position, int posAngle, bool isForward)
    {
        this.sAngle = sAngle;
        this.width = width;
        this.length = length;
        this.position = position;
        this.posAngle = posAngle;
        this.isForward = isForward;

        // Some legs start in the forward position others dont.
        this.sectionRotZ = (isForward) ? -sAngle : -5;
        this.dirZ = (isForward) ? 1.0f : -1.0f;

        section = new GameObject[SECTIONS];
        originals = new List<Vector3[]>();

        for (int i = 0; i < SECTIONS; i++)
        {
            // Create leg sections;
            section[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            section[i].name = "Leg section #"+leg_count;
            leg_count++;
            ExtractVertices(section[i], originals);
        }
    }

    // Move leg
    public void Move(float speed)
    {
        // Update angles for section rotations.
        sectionRotZ += dirZ * speed;
        if (sectionRotZ < -sAngle || sectionRotZ > -5) dirZ = -dirZ;

        // Leg rotation in Y to show forward movement.
        float angleInY = (position.x < 0) ? -sectionRotZ/2.5f : sectionRotZ/2.5f;
        Matrix4x4 rY = Transformations.RotateM(angleInY, Transformations.AXIS.AX_Y);

        // Create transformations.

        // Sets original position of the leg.
        Matrix4x4 ot = Transformations.TranslateM(position.x, position.y, position.z);
        // Sets original position of the leg around the body.
        Matrix4x4 or = Transformations.RotateM(posAngle, Transformations.AXIS.AX_Y);
        // Sets an original inclination of the leg.
        Matrix4x4 orZ = Transformations.RotateM(50, Transformations.AXIS.AX_Z);
        // Calculated translation and rotation of each section
        Matrix4x4 t = Transformations.TranslateM(length/2, 0, 0);
        Matrix4x4 r1 = Transformations.RotateM(sectionRotZ, Transformations.AXIS.AX_Z);
        Matrix4x4 r2 = Transformations.RotateM(sectionRotZ * 2.3f, Transformations.AXIS.AX_Z);
        Matrix4x4 r3 = Transformations.RotateM(sectionRotZ * 2.3f, Transformations.AXIS.AX_Z);
        // Scale of the sections.
        Matrix4x4 s = Transformations.ScaleM(length, width, width);

        // Create per link transformations.
        Matrix4x4 t1 = rY * ot * or * orZ * r1 * t;
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
