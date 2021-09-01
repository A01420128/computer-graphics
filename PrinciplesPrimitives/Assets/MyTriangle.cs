using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTriangle : MonoBehaviour
{
    Vector3[] geometry;
    int[] topology;

    // Start is called before the first frame update
    void Start()
    {
        // Objective: make a triangle using Geometry and Topology
        // https://docs.unity3d.com/ScriptReference/Mesh.html

        float HS = 15.0f;

        // Three points that make up the geometry
        Vector3 v0 = new Vector3(-HS, -HS, 2*HS);
        Vector3 v1 = new Vector3(HS, -HS,2*HS);
        Vector3 v2 = new Vector3(HS, HS,2*HS);

        geometry = new Vector3[]{v0, v1, v2};
        topology = new int[]{0, 1, 2};


        // Assign to the corresponding unity structure that is a mesh
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // For Unity vertices is geometry, triangles is topology.
        mesh.vertices = geometry;
        mesh.triangles = topology;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
