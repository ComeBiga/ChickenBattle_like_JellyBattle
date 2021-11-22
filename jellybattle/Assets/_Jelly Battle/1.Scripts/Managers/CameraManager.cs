using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public float offsetX = 0f;
    public float offsetY = 1f;
    public float mobileSize = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (KeyBoard.instance.mobileScreen)
        {
            transform.position = new Vector3(offsetX, offsetY, transform.position.z);
            GetComponent<Camera>().orthographicSize = mobileSize;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
