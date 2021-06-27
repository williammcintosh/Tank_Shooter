using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCannonScript : MonoBehaviour
{
    [Header("SELF-DESTRUCT")]
    bool ImDead = false;
    public ParticleSystem prefabBoom;
    [Header("HEALTH")]
    public float health = 20f;
    [Header("FIRING")]
    public float rotateSpeed = 5f;
    public float rateOfFire = 5f;
    public LayerMask whatAreBullets;
    public GameObject muzzlePos;
    public bool canFire = false;
    public float bulletSpeed;
    public GameObject prefabBullet;
    public GameObject myTurret;
    [Header("PLAYER")]
    public GameObject thePlayer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitialCanFire());
    }
    public void Init(GameObject thisIsThePlayer)
    {
        thePlayer = thisIsThePlayer;
    }
    // Update is called once per frame
    void Update()
    {
        if (!ImDead && thePlayer != null) {
            TurretFollowPlayer(); 
            if (canFire) {
                StartCoroutine(FireCannon());
            }
            if (GotHit()) {
                Flash();
                health -= 10;
            }
            if (health <= 0 && !ImDead) {
                ImDead = true;
                SelfDestruct();
            }
        }
    }
    public void TurretFollowPlayer()
    {
        Vector3 targetPoint = thePlayer.transform.position - myTurret.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint, Vector3.up);
        myTurret.transform.rotation = Quaternion.Slerp(myTurret.transform.rotation, targetRotation, Time.deltaTime);
    }
    public IEnumerator FireCannon()
    {
        canFire = false;
        yield return new WaitForSeconds(rateOfFire);
        if (!ImDead) {
            GameObject myBullet = Instantiate(prefabBullet, muzzlePos.transform.position, Quaternion.identity) as GameObject;
            Rigidbody bulletRB = myBullet.GetComponent<Rigidbody>();
            Vector3 forward = muzzlePos.transform.TransformDirection(Vector3.forward);
            bulletRB.AddForce(forward * bulletSpeed);
            KillTimer(myBullet);        
        }
        canFire = true;
        yield return null;
    }
    public IEnumerator KillTimer(GameObject obj)
    {
        yield return new WaitForSeconds(10f); 
        Destroy(obj);
        yield return null;
    }
    public bool GotHit()
    {
        Collider[] hitColliders = Physics.OverlapBox(this.transform.position, (transform.localScale / 2)*8.5f, Quaternion.identity, whatAreBullets);
        for (int i = 0; i < hitColliders.Length; i++) {
            Destroy(hitColliders[i].gameObject);
        }
        return (hitColliders.Length > 0);
    }
    public void Flash()
    {
        StartCoroutine(WhiteFlash());
    }
    public IEnumerator WhiteFlash()
    {
        MeshRenderer myRenderer = gameObject.GetComponent<MeshRenderer>();  
        MeshRenderer turretRenderer = myTurret.GetComponent<MeshRenderer>();  
        Material mat = myRenderer.materials[0];
        Material turretMat = turretRenderer.materials[0];
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.white);
        turretMat.EnableKeyword("_EMISSION");
        turretMat.SetColor("_EmissionColor", Color.white);
        yield return new WaitForSeconds(0.1f);
        mat.SetColor("_EmissionColor", Color.black);
        turretMat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");
        turretMat.DisableKeyword("_EMISSION");
        yield return null;
    }
    public void SelfDestruct()
    {
        StartCoroutine(SelfDestructTimer());
    }
    public IEnumerator SelfDestructTimer()
    {
        ParticleSystem myBigBoom = Instantiate(prefabBoom, this.transform.position, Quaternion.identity) as ParticleSystem;
        myBigBoom.transform.localScale = Vector3.one * 0.5f;
        MeshRenderer myRenderer = gameObject.GetComponent<MeshRenderer>();  
        Destroy(myRenderer);
        MeshRenderer trRenderer = myTurret.GetComponent<MeshRenderer>();  
        Destroy(trRenderer);
        BoxCollider myBoxCollider = gameObject.GetComponent<BoxCollider>();
        Destroy(myBoxCollider);
        thePlayer.GetComponent<TankMoveMentScript>().killCount += 1;
        yield return new WaitForSeconds(10f);
        Destroy(myBigBoom);
        Destroy(this.gameObject);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale*4.25f);
    }
    public IEnumerator InitialCanFire()
    {
        yield return new WaitForSeconds(40f); 
        canFire = true;
        yield return null;
    }
}
