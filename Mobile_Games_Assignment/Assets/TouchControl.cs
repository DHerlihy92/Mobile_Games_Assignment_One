using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    #region Touch Control Variables
    float timer;
    const float MAXTAPTIME = 0.2f;
    bool hasMoved = false;
    public bool objSelected = false;
    private float startingFingerAngle;
    private Quaternion startingObjectAngle;
    private Vector3 startingScale;
    private Quaternion startingGyroRotation;
    private float startingDistance;
    private float newFingerDestination;
    public float speed = 0.01f;
    private float dragDistance;
    Controllable tempObject;
    #endregion

    #region Camera Control Variables
    private Vector3 firstpoint; //change type on Vector3
    private Vector3 secondpoint;
    private float xAngle = 0.0f; //angle for axes x for rotation
    private float yAngle = 0.0f;
    private float xAngTemp = 0.0f; //temp variable for angle
    private float yAngTemp = 0.0f;
    private float touchZoomSpeed = 0.1f;
    private float zoomMinBound = 0.1f;
	private float zoomMaxBound = 179.9f;
    Camera myCamera;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        myCamera = GetComponent<Camera>();
        myCamera.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Scale and rotate
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.touches[0];
            Touch touch2 = Input.touches[1];

            if ((touch1.phase == TouchPhase.Began) || (touch2.phase == TouchPhase.Began))
            {
                Vector2 diff = touch2.position - touch1.position;
                startingFingerAngle = Mathf.Atan2(diff.y, diff.x);
                startingObjectAngle = tempObject.transform.rotation;
                startingDistance = Vector2.Distance(touch1.position, touch2.position);
                startingScale = tempObject.transform.localScale;
            }

            if (tempObject)
            {
                Vector2 diff = touch2.position - touch1.position;

                newFingerDestination = Mathf.Atan2(diff.y, diff.x);
                tempObject.transform.rotation = startingObjectAngle * Quaternion.AngleAxis(Mathf.Rad2Deg * (newFingerDestination - startingFingerAngle), myCamera.transform.forward);
                tempObject.transform.localScale = (Vector2.Distance(touch1.position, touch2.position) / startingDistance) * startingScale;
            }
        }
        //3 input touch gesture
        else if(Input.touchCount == 3)
        {
            tempObject.changeColor(Color.red);
        }
        //Drag and Touch
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            timer += Time.deltaTime;

            if (touch.phase == TouchPhase.Began)
            {
                if (tempObject)
                {
                    dragDistance = Vector3.Distance(tempObject.transform.position, myCamera.transform.position);
                }
                timer = 0f;
                hasMoved = false;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                hasMoved = true;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if ((timer < MAXTAPTIME) && !hasMoved)
                {
                    interact(touch);
                }
                else if (hasMoved && tempObject)
                {
                    tempObject.goDefault();
                    tempObject = null;
                }
            }

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (tempObject)
                {
                    Ray dragRay = myCamera.ScreenPointToRay(touch.position);
                    Vector3 newDestination = dragRay.GetPoint(dragDistance);
                    tempObject.latestDragPosition(newDestination);
                }
            }
        }
        //No objected Selected --> Camera Controls
        if (!tempObject)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDelataPosition = Input.GetTouch(0).deltaPosition;
                transform.Translate(-touchDelataPosition.x * speed * Time.deltaTime, -touchDelataPosition.y * speed * Time.deltaTime, 0);
            }

            if (Input.touchCount > 0)
            {

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstpoint = Input.GetTouch(0).position;
                    xAngTemp = xAngle;
                    yAngTemp = yAngle;
                }
                //Move finger by screen
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    secondpoint = Input.GetTouch(0).position;
                    //Mainly, about rotate camera. For example, for Screen.width rotate on 180 degree
                    xAngle = xAngTemp + (secondpoint.x - firstpoint.x) * 180.0f / Screen.width;
                    yAngle = yAngTemp - (secondpoint.y - firstpoint.y) * 90.0f / Screen.height;
                    //Rotate camera
                    myCamera.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
                }
            }
            if (Input.touchCount == 2)
            {
                // get current touch positions
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                // get touch position from the previous frame
                Vector2 tZeroPrevious = touch1.position - touch1.deltaPosition;
                Vector2 tOnePrevious = touch2.position - touch2.deltaPosition;

                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(touch1.position, touch2.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Zoom(deltaDistance, touchZoomSpeed);

            }


        }
    }
    void interact(Touch touch)
    {
        Ray myRay = myCamera.ScreenPointToRay(touch.position);
        RaycastHit infoOnHit;

        if (Physics.Raycast(myRay, out infoOnHit))
        {
            Controllable objHit = infoOnHit.transform.GetComponent<Controllable>();

            if (objHit)
            {
                if (!tempObject)
                {
                    objHit.changeColor(Color.blue);
                    tempObject = objHit;
                }
                else if (tempObject)
                {
                    tempObject.goDefault();
                    tempObject = null;
                }
            }
        }
        else
        {
            if (tempObject)
            {
                tempObject.goDefault();
                tempObject = null;
            }
        }
    }

    void Zoom(float distance, float speed)
    {
        myCamera.fieldOfView += distance * speed;
        myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, zoomMinBound, zoomMaxBound);
    }
}

