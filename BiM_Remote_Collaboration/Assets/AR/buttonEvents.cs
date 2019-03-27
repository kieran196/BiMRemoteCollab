using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class buttonEvents : MonoBehaviour {

    public Raycast raycast;
    public GameObject itemHolder;
    private bool copiedObject = false;
    private bool movedObject = false;
    private GameObject clonedObj;
    private List<GameObject> clonedObjects = new List<GameObject>();
    public GameObject movedSelectedObject;
    public Toggle selectionToggle;

    public Text copyText;
    public Text pickupText;
    public Transform oldParent;
    public List<GameObject> selectableObjectList;
    private bool multiObjectsOn = false;

    public Text selectableObjects;

    public void handleSelection() {
        if (selectionToggle.isOn == true && raycast.selectedObject != null) {
            if(!selectableObjectList.Contains(raycast.selectedObject)) {
                selectableObjectList.Add(raycast.selectedObject);
                selectableObjects.text += raycast.selectedObject.name+"\n";
            }
        } else if(selectionToggle.isOn == false && raycast.selectedObject != null) {
            if(selectableObjectList.Count == 0) {
                selectableObjectList.Add(raycast.selectedObject);
                selectableObjects.text = raycast.selectedObject.name + "\n";
            } else if(selectableObjectList.Count == 1) {
                selectableObjectList[0] = raycast.selectedObject;
                selectableObjects.text = raycast.selectedObject.name + "\n";
            }
        }
    }

    public void resetList() {
        selectableObjectList.Clear();
        selectableObjects.text = "";
    }

    public void resetSelectableObjects() {
        if(selectionToggle.isOn == false) {
            selectableObjectList.Clear();
            selectableObjects.text = "";
            multiObjectsOn = false;
        } else {
            multiObjectsOn = true;
        }
    }

    public void deleteObject() {
        if (raycast.selectedObject != null && multiObjectsOn == false) {
            print("Deleting object:" + raycast.selectedObject.name);
            //Destroy(raycast.selectedObject.gameObject);
            raycast.selectedObject.SetActive(false);
        } else if (selectableObjectList.Count > 0 && multiObjectsOn == true) {
            foreach (GameObject obj in selectableObjectList) {
                Destroy(obj);
            }
            resetList();
        }
    }

    public void moveObject() {
        if(multiObjectsOn == false) {
            if(movedObject == true && movedSelectedObject != null) {
                print("Dropped object:" + clonedObj);
                movedObject = false;
                if(oldParent.gameObject.activeInHierarchy == true) {
                    oldParent.gameObject.SetActive(true);
                }
                movedSelectedObject.transform.SetParent(oldParent);
                pickupText.text = "Pickup";
            } else if(raycast.selectedObject != null && raycast.selectedObject.activeInHierarchy == true && movedObject == false) {
                movedSelectedObject = raycast.selectedObject;
                print("Picked up object:" + raycast.selectedObject);
                //oldParent = raycast.selectedObject.transform.parent;
                movedSelectedObject.transform.SetParent(itemHolder.transform);
                movedObject = true;
                pickupText.text = "Drop";
            }
        } else if (multiObjectsOn == true && selectableObjectList.Count > 0) {
            if (movedObject == true) {
                foreach(GameObject obj in selectableObjectList) {
                    obj.transform.SetParent(oldParent);
                }
                pickupText.text = "Pickup";
                movedObject = false;
            } else if (movedObject == false) {
                foreach(GameObject obj in selectableObjectList) {
                    //oldParent = obj.transform.parent;
                    obj.transform.SetParent(itemHolder.transform);
                }
                movedObject = true;
                pickupText.text = "Drop";
            }
        }
    }

    public void copyObject() {
        if(multiObjectsOn == false) {
            if(copiedObject == true && clonedObj != null) {
                print("Dropped object:" + clonedObj);
                copiedObject = false;
                clonedObj.transform.SetParent(oldParent);
                copyText.text = "Copy (Add)";
            } else if(raycast.selectedObject != null && raycast.selectedObject.activeInHierarchy == true && copiedObject == false) {
                print("Picked up object:" + raycast.selectedObject);
                //oldParent = raycast.selectedObject.transform.parent;
                clonedObj = Instantiate(raycast.selectedObject, raycast.selectedObject.transform.position, Quaternion.identity) as GameObject;
                clonedObj.GetComponent<Renderer>().material = new Material(clonedObj.GetComponent<Renderer>().material);
                //clonedObj.transform.localScale = new Vector3(clonedObj.transform.lossyScale.x, clonedObj.transform.lossyScale.y, clonedObj.transform.lossyScale.z);
                clonedObj.tag = "Untagged";
                clonedObj.transform.SetParent(itemHolder.transform);
                //clonedObj.transform.localScale = new Vector3(clonedObj.transform.lossyScale.x, clonedObj.transform.lossyScale.y, clonedObj.transform.lossyScale.z);
                if (clonedObj.tag == "2DObject") {
                    clonedObj.transform.localScale = new Vector3(1f, 1f, 1f);
                } else {
                    clonedObj.transform.localScale = new Vector3(clonedObj.transform.lossyScale.x * 5, clonedObj.transform.lossyScale.y * 5, clonedObj.transform.lossyScale.z * 5);
                }
                clonedObj.transform.eulerAngles = raycast.selectedObject.transform.eulerAngles;
                copiedObject = true;
                copyText.text = "Copy (Drop)";
            }
        } else if (multiObjectsOn == true) {
            if (copiedObject == true) {
                foreach(GameObject obj in clonedObjects) {
                    obj.transform.SetParent(oldParent);
                }
                copyText.text = "Copy (Add)";
                clonedObjects.Clear();
                copiedObject = false;
            } else if (copiedObject == false) {
                for(int i=0; i<selectableObjectList.Count; i++) {
                    GameObject obj = selectableObjectList[i];
                    GameObject clonedObjTemp;
                    clonedObjTemp = Instantiate(obj, obj.transform.position, Quaternion.identity) as GameObject;
                    clonedObjTemp.transform.SetParent(itemHolder.transform);
                    clonedObjTemp.transform.localScale = new Vector3(clonedObjTemp.transform.localScale.x * 5, clonedObjTemp.transform.localScale.y * 5, clonedObjTemp.transform.localScale.z * 5);
                    clonedObjTemp.transform.eulerAngles = obj.transform.eulerAngles;
                    clonedObjects.Add(clonedObjTemp);
                    //oldParent = obj.transform.parent;
                }
                copiedObject = true;
                copyText.text = "Copy (Drop)";
            }
        }
    }

    private bool interactionEnabled = false;
    public GameObject menu1;
    public GameObject menu2;
    public void enableMenus() {
        interactionEnabled = !interactionEnabled;
        menu1.SetActive(!interactionEnabled);
        menu2.SetActive(interactionEnabled);
    }

    public void increaseScale() {
        if(multiObjectsOn == false) {
            if(raycast.selectedObject != null) {
                Vector3 scale = raycast.selectedObject.transform.localScale;
                scale /= 9f;
                raycast.selectedObject.transform.localScale += new Vector3(scale.x, scale.y, scale.z);
            }
        } else if (multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    Vector3 scale = obj.transform.localScale;
                    scale /= 9f;
                    obj.transform.localScale += new Vector3(scale.x, scale.y, scale.z);
                }
            }
        }
    }

    public void decreaseScale() {
        if(multiObjectsOn == false) {
            if(raycast.selectedObject != null) {
                Vector3 scale = raycast.selectedObject.transform.localScale;
                scale /= 9f;
                raycast.selectedObject.transform.localScale -= new Vector3(scale.x, scale.y, scale.z);
            }
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    Vector3 scale = obj.transform.localScale;
                    scale /= 9f;
                    obj.transform.localScale -= new Vector3(scale.x, scale.y, scale.z);
                }
            }
        }
    }

    public void moveRight() {
        if(multiObjectsOn == false && raycast.selectedObject != null) {
            raycast.selectedObject.transform.localEulerAngles += new Vector3(0f, 0f, 5f);
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    obj.transform.localEulerAngles += new Vector3(0f, 0f, 5f);
                }
            }
        }
    }

    public void moveRightUp() {
        if(multiObjectsOn == false && raycast.selectedObject != null) {
            raycast.selectedObject.transform.localEulerAngles += new Vector3(5f, 0f, 0f);
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    obj.transform.localEulerAngles += new Vector3(5f, 0f, 0f);
                }
            }
        }
    }

    public void moveRightDown() {
        if(multiObjectsOn == false && raycast.selectedObject != null) {
            raycast.selectedObject.transform.localEulerAngles += new Vector3(-5f, 0f, 0f);
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    obj.transform.localEulerAngles += new Vector3(-5f, 0f, 0f);
                }
            }
        }
    }

    public void moveLeft() {
        if(multiObjectsOn == false && raycast.selectedObject != null) {
            raycast.selectedObject.transform.localEulerAngles += new Vector3(0f, 0f, -5f);
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    obj.transform.localEulerAngles += new Vector3(0f, 0f, -5f);
                }
            }
        }
    }

    public void moveUp() {
        if(multiObjectsOn == false && raycast.selectedObject != null) {
            raycast.selectedObject.transform.localEulerAngles += new Vector3(0f, -5f, 0f);
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    obj.transform.localEulerAngles += new Vector3(0f, -5f, 0f);
                }
            }
        }
    }

    public void moveDown() {
        if(multiObjectsOn == false && raycast.selectedObject != null) {
            raycast.selectedObject.transform.localEulerAngles += new Vector3(0f, 5f, 0f);
        } else if(multiObjectsOn == true) {
            foreach(GameObject obj in selectableObjectList) {
                if(obj != null) {
                    obj.transform.localEulerAngles += new Vector3(0f, 5f, 0f);
                }
            }
        }
    }

    private bool groupSelectionEnabled = false;
    private bool dragEnabled = false;
    public GameObject selectionGroup;
    Vector3 tempScale;
    Vector3 startPos;

    public void groupSelection() {
        
        if (groupSelectionEnabled == true) {
            groupSelectionEnabled = false;
            dragEnabled = true;
            tempScale = selectionGroup.transform.localScale;
            startPos = selectionGroup.transform.position;
        } else if (groupSelectionEnabled == false && dragEnabled == true) {
            dragEnabled = false;
        } else if(groupSelectionEnabled == false && dragEnabled == false) {
            groupSelectionEnabled = true;
        }
    }

    public bool Contains(LayerMask searchedLayer) {
        foreach (LayerMask layer in layers)
        {
            //print(layer.value + " " + searchedLayer.value);
            if (Mathf.Log(layer.value, 2) == searchedLayer.value)
            {
                return true;
            }
        }
        return false;
    }

    // Use this for initialization
    void Start () {
        getLayers();
        AllSceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        AllInteractableObjects = new List<GameObject>();
        foreach (GameObject obj in AllSceneObjects) {
            if (Contains(obj.layer)) {
                AllInteractableObjects.Add(obj);
                //GameObject measurer = Instantiate(measureObj);
            }
        }
    }
    public GameObject toggleLayer;
    public GameObject layerParent;
    public LayerMask[] layers;
    float yVal = -10;
    public GameObject measureObj;

    void ToggleValueChanged()
    {
        changeLayerState();
        print("Toggle has been changed..");
    }

    public void getLayers() {
        for (int i=0; i<layers.Length;i++) {
            string layer = LayerMask.LayerToName((int)Mathf.Log(layers[i].value, 2));
            GameObject newLayer = Instantiate(toggleLayer);
            newLayer.name = layer;
            newLayer.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChanged();
            });




            newLayer.transform.SetParent(layerParent.transform);
            newLayer.transform.localPosition = new Vector3(30f, yVal, 0f);
            newLayer.GetComponentInChildren<Text>().text = layer;
            yVal -= 10;
        }
    }

    private GameObject[] AllSceneObjects;
    private List<GameObject> AllInteractableObjects;
    public void changeLayerState() {
        string layerName = EventSystem.current.currentSelectedGameObject.name;
        print("Layer name:" + layerName);
        bool active = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
        LayerMask layer = LayerMask.NameToLayer(layerName);
        foreach (GameObject obj in AllInteractableObjects) {
            if (LayerMask.LayerToName(obj.layer) == layerName) {
                print("Object:" + obj.name);
                obj.SetActive(active);
            }
        }
    }

    public GameObject trackedSpace;

    // Update is called once per frame
    void Update () {
        oldParent = trackedSpace.transform;
		if (groupSelectionEnabled == true) {
            selectionGroup.transform.position = Input.mousePosition;
        } if (dragEnabled == true) {
            selectionGroup.transform.localScale = new Vector3(tempScale.x + Vector3.Distance(startPos, Input.mousePosition), tempScale.y + Vector3.Distance(startPos, Input.mousePosition), tempScale.z);
        }
	}
}
