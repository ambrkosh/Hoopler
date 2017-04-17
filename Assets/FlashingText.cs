using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour {

    public Text text;

    // Use this for initialization
    void Start() {
        text = GetComponent<Text>();
        StartCoroutine(blinkText());
    }

    // Update is called once per frame
    void Update() {

    }

    public IEnumerator blinkText() {
        while (true) {
            yield return new WaitForSeconds(.7f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
            yield return new WaitForSeconds(.5f);
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        }
    }
}
