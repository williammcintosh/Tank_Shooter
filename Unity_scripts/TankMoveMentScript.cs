using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TankMoveMentScript : MonoBehaviour
{
    [Header("KILL COUNT")]
    public TextMeshProUGUI counter;
    public int killCount = 0;
    [Header("SELF-DESTRUCT")]
    public ParticleSystem prefabBoom;
    public LayerMask whatAreBaddieBullets;
    [Header("BULLET")]
    public GameObject muzzlePos;
    public float bulletSpeed;
    public GameObject prefabBullet;
    public GameObject turret;
    private float angle = 0f;
    private float angleAdj = 30f;
    [Header("MOVEMENT")]
    private CharacterController controller;
    private Vector3 playerVelocity;
    public bool groundedPlayer;
    private float playerSpeed = 2.0f;
    [Header("JUMP HEIGHT")]
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    public float groundCheckLength = 1.1f;
    public LayerMask whatIsGround;
    Dictionary<int , int> angleDict;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        CreateAngleDict();
    }

    void Update()
    {
        UpdateCounter();
        groundedPlayer = IsGrounded();
        Gravity();
        Jump();
        Movement();
        FollowMouse();
        Shoot();
        if (GotHit()) {
            MakeBigBoom();
        }
    }
    public void Gravity()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    public void Jump()
    {
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }
    }
    public void Movement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
    public bool IsGrounded()
    {
        Vector3 upwards = this.transform.TransformDirection(Vector3.up);
        return Physics.Raycast(this.transform.position+upwards, Vector3.down, groundCheckLength, whatIsGround);
    }
    public void Shoot()
    {
        if (Input.GetMouseButtonDown(0)){
            SingleShot();
        }
    }
    public void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayCastHit;
        if (Physics.Raycast(ray, out rayCastHit, Mathf.Infinity))
            CalculateTargetPosition(rayCastHit.point);
    }
    void CalculateTargetPosition(Vector3 hit) {
        Vector3 mouse_ScreenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float y_total = hit.y - mouse_ScreenToWorld.y;                          // calculating the total y difference
        float newY = (y_total - (hit.y - turret.transform.position.y));                // calculating the difference between hit.y and player's y position ...
                                                                                // ... and subtracting it from the total y difference
        float factor = newY / y_total;                                          // multiplier in order to adjust the original length and reach the target position
        Vector3 targetPos = mouse_ScreenToWorld + ((hit - mouse_ScreenToWorld) * factor); // start of at the starting point and add the adjusted directional vector
        turret.transform.LookAt(targetPos);
    }
    /*
    public void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, maxDistance: 1000.0f)) {
            turret.transform.LookAt(hit.point);
        }
    }
    */
    public void SingleShot()
    {
        GameObject myBullet = Instantiate(prefabBullet, muzzlePos.transform.position, Quaternion.identity) as GameObject;
        Rigidbody bulletRB = myBullet.GetComponent<Rigidbody>();
        Vector3 forward = muzzlePos.transform.TransformDirection(Vector3.forward);
        bulletRB.AddForce(forward * bulletSpeed);
        KillTimer(myBullet);
    }
    public bool GotHit()
    {
        Collider[] hitColliders = Physics.OverlapBox(this.transform.position+Vector3.up, (transform.localScale / 2), Quaternion.identity, whatAreBaddieBullets);
        for (int i = 0; i < hitColliders.Length; i++) {
            Destroy(hitColliders[i].gameObject);
        }
        return (hitColliders.Length > 0);
    }
    public void MakeBigBoom()
    {
        StartCoroutine(MakeBigBoomRoutine());
    }
    public IEnumerator MakeBigBoomRoutine()
    {
        ParticleSystem myBigBoom = Instantiate(prefabBoom, this.transform.position, Quaternion.identity) as ParticleSystem;
        yield return null;
        Destroy(gameObject);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 upwards = this.transform.TransformDirection(Vector3.up);
        Gizmos.DrawRay(this.transform.position+upwards, Vector3.down*groundCheckLength);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position+Vector3.up, transform.localScale);
        Gizmos.color = Color.black;
        Vector3 forward = turret.transform.TransformDirection(Vector3.forward)*100f;
        Gizmos.DrawRay(turret.transform.position, turret.transform.position + forward);
    }
    public IEnumerator KillTimer(GameObject obj)
    {
        yield return new WaitForSeconds(10f);
        Destroy(obj);
        yield return null;
    }
    public void CreateAngleDict()
    {
        angleDict = new Dictionary <int, int>() {
            {0,55},
            {15,69},
            {30,71},
            {45,69},
            {60,62},
            {75,54},
            {90,45},
            {105,37},
            {120,29},
            {135,23},
            {150,20},
            {165,24},
            {180,33},
            {-180,33},
            {-165,40},
            {-150,57},
            {-135,60},
            {-120,57},
            {-105,51},
            {-90,45},
            {-75,39},
            {-60,34},
            {-45,30},
            {-30,31},
            {-15,40},
        };
    }
    public void UpdateCounter()
    {
        counter.text = killCount.ToString();
    }
}
