using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    float CANNON_ANGLE_STEP = 2.0f;
    float CANNON_ANGLE_START = 30.0f;
    float CANNON_ANGLE_END = 60.0f;

    float CANNON_VEL_STEP = 0.5f;
    float CANNON_VEL_START = 1.0f;
    float CANNON_VEL_END = 5.0f;

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
    public float cannonAngle;
    public float cannonVelocity;

    // Bullet
    private BulletController bullet;
    private WeaponType[] weapons;
    public WeaponType currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); 
        speed = 1.0f;
        rotationSpeed = -15.0f;
        float longitud = GetComponent<MeshFilter>().mesh.bounds.size.z;
        collisionRadius = longitud;
        cannonAngle = CANNON_ANGLE_START;
        cannonVelocity = CANNON_VEL_START;
        weapons = WeaponType.GetAllWeapons();
        currentWeapon = weapons[0];

        pastTs = Matrix4x4.identity;
        centerPoint = Vector3.zero;
        cannonPoint = new Vector3(0, 1.0f, 0.5f);
        cameraPoint = new Vector3(0, 3.0f, -20.0f);
        originalCenterPoint = centerPoint;
        originalCannonPoint = cannonPoint;
        originalCameraPoint = cameraPoint;

        mainCamera.transform.position = cameraPoint;
        mainCamera.transform.LookAt(cannonPoint);
        mainCamera.fieldOfView = 30.0f;

        Mesh m = GetComponent<MeshFilter>().mesh;

        // Can this be changed to originals = m.vertices;
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
                if (cannonAngle + CANNON_ANGLE_STEP <= CANNON_ANGLE_END)
                {
                    cannonAngle += CANNON_ANGLE_STEP;
                }
                Debug.Log("Changed bullet vertical angle ++ to: " + cannonAngle);
            } else if (Input.GetKeyDown(KeyCode.Minus))
            {
                if (cannonAngle - CANNON_ANGLE_STEP >= CANNON_ANGLE_START)
                {
                    cannonAngle -= CANNON_ANGLE_STEP;
                }
                Debug.Log("Changed bullet vertical angle -- to: " + cannonAngle);
            }
        } else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Started shooting.");
            Shoot(currentWeapon);
        } else if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals))
        {
            if (cannonVelocity + CANNON_VEL_STEP <= CANNON_VEL_END)
            {
                cannonVelocity += CANNON_VEL_STEP;
            }
            Debug.Log("Changed initial bullet velocity ++ to: " + cannonVelocity);
        } else if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (cannonVelocity - CANNON_VEL_STEP >= CANNON_VEL_START)
            {
                cannonVelocity -= CANNON_VEL_STEP;
            }
            Debug.Log("Changed initial bullet velocity -- to: " + cannonVelocity);
        } else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Switched to weapon 1");
            currentWeapon = weapons[0];
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Switched to weapon 2");
            currentWeapon = weapons[1];
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Switched to weapon 3");
            currentWeapon = weapons[2];
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Switched to weapon 4");
            currentWeapon = weapons[3];
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Debug.Log("Switched to weapon 5");
            currentWeapon = weapons[4];
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
        if (bullet == null && weapon.ammo > 0)
        {
            bullet = gameObject.AddComponent<BulletController>();
            bullet.mass = 10.0f;
            bullet.r = 0.2f * currentWeapon.size;
            bullet.restitution = 0.00001f;
            bullet.cpos = cannonPoint;
            bullet.prev = bullet.cpos;
            bullet.colliding = false;
            bullet.SetUp(GetCannonForce(weapon.velocity), currentWeapon.color);
            weapon.ammo -= 1;
        }
    }

    // Dividing cannon force in its three components.
    Vector3 GetCannonForce(float mag) {
        float tankRot = this.tankYRotation * Mathf.Deg2Rad;
        float cannonRot = this.cannonAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(tankRot) * mag * cannonVelocity;
        float z = Mathf.Cos(tankRot) * mag * cannonVelocity;
        float y = Mathf.Sin(cannonRot) * mag;
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
                    Debug.Log("Collided with tank with damage: " + currentWeapon.damage);
                }
                if (bullet.timesBounced >= 2)
                {
                    Debug.Log("Bullet destroyed");
                    Destroy(bullet.sphere);
                    Destroy(bullet);
                    bullet = null;
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

public class WeaponType
{

    public float damage;
    public int ammo;
    public float velocity;
    public float size;
    public Color color;
    public WeaponTypeEnum type;


    public WeaponType(float damage, int ammo, float velocity, float size, Color color, WeaponTypeEnum type)
    {
        this.damage = damage;
        this.ammo = ammo;
        this.velocity = velocity;
        this.size = size;
        this.color = color;
        this.type = type;
    }

    public static WeaponType[] GetAllWeapons()
    {
        WeaponType Weapon1 = new WeaponType(20, 1000, 40.0f, 1.0f, Color.white, WeaponTypeEnum.Type1);
        WeaponType Weapon2 = new WeaponType(40, 10, 38.0f, 1.4f, Color.yellow, WeaponTypeEnum.Type2);
        WeaponType Weapon3 = new WeaponType(60, 5, 36.0f, 1.8f, Color.red, WeaponTypeEnum.Type3);
        WeaponType Weapon4 = new WeaponType(80, 3, 34.0f, 2.2f, Color.blue, WeaponTypeEnum.Type4);
        WeaponType Weapon5 = new WeaponType(100, 1, 32.0f, 2.6f, Color.black, WeaponTypeEnum.Type5);
        return new WeaponType[] {Weapon1, Weapon2, Weapon3, Weapon4, Weapon5};
    }
}

public enum WeaponTypeEnum
{
    Type1,
    Type2,
    Type3,
    Type4,
    Type5,
}
