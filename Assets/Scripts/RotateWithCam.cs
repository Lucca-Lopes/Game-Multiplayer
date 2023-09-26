using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCam : MonoBehaviour
{
    public float sensY;
    private float yFloat = 0;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        yFloat += Input.GetAxis("Mouse X") * sensY;
        transform.rotation = Quaternion.Euler(0, yFloat, 0);
    }
}
