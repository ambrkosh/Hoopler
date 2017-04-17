using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hoop : MonoBehaviour {
    public Text count;
    public int hitCount = 0;

    // Use this for initialization
    void Start () {
        count.text = getHitCount();
    }
	
	// Update is called once per frame
	void Update () {
        count.text = getHitCount();
    }

    private void OnTriggerEnter(Collider other) {
        hitCount++;
    }

    private string getHitCount() {
        return "Basket(s): " + hitCount;
    }
}
