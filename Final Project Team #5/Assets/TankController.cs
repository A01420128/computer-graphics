using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    Camera mainCamera;
    Vector3 cameraPoint;
    public Vector3 originalCameraPoint;

    public bool isPlaying;

    public float speed;
    public float rotationSpeed;
    public Matrix4x4 pastTs;
    public Vector3[] originals;
    public Vector3 centerPoint;
    public Vector3 originalCenterPoint;
    public float collisionRadius;

    Vector3 cannonPoint;
    public Vector3 originalCannonPoint;
    public float tankYRotation;

    // Bullet
    private BulletController bullet;

    enum WeaponType
    {
        Type1,
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); 
        speed = 1.0f;
        rotationSpeed = -15.0f;
        float longitud = GetComponent<MeshFilter>().mesh.bounds.size.z;
        collisionRadius = longitud;

        pastTs = Matrix4x4.identity;
        centerPoint = Vector3.zero;
        cannonPoint = new Vector3(0, 1.0f, 0.5f);
        cameraPoint = new Vector3(0, 4.0f, -10.0f);
        originalCenterPoint = centerPoint;
        originalCannonPoint = cannonPoint;
        originalCameraPoint = cameraPoint;

        mainCamera.transform.position = cameraPoint;
        mainCamera.transform.LookAt(cannonPoint);

        Mesh m = GetComponent<MeshFilter>().mesh;

        // Can this be changed to originals = m.vertices;
        Debug.Log(m.vertices.Length);
        originals = m.vertices;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            DetectMovement();
            DetectBulletAction();
        }
    }

    void DetectBulletAction()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
            {
                Debug.Log("Changed bullet vertical angle ++");
            } else if (Input.GetKeyDown(KeyCode.Minus))
            {
                Debug.Log("Changed bullet vertical angle --");
            }
        } else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Started shooting.");
            Shoot(WeaponType.Type1);
        } else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            Debug.Log("Changed initial bullet velocity ++");
        } else if (Input.GetKeyDown(KeyCode.Minus))
        {
            Debug.Log("Changed initial bullet velocity --");
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Switched to weapon 1");
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Switched to weapon 2");
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Switched to weapon 3");
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Switched to weapon 4");
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Switched to weapon 5");
        }
    }

    void DetectMovement()
    {
       if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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

    void Shoot(WeaponType weapon)
    {
        bullet = gameObject.AddComponent<BulletController>();
        bullet.mass = 10.0f;
        bullet.r = 0.5f;
        bullet.restitution = 0.00001f;
        bullet.cpos = cannonPoint;
        bullet.prev = bullet.cpos;
        bullet.colliding = false;

        float cannonForceMag;
        switch (weapon)
        {
            case WeaponType.Type1:
                cannonForceMag = 19.0f;
                break;
            default:
                cannonForceMag = 0.0f;
                break;
        }
        bullet.SetUp(GetCannonForce(cannonForceMag));
    }

    // Dividing cannon force in its three components.
    Vector3 GetCannonForce(float mag) {
        float thisAngle = this.tankYRotation * Mathf.Deg2Rad;
        float x = Mathf.Sin(thisAngle) * mag;
        float z = Mathf.Cos(thisAngle) * mag;
        float y = mag;
        return new Vector3(x, y, z);
    }

    public void CheckHits(TankController[] oponents)
    {
        if (bullet != null)
        {
            foreach(TankController tank in oponents)
            {
                if (bullet.CheckCollision(tank))
                {
                    Debug.Log("Collided with tank");
                }
            }
        }
    }

    void Rotate(string direction)
    {
        Matrix4x4 rm;
        switch (direction)
        {
            case "A":
                rm = Transformations.RotateM(rotationSpeed, Transformations.AXIS.AX_Y);
                this.tankYRotation += rotationSpeed;
                break;
            case "D":
                rm = Transformations.RotateM(-rotationSpeed, Transformations.AXIS.AX_Y);
                this.tankYRotation -= rotationSpeed;
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

        Vector4 tempPointCenter = new Vector4(originalCenterPoint.x, originalCenterPoint.y, originalCenterPoint.z, 1);
        centerPoint = pastTs * tempPointCenter;

        // Set cannon point
        Vector4 tempPoint = new Vector4(originalCannonPoint.x, originalCannonPoint.y, originalCannonPoint.z, 1);
        cannonPoint = pastTs * tempPoint;

        Vector4 tempPointCamera = new Vector4(originalCameraPoint.x, originalCameraPoint.y, originalCameraPoint.z, 1);
        cameraPoint = pastTs * tempPointCamera;
        mainCamera.transform.position = cameraPoint;
        mainCamera.transform.LookAt(cannonPoint);
    }
}
