using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeCharacter : MonoBehaviour
{
    Vector3 panelLocation;
    public float percentThreshold = 0.2f;
    public float easing = 0.5f;
    public int totalPages = 2;
    public int currentPage = 1;

    public GameObject one;
    public GameObject two;

    Vector3 startPos;
    Vector3 dragPos;
    float width;
    float difference;

    bool isDrag = false;

    // Start is called before the first frame update
    void Start()
    {
        startPos = new Vector3();
        //width = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x - Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        width = two.transform.position.x - one.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            panelLocation = transform.position;
        }

        if(Input.GetMouseButton(0))
        {
            dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            difference = startPos.x - dragPos.x;
            transform.position = new Vector3(panelLocation.x - difference , panelLocation.y, panelLocation.z);
        }

        if(Input.GetMouseButtonUp(0))
        {
            dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            difference = startPos.x - dragPos.x;
            float percentage = difference / width;

            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector3 newLocation = panelLocation;
                if (percentage > 0 && currentPage < totalPages)
                {
                    currentPage++;
                    newLocation += new Vector3(-width, 0, 0);
                }
                else if(percentage < 0 && currentPage > 1)
                {
                    currentPage--;
                    newLocation += new Vector3(width, 0, 0);
                }
                StartCoroutine(SmoothMove(transform.position, newLocation, easing));
                panelLocation = newLocation;
            }
            else
            {
                StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
            }
        }

        //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + different, transform.position.y, transform.position.z), Time.deltaTime);
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
