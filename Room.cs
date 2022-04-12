using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    RoomManager roomManager;
    [SerializeField] EnemyRoomManager enemyManager;
    [HideInInspector] public Player player;
    //VisualMapVariables
    public int regionIndex = -1;
    public Vector2Int region;

    //GamePlayMapVariables
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private List<GameObject> obstaclesPrefab;
    private List<GameObject> roomObstacles;
    public GameObject[,] grid;
    public Vector2Int posInRoomGrid;
    public List<WaveCollapseTile> plainOne;
    public List<WaveCollapseTile> plainTwo;
    public List<GridTile> freeTiles;

    [HideInInspector] public Room[] doors;
    [HideInInspector] public Vector2Int[] doorPositions;
    [SerializeField] private Transform roomsP;
    public List<DamageSection> dmges;

    public void Awake()
    {
        doors = new Room[]
        {
           null, null, null, null
        };
        doorPositions = new Vector2Int[]
        {
           new Vector2Int(-1, -1),
           new Vector2Int(-1, -1),
           new Vector2Int(-1, -1),
           new Vector2Int(-1, -1)
        };
        dmges = new List<DamageSection>();
    }
    private Coroutine doorTimerCoro;
    public void CreateRoom(RoomManager roomM, Vector2Int gridSize, WaveCollapseFunction waveCF, List<Vector2Int> regions)
    {
        roomManager = roomM;
        grid = new GameObject[gridSize.x, gridSize.y];
        roomObstacles = new List<GameObject>();
        float[] tileChances = waveCF.GivePalleteChance();

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                GameObject obj = Instantiate(tilePrefab, roomsP);
                grid[x, y] = obj;
                Vector2Int pos = new Vector2Int(x, y);
                obj.GetComponent<WaveCollapseTile>().InstantiateTile(pos, tileChances);
            }
        }

        CreateDoorLocations();
        waveCF.CreateRoomTileMap(this,regions);
        PopulateWithObstacles();
    }

    public void PopulateWithObstacles()
    {
        plainOne = new List<WaveCollapseTile>();
        plainTwo = new List<WaveCollapseTile>();
        freeTiles = new List<GridTile>();
        foreach (var tileObj in grid)
        {
            WaveCollapseTile tile = tileObj.GetComponent<WaveCollapseTile>();
            if (tile.type == 0)
            {
                plainOne.Add(tile);
            }
            else if(tile.type == 1)
            {
                plainTwo.Add(tile);
            }
            else
            {
                //instantiate non plain tiles
                tile.GetComponent<GridTile>().InstantiateTile(this, tile.gridPosition, 5);
                freeTiles.Add(tile.GetComponent<GridTile>());
            }
        }
        RemoveDoorPositionsFromList();
        SpawnSecondaryTile(plainOne, true);
        SpawnSecondaryTile(plainTwo, false);
    }
    public void RemoveDoorPositionsFromList()
    {
        foreach (var pos in doorPositions)
        {
            if(pos.x != -1)
            {
                WaveCollapseTile tile = grid[pos.x, pos.y].GetComponent<WaveCollapseTile>();
                if (plainOne.Contains(tile))
                {
                    tile.GetComponent<GridTile>().InstantiateTile(this, tile.gridPosition, 5);
                    plainOne.Remove(tile);
                }
                if (plainTwo.Contains(tile))
                {
                    tile.GetComponent<GridTile>().InstantiateTile(this, tile.gridPosition, 5);
                    plainTwo.Remove(tile);
                }
            }
        }
    }
    public void SpawnSecondaryTile(List<WaveCollapseTile> list, bool first)
    {
        int total, blocks, specialTiles;
        //create a random amount of obstacles and special tiles according to the amount of free tiles
        total = list.Count;
        blocks = total * Random.Range(5,10)/100;
        specialTiles = total* Random.Range(15,25)/100;

        foreach (var tile in list)
        {
            GridTile gTile = tile.GetComponent<GridTile>();
            int ran = Random.Range(0, total);

            if(blocks >= ran)
            {
                gTile.InstantiateTile(this, tile.gridPosition, 4);
                SpawnObstacleObjects(tile.gridPosition, 4);
            }
            else if(specialTiles + blocks >= ran)
            {
                if (first)
                {
                    gTile.InstantiateTile(this, tile.gridPosition, region.x);
                    SpawnObstacleObjects(tile.gridPosition, region.x);
                }
                else
                {
                    gTile.InstantiateTile(this, tile.gridPosition, region.y);
                    SpawnObstacleObjects(tile.gridPosition, region.y);
                }
               
            }
            else
            {
                //instantiate as a normal plain
                gTile.InstantiateTile(this, tile.gridPosition, 5);
                freeTiles.Add(gTile);
            }
        }
       
    }

    public void SpawnObstacleObjects(Vector2Int pos, int index)
    {
        GameObject obj = Instantiate(obstaclesPrefab[index], new Vector3(pos.x,pos.y,0f), Quaternion.identity, roomsP);
        obj.SetActive(false);
        roomObstacles.Add(obj);
    }
    public void EnableObstacles(bool enable)
    {
        foreach (var obs in roomObstacles)
        {
            obs.SetActive(enable);
        }
    }
    public void CreateDoorLocations()
    {
        for (int i = 0; i < doors.Length; i++)
        {
            if(doors[i] != null)
            {
                Vector2Int pos;
                switch (i)
                {
                    case 0:
                        pos = new Vector2Int(Random.Range(1,grid.GetLength(0)-1)  ,     0);
                        break;
                    case 1:
                        pos = new Vector2Int(0                                    , Random.Range(1, grid.GetLength(1) - 1));
                        break;
                    case 2:
                        pos = new Vector2Int(Random.Range(1,grid.GetLength(0)-2)  , grid.GetLength(1)-1);
                        break;
                    case 3:
                        pos = new Vector2Int(grid.GetLength(0)-1                  , Random.Range(1, grid.GetLength(1) - 1));
                        break;
                    default:
                        pos = Vector2Int.zero;
                        break;
                }
             
                doorPositions[i] = pos;
                grid[pos.x, pos.y].GetComponent<GridTile>().hasDoor = true;
            }
        }
    } 
    public void DoorInteraction(bool on, Vector2Int pos)
    {
        if (on)
        {
           doorTimerCoro= StartCoroutine(DoorTimerCoro(pos));
        }
        else
        {
            if(doorTimerCoro != null)
            StopCoroutine(doorTimerCoro);
        }
    }
    IEnumerator DoorTimerCoro(Vector2Int pos)
    {
        while(GameManager.instance.inTurn)
        {
            yield return null;
        }
        for (float i = 0; i < 0.5f; i+=Time.deltaTime)
        {
            yield return null;
        }

        for (int i = 0; i < doorPositions.Length; i++)
        {
            if(doorPositions[i] == pos && player.isAlive)
            {
                ActivateCurrentDroids(false);
                roomManager.ChangeRooms(doors[i],i);
                break;
;           }
        }
        grid[pos.x, pos.y].GetComponent<GridTile>().PlayerLeft();
        doorTimerCoro = null;
    }

    public void PlayerHasEnteredRoom()
    {
       enemyManager.playerHasEnteredRoom(this);
    }
    public void DestroyEnemies()
    {
        enemyManager.DestroyEnemies();
    }
    public void EnemiesTurn()
    {
        enemyManager.EnemiesTurn(player.futurePos,this);
    }
    public void ActivateCurrentDroids(bool activate)
    {
        enemyManager.ActivateDroids(activate);
    }
    public void EnemyHasDied(EnemyBrain en)
    {
        enemyManager.EnemyHasDied(en);
    }
    public bool InGrid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= grid.GetLength(0) ||
           pos.y < 0 || pos.y >= grid.GetLength(1)
            ) return false;
        return true;
    }

    public void MakeConnection(int dir, Room room )
    {
        doors[dir] = room;
    }

   public Vector2Int GiveRandomGridPos()
    {
        return new Vector2Int(Random.Range(0,grid.GetLength(0)-1),Random.Range(0,grid.GetLength(1)));
    }

    public Vector2Int GiveRandomFreePos()
    {
        if(freeTiles.Count > 0)
        {
            GridTile toGive = freeTiles[Random.Range(0, freeTiles.Count)];
            while (toGive.tileType <0 || toGive.contains != null)
            {
                toGive = freeTiles[Random.Range(0, freeTiles.Count)];
            }

            return toGive.gridPos;
        }
        else
        {
            Debug.Log("Giving random pos");
            return GiveRandomGridPos();
           
        }
    }
    public void SpawnEnemies()
    {
        enemyManager.SpawnWave(this);
    }

    public void DealDamages()
    {
        enemyManager.CheckEnemiesAliveStatus();
        player.HasDied();
       
    }

    public void PlayerHasDied()
    {
        Vector2Int pos = GiveRandomFreePos();
        player.transform.position = new Vector3(pos.x, pos.y + 0.36f, 0);
        player.PlayerInit(pos,pos,this,GameManager.instance);
        grid[pos.x, pos.y].GetComponent<GridTile>().contains =player.gameObject;
        player.currentTile = grid[pos.x, pos.y].GetComponent<GridTile>();

    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3((grid.GetLength(0) - 1f) / 2f, (grid.GetLength(1) - 1f) / 2f, 1f), new Vector3(15, 8, 1f));
    }

   
}
public class DamageSection
{
    public DamageSection(int dmg, Vector2Int loc)
    {
        damage = dmg;
        location = loc;
    }

    public int damage;
    public Vector2Int location;
}
