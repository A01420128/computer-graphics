// Computer grafics
// September 4, 2021
// Animation Assignment Spider

// Javier Flores - A01651678
// Enrique Orduna - A01027318
// Jose Javier Tlacuilo - A01420128

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySphere
{

    // Initializer
    public BodySphere()
    {
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Spider head";
        Vector3[] vh = ExtractVertices(head);
        Matrix4x4 th = Transformations.TranslateM(0, 0.1f, 0.5f);
        Matrix4x4 sh = Transformations.ScaleM(1f, 1f, 1.15f);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.name = "Spider body";
        Vector3[] vb = ExtractVertices(body);
        Matrix4x4 tb = Transformations.TranslateM(0, 0.35f, -0.5f);
        Matrix4x4 sb = Transformations.ScaleM(1.5f, 1.5f, 1.75f);

        ApplyTransformations(head, th, vh);
        ApplyTransformations(body, tb*sb, vb);

    }

    // Extract vertices from a game objecto into a list storage.
    Vector3[] ExtractVertices(GameObject go)
    {
        Mesh m = go.GetComponent<MeshFilter>().mesh;
        Vector3[] o = new Vector3[m.vertices.Length];
        for (int v = 0; v < m.vertices.Length; v++)
        {
            o[v] = new Vector3(m.vertices[v].x, m.vertices[v].y, m.vertices[v].z);
        }
        return o;
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
