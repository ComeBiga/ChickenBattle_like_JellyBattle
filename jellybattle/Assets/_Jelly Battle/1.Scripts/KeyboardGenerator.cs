using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardGenerator : MonoBehaviour
{
    public int width, height;

    Map[] func;
    Map[,] map;

    public float cubeSize;
    public float playerSize;

    public GameObject keycap;
    public GameObject[] itemkeycap;
    GameObject curCliked;

    public GameObject player;
    Vector3 nextPos;

    public float waitingTime = 10.0f;

    private void Start()
    {
        func = new Map[width];
        map = new Map[width, height];

        mapInit();
        FuncGenerator();

        InstantiateKeyCap();

        player = Instantiate(player, new Vector3(2, 2), player.transform.rotation);
        nextPos = Vector3.zero;

        InvokeRepeating("WaveMap", 1.0f, waitingTime);
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    WaveMap();

            
        //}

        ClickKey();
    }

    void mapInit()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = new Map(0, new Vector2(-width / 2 + x, -height / 2 + y), keycap);
            }
        }
    }

    void InstantiateKeyCap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x,y].keycap = Instantiate(keycap, map[x, y].position, Quaternion.identity);
            }
        }

        for (int x = 0; x < width; x++)
        {
            func[x].UpdateKeycap();
            func[x].keycap = Instantiate(keycap, func[x].position, Quaternion.identity);
        }
    }

    void WaveMap()
    {
        for(int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y].item = map[x, y + 1].item;
                map[x, y].UpdateKeycap();
            }
        }

        for(int x = 0; x < width; x++)
        {
            map[x, height - 1].item = func[x].item;
            map[x, height - 1].UpdateKeycap();
        }

        string seed = Time.time.ToString();

        System.Random randomItem = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            func[x].item = randomItem.Next(0, 2);
            func[x].UpdateKeycap();
        }

        player.transform.position = nextPos;
    }

    void FuncGenerator()
    {
        string seed = Time.time.ToString();

        System.Random randomItem = new System.Random(seed.GetHashCode());

        for(int x = 0; x < width; x++)
        {
            func[x] = new Map(randomItem.Next(0, 2), new Vector2(-width / 2 + x, height / 2 + 2), keycap);
            //func[x].UpdateKeycap();
        }
    }

    void ClickKey()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D rayhit = Physics2D.Raycast(mousePos, Vector2.zero);

        if(rayhit.transform != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                returnKeyCapColor(curCliked);
                curCliked = FindKeyCap(rayhit.transform.gameObject);

                if(curCliked != null)
                {
                    curCliked.GetComponent<SpriteRenderer>().color = Color.red;
                    nextPos = curCliked.transform.position;
                }

                Debug.Log("Clicked");
            }
                
        }
    }

    GameObject FindKeyCap(GameObject gb)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, y].keycap == gb)
                {
                    Debug.Log("Found!");
                    return map[x, y].keycap;
                }
            }
        }

        return null;
    }

     void returnKeyCapColor(GameObject gb)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, y].keycap == gb)
                {
                    if(map[x, y].item == 1)
                    {
                        curCliked.GetComponent<SpriteRenderer>().color = Color.green;
                    }
                    else
                    {
                        curCliked.GetComponent<SpriteRenderer>().color = Color.white;
                    }
                }
            }
        }
    }

    //void OnDrawGizmos()
    //{
    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            Gizmos.color = (map[x, y].item == 1) ? Color.black : Color.white;
    //            Gizmos.DrawCube(new Vector3(map[x, y].position.x, map[x, y].position.y, 0), Vector3.one * cubeSize);
    //        }
    //    }

    //    for(int x = 0; x < width; x++)
    //    {
    //        Gizmos.color = (func[x].item == 1) ? Color.black : Color.white;
    //        Gizmos.DrawCube(new Vector3(func[x].position.x, func[x].position.y, 0), Vector3.one * cubeSize);
    //    }

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(new Vector3(player.position.x, player.position.y, 0), Vector3.one * playerSize);
    //}

    public class Map
    {
        public int item;
        public Vector2 position;

        public GameObject keycap;

        public Map(int _item, Vector2 _position, GameObject _keycap)
        {
            item = _item;
            position = _position;
            keycap = _keycap;
        }

        public void UpdateKeycap()
        {
            switch(item)
            {
                case 0:
                    keycap.GetComponent<SpriteRenderer>().color = Color.white;
                    break;
                case 1:
                    keycap.GetComponent<SpriteRenderer>().color = Color.green;
                    break;
            }
        }
    }

    public class Player
    {
        public Vector2 position;

        public Player(Vector2 _pos)
        {
            position = _pos;
        }
    }
}
