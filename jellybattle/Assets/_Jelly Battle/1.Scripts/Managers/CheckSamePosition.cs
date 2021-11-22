using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CheckSamePosition : MonoBehaviour
{
    private Player[] players;
    //private Player[] players;

    //public Dictionary<Vector3, List<GameObject>> overlap;
    //public List<List<GameObject>> overlapPlayers;
    [Range(0.1f, 0.5f)]
    public float distance = 0.5f;
    public List<Dictionary<Vector3, List<Player>>> overlapPlayers;
    [HideInInspector]
    public bool isOverlaped = false;

    private List<Player>[] locations;
    private List<Player[]> resultLocated;

    public class LocatedPlayer
    {
        public Player player;
        public Vector3 nextPos;
        public Location location;
        public int randomNum;
    }
    Queue<LocatedPlayer> locatedPlayers;

    public Player winPlayer;
    public List<Player> losePlayer;
    public int overlapBattleDamage = 10;

    private int wreck = 0;

    public enum Location { Top = 0, Left, Right, Bottom }

    private void Start()
    {
        PlayerManager pm = GetComponent<PlayerManager>();
        players = new Player[pm.players.Length];

        for(int i = 0; i < pm.players.Length; i++)
        {
            players[i] = pm.players[i];
        }

        //overlap = new Dictionary<Vector3, List<GameObject>>();

        overlapPlayers = new List<Dictionary<Vector3, List<Player>>>();
        resultLocated = new List<Player[]>();
        locatedPlayers = new Queue<LocatedPlayer>();
    }

    public void HandleOverlap()
    {
        CheckOverlap();

        LocateOverlapedPlayers();
    }

    /// <summary>
    /// 한 자리에 겹치는 플레이어가 있는지 확인 후 플레이어들을 저장해둔다.
    /// </summary>
    public void CheckOverlap()
    {
        PlayerManager pm = GetComponent<PlayerManager>();
        players = new Player[pm.players.Length];
        Debug.Log(pm.players.Length);
        for (int i = 0; i < pm.players.Length; i++)
        {
            Debug.Log(pm.players[i]);
            players[i] = pm.players[i];
        }

        overlapPlayers = new List<Dictionary<Vector3, List<Player>>>();

        Dictionary<Vector3, List<Player>> allPlayer = new Dictionary<Vector3, List<Player>>();

        foreach (Player player in players)
        {
            List<Player> existing;

            if(allPlayer.TryGetValue(player.NextPos, out existing))
            {
                existing.Add(player);
                allPlayer[player.NextPos] = existing;
                //allPlayer.Add(player.GetComponent<Player>().nextPos, existing);
            }
            else
            {
                allPlayer.Add(player.NextPos, new List<Player>() { player });
            }
        }

        foreach(KeyValuePair<Vector3, List<Player>> pair in allPlayer)
        {
            if(pair.Value.Count >= 2)
            {
                Dictionary<Vector3, List<Player>> newDic = new Dictionary<Vector3, List<Player>>();
                newDic.Add(pair.Key, pair.Value);
                overlapPlayers.Add(newDic);

                isOverlaped = true;
                //overlapPlayers.Add(pair.Value);
            }
        }
    }

    private void CheckOverlapNew()
    {
        
    }

    /// <summary>
    /// 한 자리에 겹치는 플레이어들의 사이를 띄어둔다.
    /// </summary>
    public void RePositionOverlapPlayers()
    {
        if (isOverlaped)
        {
            foreach (Dictionary<Vector3, List<Player>> dic in overlapPlayers)
            {
                foreach (KeyValuePair<Vector3, List<Player>> overlaped in dic)
                {
                    switch (overlaped.Value.Count)
                    {
                        case 2:
                            if (overlaped.Value[0].transform.position.x <= overlaped.Value[1].transform.position.x)
                            {
                                overlaped.Value[0].SetNextPos(overlaped.Value[0].NextPos.x - distance,
                                                                                        overlaped.Value[0].NextPos.y);
                                overlaped.Value[1].SetNextPos(overlaped.Value[1].NextPos.x + distance,
                                                                                        overlaped.Value[1].NextPos.y);
                            }
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// nextPos가 겹치는 플레이어들을 상, 하, 좌, 우 각 구역으로 나눠놓음
    /// </summary>
    public void LocateOverlapedPlayers()
    {
        if (isOverlaped)
        {
            //locations = new List<List<Player>[]>();

            foreach (Dictionary<Vector3, List<Player>> dic in overlapPlayers)
            {
                foreach (KeyValuePair<Vector3, List<Player>> overlaped in dic)
                {
                    locations = new List<Player>[4];
                    
                    for (int i = 0; i < 4; i++)
                    {
                        locations[i] = new List<Player>();
                    }

                    Vector3 nextPos = overlaped.Key;
                    List<Player> ol = overlaped.Value;

                    int bPositive = (int)(nextPos.y - nextPos.x);
                    int bNegative = (int)(nextPos.y + nextPos.x);

                    foreach(Player player in ol)
                    {
                        Vector3 playerPos = player.transform.position;
                        int yPositive = (int)(playerPos.y - playerPos.x);
                        int yNegative = (int)(playerPos.y + playerPos.x);

                        // Top
                        if (yPositive >= bPositive && yNegative >= bNegative)
                            locations[0].Add(player);
                        // left
                        else if (yPositive >= bPositive && yNegative < bNegative)
                            locations[1].Add(player);
                        // right
                        else if (yPositive < bPositive && yNegative >= bNegative)
                            locations[2].Add(player);
                        // bottom
                        else if (yPositive < bPositive && yNegative < bNegative)
                            locations[3].Add(player);
                    }

                    Debug.Log("Before Separate()");

                    while (SeparateOverlapedPlayers()) { Debug.Log(wreck); if (wreck++ > 10) { wreck = 0; break; } }

                    Debug.Log("after SeparateOverLapedPlayers()");
                    //locations.Add(newLocations);
                    RePositionOverlapPlayersNew(locations);
                }
            }
        }
    }

    /// <summary>
    /// 각 구역 내에서 겹치는 플레이어를 분배 시킴
    /// </summary>
    /// <returns></returns>
    private bool SeparateOverlapedPlayers()
    {
        Debug.Log("Start of Saparate()");
        if (!locations.Any(list => list.Count >= 2)) return false;
        
        var oLoc = locations.First(list => list.Count >= 2);
        Debug.Log(oLoc.Count);
        Debug.Log(oLoc);

        int emptyIndex = -1;

        for(int i = 0; i < 4; i++)
        {
            if (locations[i].Count == 0)
            {
                emptyIndex = i;
                break;
            }
        }

        Player movePlayer = new Player();
        Debug.Log("Before Switch");
        switch(emptyIndex)
        {
            case 0:
                movePlayer = oLoc.OrderByDescending(player => player.transform.position.y).First();
                locations[0].Add(movePlayer);
                oLoc.Remove(movePlayer);
                break;
            case 1:
                movePlayer = oLoc.OrderBy(player => player.transform.position.x).First();
                locations[1].Add(movePlayer);
                oLoc.Remove(movePlayer);
                break;
            case 2:
                movePlayer = oLoc.OrderByDescending(player => player.transform.position.x).First();
                locations[2].Add(movePlayer);
                oLoc.Remove(movePlayer);
                break;
            case 3:
                movePlayer = oLoc.OrderBy(player => player.transform.position.y).First();
                locations[3].Add(movePlayer);
                oLoc.Remove(movePlayer);
                break;
        }
        Debug.Log("After Switch");

        return true;
    }

    private void RePositionOverlapPlayersNew(List<Player>[] result)
    {
        Player[] locations = new Player[4];

        for(int i = 0; i< 4; i++)
        {
            if (result[i].Count == 0)
                locations[i] = null;
            else
                locations[i] = result[i][0];
        }

        for (int i = 0; i < 4; i++)
        {
            if (locations[i] != null)
            {
                LocatedPlayer newPlayer = new LocatedPlayer();
                newPlayer.player = locations[i];
                newPlayer.nextPos = locations[i].NextPos;
                newPlayer.location = (Location)i;
                locatedPlayers.Enqueue(newPlayer);
            }
        }

        // Top
        if (locations[0] != null)
            locations[0].SetNextPos(new Vector3(locations[0].NextPos.x, locations[0].NextPos.y + distance));
        // Left
        if (locations[1] != null)
            locations[1].SetNextPos(new Vector3(locations[1].NextPos.x - distance, locations[1].NextPos.y));
        // Right
        if (locations[2] != null)
            locations[2].SetNextPos(new Vector3(locations[2].NextPos.x + distance, locations[2].NextPos.y));
        // Bottom
        if (locations[3] != null)
            locations[3].SetNextPos(new Vector3(locations[3].NextPos.x, locations[3].NextPos.y - distance));

        resultLocated.Add(locations);
    }

    /// <summary>
    /// Player의 원래의 nextPos를 찾아낸다.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="nextPos"></param>
    /// <returns></returns>
    private bool GetOverlapPosition(Player player, out Vector3 nextPos)
    {
        foreach (Dictionary<Vector3, List<Player>> dic in overlapPlayers)
        {
            foreach (KeyValuePair<Vector3, List<Player>> overlaped in dic)
            {
                if (overlaped.Value.Contains(player))
                {
                    nextPos = overlaped.Key;
                    return true;
                }
            }
        }

        nextPos = new Vector3();
        return false;
    }

    /// <summary>
    /// 한 자리에 겹치는 플레이어들 중 자리를 차지할 플레이어를 결정하고 나머지는 옆 자리로 밀려난다.
    /// </summary>
    public void RandomPositionWinner()
    {
        if (isOverlaped)
        {
            Debug.Log("RandomPositionWinner()");

            string seed = (Time.time + Random.value).ToString();

            System.Random random = new System.Random(seed.GetHashCode());

            Dictionary<int, Player> randomPlayer = new Dictionary<int, Player>();

            foreach (Dictionary<Vector3, List<Player>> dic in overlapPlayers)
            {
                foreach (KeyValuePair<Vector3, List<Player>> overlaped in dic)
                {
                    foreach (Player player in overlaped.Value)
                    {
                        int rn = random.Next(1, 11);

                        
                        Debug.LogWarning("RandomNumber has SameKey");
                            
                        while(randomPlayer.ContainsKey(rn))
                        {
                            rn = random.Next(1, 11);
                        }

                        randomPlayer.Add(rn, player);
                    }
                }
            }

            Player winner = new Player();
            int HighNum = 0;

            foreach (KeyValuePair<int, Player> player in randomPlayer)
            {
                //Debug.Log(player.Key + ", " + player.Value);
                if (player.Key > HighNum)
                {
                    HighNum = player.Key;
                    winner = player.Value;
                }
            }

            //Debug.Log(HighNum + " / " + winner);

            randomPlayer.TryGetValue(HighNum, out winner);

            //Debug.Log(winner.GetComponent<Player>().ID);

            Vector3 winnerPos = new Vector3();
            if (GetOverlapPosition(winner, out winnerPos))
            {
                winner.SetPosition(winnerPos);
                winner.SetNextPos(winnerPos);
                winner.SetCurrentKeyCap(winnerPos);
            }
            else
                Debug.LogError("Can't find winner's next position. (Method:RandomPositionWinner / CheckSamePostion.cs)");

            randomPlayer.Remove(HighNum);

            foreach (KeyValuePair<int, Player> player in randomPlayer)
            {
                Vector3 nextPos;

                if (GetOverlapPosition(player.Value, out nextPos))
                {
                    if (player.Value.transform.position.x < nextPos.x)
                    {
                        player.Value.SetPosition(new Vector3(nextPos.x - 1, nextPos.y));
                        player.Value.SetNextPos(new Vector3(nextPos.x - 1, nextPos.y));
                        player.Value.SetCurrentKeyCap(new Vector3(nextPos.x - 1, nextPos.y));
                    }
                    else
                    {
                        player.Value.SetPosition(new Vector3(nextPos.x + 1, nextPos.y));
                        player.Value.SetNextPos(new Vector3(nextPos.x + 1, nextPos.y));
                        player.Value.SetCurrentKeyCap(new Vector3(nextPos.x + 1, nextPos.y));
                    }
                }
                else
                {
                    Debug.LogError("Can't find position. (Method:RandomPositionWinner / CheckSamePostion.cs)");
                }
            }

            isOverlaped = false;
        }
    }

    public void RandomPositionWinnerNew()
    {
        if (isOverlaped)
        {
            while(locatedPlayers.Count > 0)
            {
                List<LocatedPlayer> lp = new List<LocatedPlayer>();
                lp.Add(locatedPlayers.Dequeue());

                while(locatedPlayers.Peek().nextPos == lp[0].nextPos)
                {
                    lp.Add(locatedPlayers.Dequeue());
                    if (locatedPlayers.Count == 0) break;
                }

                for(int i = 0; i < lp.Count; i++)
                {
                    string seed = (Time.time + Random.value).ToString();
                    System.Random random = new System.Random(seed.GetHashCode());

                    lp[i].randomNum = random.Next(1, 11);
                }

                var winner = lp.OrderByDescending(player => player.randomNum).First();
                winner.player.SetPosition(winner.nextPos);
                winner.player.SetNextPos(winner.nextPos);
                winner.player.SetCurrentKeyCap(winner.nextPos);
                lp.Remove(winner);
                winPlayer = new Player();
                winPlayer = winner.player;                
                
                foreach(LocatedPlayer locatedPlayer in lp)
                {
                    losePlayer = new List<Player>();
                    losePlayer.Add(locatedPlayer.player);

                    Debug.Log("Located Player : " + KeyBoard.instance.IsKeyBoard(locatedPlayer.player.transform.position));

                    if (locatedPlayer.location == Location.Top)
                    {
                        Debug.Log("Located : " + locatedPlayer.location);

                        if(!KeyBoard.instance.IsKeyBoard(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y + 1)))
                        {
                            locatedPlayer.location = Location.Left;
                        }
                        else
                        {
                            locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y + 1));
                            locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                            locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                        }
                    }
                    if (locatedPlayer.location == Location.Left)
                    {

                        Debug.Log("Located : " + locatedPlayer.location);
                        

                        Debug.Log("IsKeyBoard() In CheckSamePosition.cs : " + KeyBoard.instance.IsKeyBoard(locatedPlayer.player.transform.position));

                        if (!KeyBoard.instance.IsKeyBoard(new Vector3(locatedPlayer.nextPos.x - 1, locatedPlayer.nextPos.y)))
                        {
                            Debug.Log("Location Left to Right");
                            locatedPlayer.location = Location.Right;
                        }
                        else
                        {
                            locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x - 1, locatedPlayer.nextPos.y));
                            locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                            locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                        }
                    }
                    if (locatedPlayer.location == Location.Right)
                    {
                        Debug.Log("Located : " + locatedPlayer.location);
                        

                        if (!KeyBoard.instance.IsKeyBoard(new Vector3(locatedPlayer.nextPos.x + 1, locatedPlayer.nextPos.y)))
                        {
                            locatedPlayer.location = Location.Bottom;
                        }
                        else
                        {
                            locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x + 1, locatedPlayer.nextPos.y));
                            locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                            locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                        }
                    }
                    if (locatedPlayer.location == Location.Bottom)
                    {
                        Debug.Log("Located : " + locatedPlayer.location);

                        if (!KeyBoard.instance.IsKeyBoard(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y - 1)))
                        {
                            locatedPlayer.location = Location.Top;

                            locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y + 1));
                            locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                            locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                        }
                        else
                        {
                            locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y - 1));
                            locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                            locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                        }
                    }

                    //switch (locatedPlayer.location)
                    //{
                    //    case Location.Top:
                    //        locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y + 1));
                    //        locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                    //        locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                    //        break;
                    //    case Location.Left:
                    //        locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x - 1, locatedPlayer.nextPos.y));
                    //        locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                    //        locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                    //        break;
                    //    case Location.Right:
                    //        locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x + 1, locatedPlayer.nextPos.y));
                    //        locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                    //        locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                    //        break;
                    //    case Location.Bottom:
                    //        locatedPlayer.player.SetPosition(new Vector3(locatedPlayer.nextPos.x, locatedPlayer.nextPos.y - 1));
                    //        locatedPlayer.player.SetNextPos(locatedPlayer.player.transform.position);
                    //        locatedPlayer.player.SetCurrentKeyCap(locatedPlayer.player.transform.position);
                    //        break;
                    //}
                }
            }

            //isOverlaped = false;
        }
    }
}
