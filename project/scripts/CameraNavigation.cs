using UnityEngine;
using System.Collections;

public class CameraNavigation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    float smoothScroll = 0;
    public Transform rotationPoint;
    public Camera camera1;

    void Update () {
        float horizontalinput = Input.GetAxis("Horizontal");
        float verticalinput = Input.GetAxis("Vertical");
        float scrollinput = Input.GetAxis("Mouse ScrollWheel") * 5000;
        float speedRelativeToHeight = transform.position.y / 30;
        smoothScroll = Mathf.Lerp(smoothScroll, scrollinput, Time.deltaTime * 10);

        //if( transform.position.y/5 < 30 ){
        //    camera1.fieldOfView = 75f - (transform.position.y / 5);
        //}
        

        float timeAndSpace = Time.deltaTime * speedRelativeToHeight * 100;

        {
            //transform.Translate(horizontalinput * timeAndSpace, 0, verticalinput * timeAndSpace, Space.World);
            
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            if (verticalinput > 0 && transform.eulerAngles.x < 80)
            {
                transform.Translate(direction * timeAndSpace * verticalinput, Space.World);
            }else if (verticalinput < 0)
            {
                transform.Translate(direction * timeAndSpace * verticalinput, Space.World);
            }

            transform.Translate(horizontalinput * timeAndSpace, 0, 0);

            if (transform.position.y >= 5 && smoothScroll > 0 )
            {
                transform.Translate(0, 0, smoothScroll * Time.deltaTime * speedRelativeToHeight);
            }
            if (smoothScroll < 0)
            {
                transform.Translate(0, 0, smoothScroll * Time.deltaTime * speedRelativeToHeight);
            }

            //keeps camera above ground
            if (transform.position.y < 5)
            {
                transform.Translate(0, (transform.position.y - 5) * -1, 0);
            }
            
        }

        //Cursor pan
        var m = Input.mousePosition;

        if(m.x < 5)
        {
            transform.Translate(-1 * timeAndSpace, 0, 0);
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.Translate(direction * (-(Screen.height / 2 - m.y) / Screen.height * timeAndSpace), Space.World);
        }

        else if (m.x > Screen.width - 5)
        {
            transform.Translate(1 * timeAndSpace, 0, 0);
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.Translate(direction * (-(Screen.height / 2 - m.y) / Screen.height * timeAndSpace), Space.World);
        }

        else if (m.y > Screen.height - 5)
        {
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.Translate(-(Screen.width / 2 - m.x) / Screen.width * timeAndSpace, 0, 0);
            transform.Translate(direction * timeAndSpace * 1, Space.World);
        }

        else if (m.y < 5)
        {
            Vector3 direction = transform.forward;
            direction.y = 0;
            direction.Normalize();
            transform.Translate(-(Screen.width / 2 - m.x) / Screen.width * timeAndSpace, 0, 0);
            transform.Translate(direction * timeAndSpace * -1, Space.World);
        }

        //camera rotation
        
        if (Input.GetMouseButtonDown(2))
        {
            Cursor.lockState = CursorLockMode.Locked;

            Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
            // create a plane at 0,0,0 whose normal points to +Y:
            Plane hPlane = new Plane(Vector3.up, Vector3.zero);
            // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
            float distance = 0;
            // if the ray hits the plane...
            if (hPlane.Raycast(ray, out distance))
            {
                // get the hit point:
                rotationPoint.transform.position = ray.GetPoint(distance);
            }
        }
        if (Input.GetMouseButton(2))
        {
            transform.Translate(Vector3.right * Input.GetAxis("Mouse X") * Time.deltaTime * 150 * speedRelativeToHeight);

            if (Input.GetAxis("Mouse Y") > 0 && transform.eulerAngles.x < 80)
            {
                transform.Translate(Vector3.up * Input.GetAxis("Mouse Y") * Time.deltaTime * 200 * speedRelativeToHeight);
            }else if(Input.GetAxis("Mouse Y") < 0){
                transform.Translate(Vector3.up * Input.GetAxis("Mouse Y") * Time.deltaTime * 200 * speedRelativeToHeight);
            }

            transform.LookAt(rotationPoint);
        }
        else{
            updateRotationPoint();
        }

        if (Input.GetMouseButtonUp(2))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void updateRotationPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // create a plane at 0,0,0 whose normal points to +Y:
        Plane hPlane = new Plane(Vector3.up, Vector3.zero);
        // Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
        float distance = 0;
        // if the ray hits the plane...
        if (hPlane.Raycast(ray, out distance))
        {
            // get the hit point:
            rotationPoint.transform.position = ray.GetPoint(distance);
        }
    }
}
