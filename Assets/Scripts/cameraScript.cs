using UnityEngine;
using System.Collections;


public class cameraScript : MonoBehaviour {

    public Transform flag;
    public float windX=0;
    public float windY=0;
    public float windZ=0;
    public float windXSliderValue;
    public float windYSliderValue;
    public float windZSliderValue;


    // Use this for initialization
    void Start () {
        windXSliderValue = windX;
        windYSliderValue = windY;
        windZSliderValue = windZ;
}
	
    
	// Update is called once per frame
	void Update () {
	
        transform.LookAt(flag);
        if(Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left*50 * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right *50* Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.up * 50 * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.down * 50 * Time.deltaTime);
        }

        windX = windXSliderValue;
        windY = windYSliderValue;
        windZ = windZSliderValue;
    }
    void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(10, 95, 100, 30), "X Wind: " + windX);
        windXSliderValue = GUI.HorizontalSlider(new Rect(100, 90, 100, 30), windXSliderValue, -1.0f, 1.0f);
        GUI.Label(new Rect(10, 125, 100, 30), "Y Wind: " + windY);
        windYSliderValue = GUI.HorizontalSlider(new Rect(100, 130, 100, 30), windYSliderValue, -1.0f, 1.0f);
        GUI.Label(new Rect(10, 165, 100, 30), "Z Wind: " + windZ);
        windZSliderValue = GUI.HorizontalSlider(new Rect(100, 170, 100, 30), windZSliderValue, -1.0f, 1.0f);
    }
}
