// Computer grafics
// Transformations Quiz

// August 27, 2021

// Javier Flores - A01651678
// Enrique Orduna - A01027318
// Jose Javier Tlacuilo - A01420128

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arm : MonoBehaviour
{
    public GameObject Link1;
    public GameObject Link2;
    public GameObject Link3;

    public float deltaA;

    Vector3[] a_origin;
    Vector3[] b_origin;
    Vector3[] c_origin;

    float rotA; 
    bool isIncreasing;

    // Start is called before the first frame update
    void Start()
    {
        isIncreasing = true;

        // We save the original position of the links 

        Mesh a_mesh = Link1.GetComponent<MeshFilter>().mesh;
        a_origin = new Vector3[Link1.GetComponent<MeshFilter>().mesh.vertices.Length];
        for (int i = 0; i < a_origin.Length; i++) {
            a_origin[i] = a_mesh.vertices[i];
        }

        Mesh b_mesh = Link2.GetComponent<MeshFilter>().mesh;
        b_origin = new Vector3[Link2.GetComponent<MeshFilter>().mesh.vertices.Length];
        for (int i = 0; i < b_origin.Length; i++) {
            b_origin[i] = b_mesh.vertices[i];
        }

        Mesh c_mesh = Link3.GetComponent<MeshFilter>().mesh;
        c_origin = new Vector3[Link3.GetComponent<MeshFilter>().mesh.vertices.Length];
        for (int i = 0; i < c_origin.Length; i++) {
            c_origin[i] = c_mesh.vertices[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rotA >= 45) {
            isIncreasing = false;
        } else if (rotA <= -45) {
            isIncreasing = true;
        }

        if ( isIncreasing ) {
            rotA += deltaA; // Here rotation is aggregated.
        } else {
            rotA -= deltaA; // Here rotation is aggregated.
        }


        Matrix4x4 rotL = Transformations.RotateM(rotA, Transformations.AXIS.AX_Z);;
        Matrix4x4 scaleL = Transformations.ScaleM(1.0f, 0.5f, 0.5f);
        Matrix4x4 tranL = Transformations.TranslateM(1.0f, 0, 0);
        Matrix4x4 tranIni = Transformations.TranslateM(0.5f, 0, 0);

        // Pivot
        Matrix4x4 pivotIn = Transformations.TranslateM(-0.5f, 0, 0);
        Matrix4x4 pivotOut = Transformations.TranslateM(0.5f, 0, 0);

        // Link 1
        Mesh meshL1 = Link1.GetComponent<MeshFilter>().mesh;

        Vector3[] transformedL1 = new Vector3[a_origin.Length];
        for (int i = 0; i < a_origin.Length; i++) {
            Vector3 o = a_origin[i];
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            transformedL1[i] = tranIni * pivotIn * rotL * pivotOut * scaleL * temp;
        }

        meshL1.vertices = transformedL1;
        meshL1.RecalculateNormals();

        // Link 2
        Mesh meshL2 = Link2.GetComponent<MeshFilter>().mesh;

        Vector3[] transformedL2 = new Vector3[b_origin.Length];
        for (int i = 0; i < b_origin.Length; i++) {
            Vector3 o = b_origin[i];
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            transformedL2[i] = tranIni * pivotIn * rotL * pivotOut * tranL * pivotIn * rotL * pivotOut * scaleL * temp;
        }

        meshL2.vertices = transformedL2;
        meshL2.RecalculateNormals();

        // Link3
        Mesh meshL3 = Link3.GetComponent<MeshFilter>().mesh;

        Vector3[] transformedL3 = new Vector3[c_origin.Length];
        for (int i = 0; i < c_origin.Length; i++) {
            Vector3 o = c_origin[i];
            Vector4 temp = new Vector4(o.x, o.y, o.z, 1);
            transformedL3[i] = tranIni * pivotIn * rotL * pivotOut * tranL * pivotIn * rotL * pivotOut * tranL * pivotIn * rotL * pivotOut * scaleL * temp;
        }

        meshL3.vertices = transformedL3;
        meshL3.RecalculateNormals();
    }
}
