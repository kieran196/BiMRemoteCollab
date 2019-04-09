// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MoveCamera : MonoBehaviour {

    /*
Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
Converted to C# 27-02-13 - no credit wanted.
Simple flycam I made, since I couldn't find any others made public.  
Made simple to use (drag and drop, done) for regular keyboard layout  
wasd : basic movement
shift : Makes camera accelerate
space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/

    float mainSpeed = 50.0f; //regular speed
    float shiftAdd = 150.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 5000.0f; //Maximum speed when holdin gshift
    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;
    private Camera cam;
    private Transform lastSelectedObject;
    private Color oldColor;
    public buttonEventsPC buttonEvents;

    private void Start() {
        cam = GetComponent<Camera>();
    }


    void Update() {
        if (cam.enabled) {
            castRay();
            moveCamera();
        }
    }

    public void castRay() {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            if (Input.GetMouseButtonDown(0)) {
                Debug.Log("You selected the " + hit.transform.name);
                if (lastSelectedObject != null && lastSelectedObject != hit.transform) {
                    //buttonEvents.highlightTool(hit.transform.gameObject);
                    //hit.transform.GetComponent<Renderer>().material.color = lastSelectedObject.GetComponent<Renderer>().material.color;
                    lastSelectedObject.transform.GetComponent<Renderer>().material.color = oldColor;
                }
                lastSelectedObject = (lastSelectedObject == null) ? hit.transform : (lastSelectedObject != hit.transform) ? hit.transform : null;
                oldColor = lastSelectedObject.GetComponent<Renderer>().material.color;
                hit.transform.GetComponent<Renderer>().material.color = Color.yellow;
            }
        }
    }

    private void moveCamera() {
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        if (Input.GetMouseButton(1)) {
            transform.eulerAngles = lastMouse;
        }
        lastMouse = Input.mousePosition;
        //Mouse  camera angle done.  

        //Keyboard commands
        float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (Input.GetKey(KeyCode.LeftShift)) {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        } else {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime;
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.Space)) { //If player wants to move on X and Z axis only
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        } else {
            transform.Translate(p);
        }
    }

    private Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S)) {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A)) {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }

}