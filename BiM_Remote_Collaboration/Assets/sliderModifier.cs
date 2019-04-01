using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderModifier : MonoBehaviour {

    private Transform model;

    public void Start() {
        model = GameObject.FindGameObjectWithTag("World").transform;
    }

    public void sliderChanged() {

        float scale = GetComponent<Slider>().value / 10f;
        model.localScale = new Vector3(scale, scale, scale);
        print("Change scale");
    }
}
