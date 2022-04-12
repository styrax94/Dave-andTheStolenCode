using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private WaveCollapseFunction waveCollapse;

    private List<Vector2Int> possibleRegions = new List<Vector2Int>()
    {
        new Vector2Int(0,1),
        new Vector2Int(0,2),
        new Vector2Int(0,3),
        new Vector2Int(1,2),
        new Vector2Int(1,3),
        new Vector2Int(3,2),

    };
    //Direction variables
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(1,0)
    };
    private Vector2Int[] reversedDirections = new Vector2Int[]
    {
         new Vector2Int(0, 1),
        new Vector2Int( 1,0),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0)
    };
    
    //VisualMapVariables
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private List<RegionTiles> regionTiles;
    [SerializeField] private List<Tile> mapObjectList;
    [SerializeField] private GameObject cam;

    //Game Rooms Variables
    private int roomGridSize = 12;
    private Room[,] roomGrid;
    private List<Room> roomsList;
    [SerializeField] private GameObject roomPrefab;
    public Room currentPlayerRoom; //CurrentRoomPlayerIsIn

    public void StartGame()
    {     
        roomsList = new List<Room>();
        roomGrid = new Room[roomGridSize, roomGridSize];
        int ranNumRooms = Random.Range(roomGridSize, roomGridSize);
        int spawnedRooms = 1;
        GameObject roomObj = Instantiate(roomPrefab, transform);
        Room currentRoom = roomObj.GetComponent<Room>();
        roomGrid[roomGridSize / 2, roomGridSize / 2] = currentRoom;
        currentRoom.posInRoomGrid = new Vector2Int(roomGridSize / 2, roomGridSize / 2);
        roomsList.Add(currentRoom);
        currentPlayerRoom = currentRoom;
      
        int count = 0;
        while (spawnedRooms < ranNumRooms)
        {
            spawnedRooms += SpawnRooms(currentRoom, 0, 0);
            count++;
            if(count > 10)
            {
                break;
                
            }
        }
       
        MakeRegionConnections(currentRoom, null);
        SpawnTileMapDataForRooms();
        SetTileMap();
        
        //cam.transform.position = new Vector3((currentRoom.grid.GetLength(0)-1f)/ 2f, (currentRoom.grid.GetLength(1)-1f) / 2f, cam.transform.position.z);
    }

    public void SpawnTileMapDataForRooms()
    {
        foreach (var room in roomsList)
        {
            room.CreateRoom(this,new Vector2Int(Random.Range(10,16),Random.Range(5,8)),waveCollapse, possibleRegions);
        }
    }
    public void MakeRegionConnections(Room current, Room parent)
    {
        if (current.regionIndex < 0)
        {
            current.regionIndex = Random.Range(0, possibleRegions.Count);
           // current.regionIndex = 3;
            current.region = possibleRegions[current.regionIndex];
        }
          foreach (var neighbour in current.doors)
          {
            if (neighbour != null && parent != neighbour)
            {
                List<int> possIndex = new List<int>();

                for (int i = 0; i < possibleRegions.Count; i++)
                {
                    Vector2Int region = possibleRegions[i];
                    Vector2Int tileRegion = possibleRegions[current.regionIndex];

                    if (tileRegion.x == region.x || tileRegion.x == region.y || tileRegion.y == region.x || tileRegion.y == region.y)
                    {
                        possIndex.Add(i);
                    }
                }

                neighbour.regionIndex = possIndex[Random.Range(0, possIndex.Count)];
                //neighbour.regionIndex = 3;
                neighbour.region = possibleRegions[neighbour.regionIndex];
                MakeRegionConnections(neighbour, current);
            }
          }
    }
    public int SpawnRooms( Room current, int count, int loop)
    {
        List<int> possibleDirections = new List<int>();
        for (int i = 0; i < current.doors.Length; i++)
        {
            if(current.doors[i] == null)
            {
                possibleDirections.Add(i);
            }
        }

        foreach (var index in possibleDirections)
        {
            int random = Random.Range(0, 10);

            if(random < 5-loop)
            {             
                Vector2Int ranDir = directions[index];
                Vector2Int gridPos = current.posInRoomGrid + ranDir;

                if (InGrid(gridPos))
                {
                    if (roomGrid[gridPos.x, gridPos.y] == null)
                    {
                        GameObject roomObj = Instantiate(roomPrefab, transform);
                        Room spawnedRoom = roomObj.GetComponent<Room>();
                        roomGrid[gridPos.x, gridPos.y] = spawnedRoom;
                        spawnedRoom.posInRoomGrid = gridPos;
                        current.MakeConnection(index, spawnedRoom);
                        spawnedRoom.MakeConnection(GetReversedIndex(index), current);
                        roomsList.Add(spawnedRoom);
                        count++;
                        count += SpawnRooms(spawnedRoom, 0, loop + 1);
                    }                  
                }
            }
        }
       
        return count;
    }
    public void SetTileMap()
    {
        tileMap.ClearAllTiles();
        for (int i = 0; i < currentPlayerRoom.grid.GetLength(0); i++)
        {
            for (int b = 0; b < currentPlayerRoom.grid.GetLength(1); b++)
            {
              
                WaveCollapseTile waveTile = currentPlayerRoom.grid[i, b].GetComponent<WaveCollapseTile>();               
                if (waveTile.type >= 0 && waveTile.collapsed)
                {
                    tileMap.SetTile(new Vector3Int(i, b, 0), regionTiles[currentPlayerRoom.regionIndex].tileList[waveTile.type]);
                  
                }
                   
            }

        }
        SetMapObjects();
    }
    public void SetMapObjects()
    {
        currentPlayerRoom.EnableObstacles(true);
        //foreach (var plainOne in currentPlayerRoom.plainOne)
        //{
        //    GridTile tile = plainOne.GetComponent<GridTile>();

        //    switch (tile.tileType)
        //    {
        //        case -1:

        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[4]);
        //            break;
        //        case 0:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[0]);
        //            break;
        //        case 1:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[1]);
        //            break;
        //        case 2:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[2]);
        //            break;
        //        case 3:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[3]);
        //            break;
        //    }
        //}
        //foreach (var plainTwo in currentPlayerRoom.plainTwo)
        //{
        //    GridTile tile = plainTwo.GetComponent<GridTile>();

        //    switch (tile.tileType)
        //    {
        //        case -1:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[4]);
        //            break;
        //        case 0:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[0]);
        //            break;
        //        case 1:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[1]);
        //            break;
        //        case 2:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[2]);
        //            break;
        //        case 3:
        //            tileMap.SetTile(new Vector3Int(tile.gridPos.x, tile.gridPos.y, 0), mapObjectList[3]);
        //            break;
        //    }
        //}
        foreach (var item in currentPlayerRoom.doorPositions)
        {
            if (item.x == -1) continue;
            tileMap.SetTile(new Vector3Int(item.x, item.y, 0), mapObjectList[5]);
        }
    }
    public int GetReversedIndex(int index)
    {
        if(index + 2 >= 4)
        {
            return (index + 2) - 4;
        }
        return index + 2;
    }
    public void ChangeRooms(Room room, int index)
    {
        Vector2Int newPlayerPosition = room.doorPositions[GetReversedIndex(index)];
        currentPlayerRoom.EnableObstacles(false);
        currentPlayerRoom.player.MoveRooms(room, newPlayerPosition);
        currentPlayerRoom = room;
       // cam.transform.position = new Vector3((currentPlayerRoom.grid.GetLength(0) - 1f) / 2f, (currentPlayerRoom.grid.GetLength(1) - 1f) / 2f, cam.transform.position.z);
        SetTileMap();
        currentPlayerRoom.PlayerHasEnteredRoom();
    }
    public Vector2Int GiveRandomPosition()
    {
        return currentPlayerRoom.GiveRandomGridPos();
    }
    public bool InGrid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= roomGrid.GetLength(0) ||
           pos.y < 0 || pos.y >= roomGrid.GetLength(1)
            ) return false;
        return true;
    }
}
