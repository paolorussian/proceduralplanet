using UnityEngine;
using System.Collections;

public class KeyboardScript : MonoBehaviour {



    public float speed = 5f;
    //float rollSpeed = 0.03f;
    float rotSpeed = 0.5f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {



        /*
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * speed, Space.World);

        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.forward * speed, Space.World);

        if (Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.back * speed, Space.World);

        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * speed, Space.World);

        if (Input.GetKey(KeyCode.R))
            transform.Translate(Vector3.up * speed, Space.World);

        if (Input.GetKey(KeyCode.F))
            transform.Translate(Vector3.down * speed, Space.World);
          */

        Vector3 targetDirection = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0.0f);
        transform.Rotate(targetDirection);

        if (Input.GetKey(KeyCode.Space)) {
            targetDirection = transform.TransformDirection(targetDirection);
            //targetDirection.y = 0.0f;
            transform.Translate(transform.forward * speed, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            targetDirection = transform.TransformDirection(targetDirection);
            //targetDirection.y = 0.0f;
            transform.Translate(-transform.forward * speed, Space.World);
        }


        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Vector3.back * rotSpeed);

        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Vector3.forward * rotSpeed);

        //transform.Translate(Vector3.forward * speed, Space.World);
        //transform.Translate(Vector3.forward * speed, Space.World);
        //transform.m .Move(moveDirection * Time.deltaTime);

        //  transform.Translate(Vector3.forward * Time.deltaTime);
        //  transform.Translate(Vector3.up * Time.deltaTime, Space.World);

    }
}
