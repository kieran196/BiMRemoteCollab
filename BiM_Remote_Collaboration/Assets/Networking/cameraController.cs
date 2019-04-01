using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class cameraController : NetworkBehaviour {

    public Camera cam;
    public Camera camRenderPerspective;
    public Canvas canvas;

    public Canvas canvas2;

    // Update is called once per frame
    void Update() {
        if (canvas2 != null && !canvas2.GetComponent<Canvas>().enabled && this.transform.parent.GetComponent<NetworkBehaviour>().isLocalPlayer) {
            canvas2.GetComponent<Canvas>().enabled = true;
        }
        if(isLocalPlayer && !cam.enabled && !canvas.GetComponent<Canvas>().enabled) {
            print("Enabled camera for:" + this.name);
            cam.enabled = true;
            canvas.GetComponent<Canvas>().enabled = true;
        }
        if(camRenderPerspective != null && isLocalPlayer && !camRenderPerspective.enabled) {
            camRenderPerspective.enabled = true;
        }
    }
}
