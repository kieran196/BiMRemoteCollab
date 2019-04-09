using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class buttonEventsPC : MonoBehaviour{

    private bool buildingMode = false;
    private Transform oldParent;

    public RuntimeAnimatorController highlightController;

    public void highlightTool(GameObject selectedObj) {
        if (objectIsBimLayer(selectedObj)) {
            if (selectedObj.GetComponent<Animator>() == null) { // Set animator
                Animator animator = selectedObj.AddComponent<Animator>();
                animator.runtimeAnimatorController = highlightController;
            } else { // Animator already exists
                selectedObj.GetComponent<Animator>().enabled = !selectedObj.GetComponent<Animator>().enabled;
            }
        }
    }

    public bool Contains(LayerMask searchedLayer) {
        foreach (LayerMask layer in layers) {
            //print(layer.value + " " + searchedLayer.value);
            if (Mathf.Log(layer.value, 2) == searchedLayer.value) {
                return true;
            }
        }
        return false;
    }

    void Start() {
        trackedSpace = GameObject.FindGameObjectWithTag("trackedSpace");
    }

    public GameObject toggleLayer;
    public GameObject layerParent;
    public LayerMask[] layers;
    float yVal = -10;
    public GameObject measureObj;

    void ToggleValueChanged() {
        changeLayerState();
        print("Toggle has been changed..");
    }

    public void getLayers() {
        char[] ifcTrim = { 'I', 'f', 'c' };
        print("Assigning layers..");
        assignedLayers = true;
        for (int i = 0; i < ImportIFC.itemReferences.Count; i++) {
            string name = ImportIFC.itemReferences[i];
            name = name.TrimStart(ifcTrim);
            print("Layer:" + i + " | " + name);
            GameObject newLayer = Instantiate(toggleLayer);
            newLayer.name = name;
            newLayer.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChanged();
            });
            newLayer.transform.SetParent(layerParent.transform);
            newLayer.transform.localPosition = new Vector3(30f, yVal, 0f);
            newLayer.GetComponentInChildren<Text>().text = name;
            yVal -= 10;
        }
        initializeDropdown();
    }

    public void initializeDropdown() {
        Debug.Log("Initialize dropdown() called");
        Transform firstLayer = layerParent.transform.GetChild(1);
        firstLayer.GetComponentInChildren<Dropdown>().ClearOptions();
        Dropdown.OptionData opt1 = new Dropdown.OptionData();
        opt1.text = "N/A";
        firstLayer.GetComponentInChildren<Dropdown>().options.Add(opt1);
        for (int i = 0; i < ImportIFC.itemReferences.Count; i++) {
            Dropdown.OptionData dropdownOption = new Dropdown.OptionData();
            dropdownOption.text = i.ToString();
            firstLayer.GetComponentInChildren<Dropdown>().options.Add(dropdownOption);
        }
        int count = 1;
        foreach (Transform layer in layerParent.transform) {
            //print("LAYER: " + layer.name + " | " +count);
            if (layer.GetComponentInChildren<Dropdown>() != null) {
                layer.GetComponentInChildren<Dropdown>().options = firstLayer.GetComponentInChildren<Dropdown>().options;
                layer.GetComponentInChildren<Dropdown>().value = count;
                count++;
            }
            print(count);

        }
    }

    public bool objectIsBimLayer(GameObject obj) {
        return (obj != null && obj.transform.parent != null && obj.transform.parent.tag == "World") ? true : false;
    }

    public static readonly float VISUALIZE_TIME = 5;
    private float scaleVal = 0;
    public bool visualizingLayers = false;
    private bool buildComplete = false;
    private int objIndex = 1;

    public void visualizeLayers() {
        visualizingLayers = true;
        if (scaleVal <= 1) {
            scaleVal += Time.deltaTime / VISUALIZE_TIME;
            foreach (GameObject item in ImportIFC.itemList) {
                item.transform.localScale = new Vector3(1f, scaleVal, 1f);
            }
            print("NAME:" + ImportIFC.itemList[objIndex].transform.name + " || " + ImportIFC.itemList[objIndex].GetComponent<MeshCollider>().bounds);
            //ImportIFC.itemList[objIndex].transform.localScale = new Vector3(1f, scaleVal, 1f);
        } else {
            visualizingLayers = false;
            scaleVal = 0;
            objIndex++;
        }
    }

    public void changeLayerState() {
        string name = "Ifc" + EventSystem.current.currentSelectedGameObject.name;
        print("name:" + name);
        bool active = EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>().isOn;
        foreach (GameObject obj in ImportIFC.itemList) {
            string objRef = obj.GetComponent<IFCVariables>() != null ? obj.GetComponent<IFCVariables>().vars[1].value : null;
            if (objRef != null && objRef == name) {
                obj.SetActive(active);
            }
        }
    }

    public void viewDimensions() {
        GameObject button = EventSystem.current.currentSelectedGameObject;
        buildingMode = !buildingMode;
        button.GetComponentInChildren<Text>().text = buildingMode ? "Building Mode\nON" : "Building Mode\nOFF";
        foreach (GameObject obj in ImportIFC.itemList) {
            if (buildingMode) {
                obj.GetComponent<Renderer>().enabled = false;
                foreach (Transform child in obj.transform) {
                    child.GetComponent<Renderer>().enabled = true;
                }
            } else {
                obj.GetComponent<Renderer>().enabled = true;
                foreach (Transform child in obj.transform) {
                    child.GetComponent<Renderer>().enabled = false;
                }
            }
        }
    }

    public GameObject trackedSpace;
    private bool assignedLayers = false;
    // Update is called once per frame
    void Update() {
        if (visualizingLayers) {
            visualizeLayers();
        }
        if (Manager.importComplete && !assignedLayers) {
            getLayers();
        }
        oldParent = trackedSpace.transform;
    }
}
