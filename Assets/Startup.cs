using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Startup : MonoBehaviour {
    public Button start; 

	// Use this for initialization
	void Start () {
		start.onClick.AddListener( delegate { startButtonClicked(); } );
	}

	// Update is called once per frame
	void Update () {
		
	}

    void startButtonClicked() {
        GetComponent<Canvas>().enabled = false;
    }
}
