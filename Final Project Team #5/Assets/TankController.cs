using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public Matrix4x4 pastTs;
    public Vector3[] originals;

    Vector3 cannonPoint;
    GameObject sphere;
    public Vector3 originalCannon;

    // Start is called before the first frame update
    void Start()
    {
        speed = 1.0f;
        rotationSpeed = 15.0f;

        pastTs = Matrix4x4.identity;
        cannonPoint = new Vector3(0, 1.0f, 0.5f);
        originalCannon = cannonPoint;

        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        sphere.transform.localPosition = cannonPoint;

        Mesh m = GetComponent<MeshFilter>().mesh;
        originals = new Vector3[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            originals[i] = m.vertices[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKey(KeyCode.LeftShift))
       {
           if (Input.GetKeyDown(KeyCode.A)) 
           {
               Debug.Log("Rotate left");
               Rotate("A");
           }
           else if (Input.GetKeyDown(KeyCode.D))
           {
               Debug.Log("Rotate right");
               Rotate("D");
           }
       
       } 
       else if (Input.GetKeyDown(KeyCode.W))
       {
           Debug.Log("Move forward");
           Move("W");
       }
       else if (Input.GetKeyDown(KeyCode.A))
       {
           Debug.Log("Move left");
           Move("A");
       }
       else if (Input.GetKeyDown(KeyCode.S))
       {
           Debug.Log("Move back");
           Move("S");
       }
       else if (Input.GetKeyDown(KeyCode.D))
       {
           Debug.Log("Move right");
           Move("D");
       }
    }

    void Rotate(string direction)
    {
        Matrix4x4 rm;
        switch (direction)
        {
            case "A":
                rm = Transformations.RotateM(rotationSpeed, Transformations.AXIS.AX_Y);
                break;
            case "D":
                rm = Transformations.RotateM(-rotationSpeed, Transformations.AXIS.AX_Y);
                break;
            default:
                rm = Transformations.RotateM(0, Transformations.AXIS.AX_Y);
                break;
        }
        ApplyTransformation(rm);
    }

    void Move(string direction)
    {
        Matrix4x4 tm;
        switch (direction)
        {
            case "W":
                tm = Transformations.TranslateM(0, 0, speed);
                break;
            case "A":
                tm = Transformations.TranslateM(-speed, 0, 0);
                break;
            case "S":
                tm = Transformations.TranslateM(0, 0, -speed);
                break;
            case "D":
                tm = Transformations.TranslateM(speed, 0, 0);
                break;
            default:
                tm = Transformations.TranslateM(0, 0, 0);
                break;
        }
        ApplyTransformation(tm);
    }

    void ApplyTransformation(Matrix4x4 tm)
    {
        pastTs = pastTs * tm;
        Mesh m = GetComponent<MeshFilter>().mesh;
        Vector3[] transform = new Vector3[m.vertices.Length];
        for (int i = 0; i < m.vertices.Length; i++)
        {
            Vector3 v = originals[i];
            Vector4 temp = new Vector4(v.x, v.y, v.z, 1);
            transform[i] = pastTs * temp;
        }
        m.vertices = transform;

        // Set cannon point
        Vector4 tempPoint = new Vector4(originalCannon.x, originalCannon.y, originalCannon.z, 1);
        cannonPoint = pastTs * tempPoint;
        sphere.transform.localPosition = cannonPoint;
    }
}
