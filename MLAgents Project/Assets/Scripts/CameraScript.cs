using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [SerializeField] private GameObject FollowedCar;


    public float smoothSpeed = 5.0f;

    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = new Vector3(0, 2, -5); 
        Screen.SetResolution(1920, 1080, false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (FollowedCar != null)
        {
            Quaternion rotation = Quaternion.Euler(FollowedCar.transform.eulerAngles.x, FollowedCar.transform.eulerAngles.y, 0);
            Vector3 desiredPosition = FollowedCar.transform.position + rotation * offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;

            transform.LookAt(FollowedCar.transform.position);
        }

    }
}
