using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour {

    public GameObject target = null;
    public bool orbitY = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (transform != null) {

            transform.LookAt(target.transform.position);

            // Vector3 worldPt = target.transform.localToWorldMatrix.MultiplyPoint3x4(target.transform.position);
            //  transform.RotateAround(new Vector3(0f,0f,-100f), Vector3.up, 50*Time.deltaTime);
            /*
            if (orbitY) {
               
            }
            */
            

            if (Input.GetMouseButton(1)) {
                //transform.LookAt(target.transform);
                transform.RotateAround(target.transform.position, Vector3.up, Input.GetAxis("Mouse X") * 3f);
                transform.RotateAround(target.transform.position, Vector3.left, Input.GetAxis("Mouse Y") * 3f);
            }

        }

	
	}
}
