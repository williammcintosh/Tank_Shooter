using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerScript : MonoBehaviour
{
    [Header("PLAYER")]
    public GameObject thePlayer;
    TankMoveMentScript playerScript;
    [Header("SPAWNING OBJECTS")]
    public bool canMakeAnNewItem = true;
    float waitTime = 5f;
    public GameObject[] objects;
    [Header("SPAWNING PLANES")]
    public bool theresAnEdge;
    public GameObject prefabPlane;
    public bool canMakeANewPlane = true;
    public LayerMask whatIsGround;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = thePlayer.GetComponent<TankMoveMentScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMakeAnNewItem)
            StartCoroutine(MakeObject());
        theresAnEdge = TheresAnEdge();
        if (theresAnEdge && canMakeANewPlane)
            StartCoroutine(MakePlane());
    }
    IEnumerator MakeObject()
    {
        canMakeAnNewItem = false;
        if (playerScript.killCount %10 == 0 && playerScript.killCount >0) 
            waitTime -= 1f;
        yield return new WaitForSeconds(waitTime);
        int selectedItem = Random.Range(0, objects.Length);
        int rollTheDie = Random.Range(-5,5); 
        Vector3 location = this.transform.TransformDirection(Vector3.right);
        GameObject nextObject = Instantiate(objects[selectedItem], this.transform.position+location*rollTheDie, Quaternion.identity) as GameObject;
        TowerCannonScript objScript = nextObject.GetComponent<TowerCannonScript>();
        objScript.Init(thePlayer);
        canMakeAnNewItem = true;
        yield return null;
    }
    public bool TheresAnEdge()
    {
        Vector3 forward = this.transform.TransformDirection(Vector3.forward)*10f;
        Vector3 up = this.transform.TransformDirection(Vector3.up);
        return Physics.Raycast(this.transform.position+forward+up, Vector3.down, 2f, whatIsGround);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 1);
        Vector3 forward = this.transform.TransformDirection(Vector3.forward)*10f;
        Vector3 up = this.transform.TransformDirection(Vector3.up);
        Gizmos.DrawRay(this.transform.position+forward+up, Vector3.down*2f);
    }
    public IEnumerator MakePlane()
    {
        canMakeANewPlane = false;
        Vector3 forward = this.transform.TransformDirection(Vector3.forward)*35f;
        Instantiate(prefabPlane, this.transform.position+forward, Quaternion.identity);
        yield return new WaitForSeconds(5f);
        canMakeANewPlane = true;
        yield return null;
    }
}
