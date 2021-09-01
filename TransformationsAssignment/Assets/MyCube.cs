// Computer grafics
// August 22, 2021
// Javier Flores - A01651678
// Enrique Orduna - A01027318
// Jose Javier Tlacuilo - A01420128

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCube : MonoBehaviour
{
    // We can have two meshes in the script.
    public GameObject translatedGO;
    public GameObject rotatetedGO;

    Vector3[] geometry;
    Vector3[] normals;
    int[] topology;

    void TranslateCube() {
        Matrix4x4 transM = Transformations.TranslateM(7, 8, -4);
        Vector3[] transformed = new Vector3[geometry.Length];

        for (int i = 0; i < geometry.Length; i++) { // A GPU wouldnt need to perform the loop.
            // Create a vector of size 4 to match rotM .
            Vector4 temp = new Vector4(geometry[i].x, geometry[i].y, geometry[i].z, 1);
            // Transformations could be accumulated and applied at this point.
            transformed[i] = transM * temp; // Unity removes w and stores it as a vector 3.
            // Debug.Log("translation["+i+"]="+transformed[i]);
        }

        Debug.Log(transformed);

        // Assign to the corresponding unity structure that is a mesh
        Mesh mesh = translatedGO.GetComponent<MeshFilter>().mesh;
        // mesh.Clear(); dont remove the mesh, ortiginal geometry but transformed vertices.

        // For Unity vertices is geometry, triangles is topology.
        mesh.vertices = transformed;
        mesh.triangles = topology; // They also share topology.
    }

    void GoToPivot() {
        Matrix4x4 transM = Transformations.TranslateM(3.12f, -3.85f, -5.27f);
        Vector3[] geo = translatedGO.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] transformed = new Vector3[geo.Length];

        for (int i = 0; i < geo.Length; i++) { // A GPU wouldnt need to perform the loop.
            // Create a vector of size 4 to match rotM .
            Vector4 temp = new Vector4(geo[i].x, geo[i].y, geo[i].z, 1);
            // Transformations could be accumulated and applied at this point.
            transformed[i] = transM * temp; // Unity removes w and stores it as a vector 3.
            Debug.Log("temp["+i+"]="+temp);
            Debug.Log("goToPivot["+i+"]="+transformed[i]);
        }

        // Assign to the corresponding unity structure that is a mesh
        Mesh mesh = rotatetedGO.GetComponent<MeshFilter>().mesh;

        // For Unity vertices is geometry, triangles is topology.
        mesh.vertices = transformed;
        mesh.triangles = topology; // They also share topology.
    }

    void RotateCube() {
        Matrix4x4 rotM = Transformations.RotateM(-50, Transformations.AXIS.AX_X);
        Vector3[] geo = rotatetedGO.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] rotation = new Vector3[geo.Length];

        for (int i = 0; i < geometry.Length; i++) { // A GPU wouldnt need to perform the loop.
            // Create a vector of size 4 to match rotM .
            Vector4 temp = new Vector4(geo[i].x, geo[i].y, geo[i].z, 1);
            // Transformations could be accumulated and applied at this point.
            rotation[i] = rotM * temp; // Unity removes w and stores it as a vector 3.
            Debug.Log("rotation["+i+"]="+rotation[i]);
        }

        // Assign to the corresponding unity structure that is a mesh
        Mesh mesh = rotatetedGO.GetComponent<MeshFilter>().mesh;

        // For Unity vertices is geometry, triangles is topology.
        mesh.vertices = rotation;
        mesh.triangles = topology; // They also share topology.
    }

    void ReturnFromPivot() {
        Matrix4x4 transM = Transformations.TranslateM(-3.12f, 3.85f, 5.27f);
        Vector3[] geo = rotatetedGO.GetComponent<MeshFilter>().mesh.vertices;
        Vector3[] transformed = new Vector3[geo.Length];

        for (int i = 0; i < geo.Length; i++) { // A GPU wouldnt need to perform the loop.
            // Create a vector of size 4 to match rotM .
            Vector4 temp = new Vector4(geo[i].x, geo[i].y, geo[i].z, 1);
            // Transformations could be accumulated and applied at this point.
            transformed[i] = transM * temp; // Unity removes w and stores it as a vector 3.
            Debug.Log("returnFromPivot["+i+"]="+transformed[i]);
        }

        // Assign to the corresponding unity structure that is a mesh
        Mesh mesh = rotatetedGO.GetComponent<MeshFilter>().mesh;
        // mesh.Clear(); dont remove the mesh, ortiginal geometry but transformed vertices.

        // For Unity vertices is geometry, triangles is topology.
        mesh.vertices = transformed;
        mesh.triangles = topology; // They also share topology.
    }

    // Start is called before the first frame update
    void Start()
    {
        // Objective: make a cube using Geometry and Topology
        // https://docs.unity3d.com/ScriptReference/Mesh.html

        // Constructing a cube.
        // x y z
        float S = 10.0f;
        float HS = S / 2.0f;

        // Front face. 

        // Four points that make up the geometry
        Vector3 v0 = new Vector3(-HS, -HS, HS);
        Vector3 v1 = new Vector3(HS, -HS, HS);
        Vector3 v2 = new Vector3(HS, HS, HS);
        Vector3 v3 = new Vector3(-HS, HS, HS);

        // Side face. 

        // Four points that make up the geometry
        Vector3 v4 = new Vector3(HS, -HS, -HS);
        Vector3 v5 = new Vector3(HS, HS, -HS);

        // Bottom face
        Vector3 v6 = new Vector3(-HS, -HS, -HS);

        // Top face
        Vector3 v7 = new Vector3(-HS, HS, -HS);

        geometry = new Vector3[]{v0, v1, v2, v3, v4, v5, v6, v7};

        // Topology is represented by an int array.

        // Front = {0, 1, 2, 0, 2, 3}
        // Right = {1, 4, 5, 1, 5, 2}
        // Back = {4, 6, 7, 4, 7, 5}
        // Left = {6, 0, 3, 6, 3, 7}
        // Top = {3, 2, 5, 3, 5, 7}
        // Bottom = {1, 0, 6, 1, 6, 4}
        topology = new int[]{0, 1, 2, 0, 2, 3, 1, 4, 5, 1, 5, 2, 4, 6, 7, 4, 7, 5, 6, 0, 3, 6, 3, 7, 3, 2, 5, 3, 5, 7, 1, 0, 6, 1, 6, 4};

        // Normals
        Vector3 n0 = new Vector3(0,0,1);
        Vector3 n1 = new Vector3(1,0,0);
        Vector3 n2 = new Vector3(0,0,-1);
        Vector3 n3 = new Vector3(-1,0,0);
        Vector3 n4 = new Vector3(0,1,0);
        Vector3 n5 = new Vector3(0,0,0);

        // Repeating normals to satisfy unity's size == size of vertices array.
        normals = new Vector3[] { n0, n1, n2, n3, n4, n5, n5, n5 };


        // Assign to the corresponding unity structure that is a mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // For Unity vertices is geometry, triangles is topology.
        mesh.vertices = geometry;
        mesh.triangles = topology; // It is important that the topology doesnt change when transformation happen.
        mesh.normals = normals;

        TranslateCube();
        GoToPivot();
        RotateCube();
        ReturnFromPivot();
        mesh.RecalculateNormals();
    }

    // Update is called once per frame
    // 30 to 60 times per second.
    void Update()
    {
        // Paint the axis to give reference
        Debug.DrawLine(Vector3.zero, new Vector3(10,0,0), Color.red);
        Debug.DrawLine(Vector3.zero, new Vector3(0,10,0), Color.green);
        Debug.DrawLine(Vector3.zero, new Vector3(0,0,10), Color.blue);
    }
}
