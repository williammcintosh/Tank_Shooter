using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanManagerScript : MonoBehaviour
{
    public GameObject [] movingObjects;
    public Vector3 deathBarSize;
    public LayerMask whatIsTank;
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < movingObjects.Length; i++) {
            Transform myTrans = movingObjects[i].transform;
            Vector3 rightWard = myTrans.TransformDirection(Vector3.forward);
            Vector3 startPos = myTrans.position;
            Vector3 destiPos = myTrans.position+Vector3.forward;
            movingObjects[i].transform.position = Vector3.Lerp(startPos, destiPos, Time.deltaTime * 0.5f);
        }
        KillTank();
    }
    public void KillTank()
    {
        Collider [] hitColliders = Physics.OverlapBox(movingObjects[1].transform.position+(Vector3.back*6), deathBarSize, Quaternion.identity, whatIsTank);
        for (int i = 0; i < hitColliders.Length; i++) {
            if (hitColliders[i].gameObject.TryGetComponent(out TankMoveMentScript tankScript)) {
                tankScript.MakeBigBoom();
            }
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(movingObjects[1].transform.position+(Vector3.back*6), deathBarSize);
    }
}
