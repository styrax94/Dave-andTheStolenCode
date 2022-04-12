using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveCollapseFunction : MonoBehaviour
{
    //works for different regions not only grass-snow
    //All tile possible neighbours from all directions
    private List<Dictionary<int, List<int>>> tileCombinations = new List<Dictionary<int, List<int>>>()
        {
            //Plain Grass Tile   //14/15/16/17/ removed                      0
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){0,2,4,8,19,21,32,34,38}},
            {1, new List<int>(){0,2,3,4,6,7,13,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){0,2,3,7,19,20,24,32,33,37}},
            {3, new List<int>(){0,2,3,5,7,12,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){0,3,5,9,20,22,26,33,35,39}},
            {5, new List<int>(){0,3,4,5,6,9,10,20,21,22,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){0,4,5,6,21,22,23,34,35,36}},
            {7, new List<int>(){0,2,4,5,6,8,11,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
              //Plain Snow Tile    //14/15/16/17/ removed                      1
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){1,9,10,12}},
            {1, new List<int>(){1,5,6,9,10,11,12 }},
            {2, new List<int>(){1,6,10,11}},
            {3, new List<int>(){1,4,6,8,10,11,13}},
            {4, new List<int>(){1,8,11,13}},
            {5, new List<int>(){1,2,7,8,11,12,13}},
            {6, new List<int>(){1,7,12,13}},
            {7, new List<int>(){1,3,7,9,10,12,13}}
           },
               //Grass with Snow (SW Corner) Tile      2
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){3,7,13}},
            {1, new List<int>(){1,5,6,9,10,11,12, 15,17 }},
            {2, new List<int>(){4,8,13}},
            {3, new List<int>(){0,2,3,5,7,12, 15,17,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){0,3,5,9,20,22,26,33,35,39}},
            {5, new List<int>(){0,3,4,5,6,9,10,14,16,20,21,22,23,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){0,4,5,6,21,22,23,34,35,36}},
            {7, new List<int>(){0,2,4,5,6,8,11,15,17,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
                //Grass with Snow (NW Corner) Tile     3
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){0,2,4,8,19,21,32,34,38}},
            {1, new List<int>(){0,2,3,4,6,7,13,14,16,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){5,9,12}},
            {3, new List<int>(){1,4,6,8,10,11,13, 14, 16}},
            {4, new List<int>(){2,7,12}},
            {5, new List<int>(){0,3,4,5,6,9,10,14,16,20,21,22,23,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){0,4,5,6,21,22,23,34,35,36}},
            {7, new List<int>(){0,2,4,5,6,8,11,15,17,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
                //Grass with Snow (SE Corner) Tile     4
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){5,6,11}},
            {1, new List<int>(){0,2,3,4,6,7,13,14,16,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){0,2,3,7,19,20,24,32,33,37}},
            {3, new List<int>(){0,2,3,5,7,12, 15,17,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){0,3,5,9,20,22,26,33,35,39}},
            {5, new List<int>(){0,3,4,5,6,9,10,14,16,20,21,22,23,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){2,8,11}},
            {7, new List<int>(){1,3,7,9,10,12,13, 14, 16}}
           },
                //Grass with Snow (NE Corner) Tile     5
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){0,2,4,8,19,21,32,34,38}},
            {1, new List<int>(){0,2,3,4,6,7,13,14,16,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){0,2,3,7,19,20,24,32,33,37}},
            {3, new List<int>(){0,2,3,5,7,12, 15,17,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){4,6,10}},
            {5, new List<int>(){1,2,7,8,11,12,13, 15,17}},
            {6, new List<int>(){3,9,10}},
            {7, new List<int>(){0,2,4,5,6,8,11,15,17,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
                //Grass with Snow (Half-Right) Tile    6
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){5,6,11}},
            {1, new List<int>(){0,2,3,4,6,7,13,14,16,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){0,2,3,7,19,20,24,32,33,37}},
            {3, new List<int>(){0,2,3,5,7,12, 15,17,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){4,6,10}},
            {5, new List<int>(){1,2,7,8,11,12,13, 15,17}},
            {6, new List<int>(){1,7,12,13}},
            {7, new List<int>(){1,3,7,9,10,12,13, 14, 16}}
           },
                //Grass with Snow (Snow on Half-Left) Tile     7
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){3,7,13}},
            {1, new List<int>(){1,5,6,9,10,11,12, 15,17 }},
            {2, new List<int>(){1,6,10,11}},
            {3, new List<int>(){1,4,6,8,10,11,13, 14, 16}},
            {4, new List<int>(){2,7,12}},
            {5, new List<int>(){0,3,4,5,6,9,10,14,16,20,21,22,23,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){0,4,5,6,21,22,23,34,35,36}},
            {7, new List<int>(){0,2,4,5,6,8,11,15,17,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
                //Grass with Snow (Half-Down) Tile     8
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){1,9,10,12}},
            {1, new List<int>(){1,5,6,9,10,11,12, 15,17 }},
            {2, new List<int>(){4,8,13}},
            {3, new List<int>(){0,2,3,5,7,12, 15,17,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){0,3,5,9,20,22,26,33,35,39}},
            {5, new List<int>(){0,3,4,5,6,9,10,14,16,20,21,22,23,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){2,8,11,15,17}},
            {7, new List<int>(){1,3,7,9,10,12,13, 14, 16}}
           },
               //Grass with Snow (Half-Up) Tile        9
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){0,2,4,8,19,21,32,34,38}},
            {1, new List<int>(){0,2,3,4,6,7,13,14,16,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){5,9,12}},
            {3, new List<int>(){1,4,6,8,10,11,13, 14, 16}},
            {4, new List<int>(){1,8,11,13}},
            {5, new List<int>(){1,2,7,8,11,12,13, 15,17}},
            {6, new List<int>(){3,9,10}},
            {7, new List<int>(){0,2,4,5,6,8,11,15,17,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
               //Snow with Grass (SW Corner) Tile     10
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){5,6,11}},
            {1, new List<int>(){0,2,3,4,6,7,13,14,16,19,20,21,24,25,30,32,33,34,37,38,43}},
            {2, new List<int>(){5,9,12}},
            {3, new List<int>(){1,4,6,8,10,11,13, 14, 16}},
            {4, new List<int>(){1,8,11,13}},
            {5, new List<int>(){1,2,7,8,11,12,13, 15,17}},
            {6, new List<int>(){1,7,12,13}},
            {7, new List<int>(){1,3,7,9,10,12,13, 14, 16}}
           },
               //Snow with Grass (NW Corner) Tile     11
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){1,9,10,12}},
            {1, new List<int>(){1,5,6,9,10,11,12, 15,17 }},
            {2, new List<int>(){4,8,13}},
            {3, new List<int>(){0,2,3,5,7,12, 15,17,19,20,22,24,26,29,32,33,35,37,39,42}},
            {4, new List<int>(){4,6,10}},
            {5, new List<int>(){1,2,7,8,11,12,13, 15,17}},
            {6, new List<int>(){1,7,12,13}},
            {7, new List<int>(){1,3,7,9,10,12,13, 14, 16}}
           },
              //Snow with Grass (SE Corner) Tile      12
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){3,7,13}},
            {1, new List<int>(){1,5,6,9,10,11,12, 15,17 }},
            {2, new List<int>(){1,6,10,11}},
            {3, new List<int>(){1,4,6,8,10,11,13, 14, 16}},
            {4, new List<int>(){1,8,11,13}},
            {5, new List<int>(){1,2,7,8,11,12,13, 15,17}},
            {6, new List<int>(){3,9,10}},
            {7, new List<int>(){0,2,4,5,6,8,11,15,17,19,21,22,23,25,28,32,34,35,36,38,41}}
           },
             //Snow with Grass (NE Corner) Tile       13
            new Dictionary<int, List<int>>
           {
               //neighbor directions
            {0, new List<int>(){1,9,10,12}},
            {1, new List<int>(){1,5,6,9,10,11,12, 15,17 }},
            {2, new List<int>(){1,6,10,11}},
            {3, new List<int>(){1,4,6,8,10,11,13, 14, 16}},
            {4, new List<int>(){2,7,12}},
            {5, new List<int>(){0,3,4,5,6,9,10,14,16,20,21,22,23,26,27,33,34,35,36,39,40}},
            {6, new List<int>(){2,8,11}},
            {7, new List<int>(){1,3,7,9,10,12,13, 14, 16}}
           }          
        };
    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int( 0, 1),
        new Vector2Int( 1, 1),
        new Vector2Int( 1, 0),
        new Vector2Int( 1, -1),
        new Vector2Int( 0, -1),
        new Vector2Int( -1,-1),
        new Vector2Int( -1, 0),
        new Vector2Int( -1, 1),
        
    };


    private List<float[]> tileChancePresets = new List<float[]>()
    {
        new float[]{5,75,2,2,2,2,1,1,1,1,2,2,2,2},
        new float[]{8,8,10,10,10,10,1,1,1,1,10,10,10,10},
        new float[]{6,6,1,1,1,1,20,20,20,20,1,1,1,1},
        new float[]{50,15,2,2,2,2,5,5,5,5,2,2,2,1,},
        new float[]{40,40,2,2,2,1,2,2,2,2,2,1,1,1,},
    };
   
    public void CreateRoomTileMap(Room room, List<Vector2Int>possibleRegions)
    {
        List<WaveCollapseTile> openList = new List<WaveCollapseTile>();
        List<int> startingTiles = new List<int>();
        for (int i = 0; i < room.doors.Length; i++)
        {
            if(room.doors[i] != null)
            {
                Vector2Int current = possibleRegions[room.regionIndex];
                Vector2Int neighbor = possibleRegions[room.doors[i].regionIndex];

                if(current.x == neighbor.x || current.x == neighbor.y)
                {
                    startingTiles.Add(0);
                }
                else if(current.y == neighbor.x || current.y == neighbor.y)
                {
                    startingTiles.Add(1);
                }
            }
            startingTiles.Add(-1);
        }
       
        for (int i = 0; i < room.doorPositions.Length; i++)
        {         
            Vector2Int doorPos = room.doorPositions[i];
            if (doorPos.x == -1) continue;
           
            WaveCollapseTile waveTile = room.grid[doorPos.x, doorPos.y].GetComponent<WaveCollapseTile>();
            waveTile.SetTileType(startingTiles[i]);
            UpdateNeighborCompatibility(room, waveTile);
           openList.Add(waveTile);

        }
        while (openList.Count > 0)
        {
            WaveCollapseTile currentTile = Min(openList);
            openList.Remove(currentTile);

           // AddNeighborsToList(room, currentTile, openList);

            if (!currentTile.collapsed)
            {
                currentTile.SetTileType();
                UpdateNeighborCompatibility(room, currentTile);
            }
        }
        WaveCollapseTile temp = room.grid[Random.Range(0, room.grid.GetLength(0) - 1)/2, Random.Range(0, room.grid.GetLength(1))/2].GetComponent<WaveCollapseTile>();
        temp.SetTileType(0);
        UpdateNeighborCompatibility(room, temp);
        openList.Add(temp);

        while (openList.Count > 0)
        {
            WaveCollapseTile currentTile = Min(openList);
            openList.Remove(currentTile);

            AddNeighborsToList(room,currentTile, openList);

            if (!currentTile.collapsed)
            {
                currentTile.SetTileType();
                UpdateNeighborCompatibility(room,currentTile);
            }
        }

       //ReCheckGrid(room);
    }

    public void ReCheckGrid(Room room)
    {
        foreach (var tile in room.grid)
        {
            WaveCollapseTile waveTile = tile.GetComponent<WaveCollapseTile>();
            if (waveTile.type == -1)
            {
                waveTile.ResetCombinations();
                ResetNeighbour(room, waveTile);
                WaveCollapseMini(room,waveTile);
            }
        }
    }
    public void WaveCollapseMini(Room room,WaveCollapseTile gridTile)
    {
        List<WaveCollapseTile> openList = new List<WaveCollapseTile>();
        //gridTile.SetType(true, chance);


        UpdateNeighborCompatibility(room,gridTile);
        AddNeighborsToList(room,gridTile, openList);
        openList.Add(gridTile);

        while (openList.Count > 0)
        {
            WaveCollapseTile currentTile = Min(openList);
            openList.Remove(currentTile);

            if (!currentTile.collapsed)
            {
                currentTile.SetTileType();
                UpdateNeighborCompatibility(room,currentTile);
            }

        }
    }
    public void ResetNeighbour(Room room, WaveCollapseTile current)
    {
        foreach (var item in directions)
        {
            Vector2Int pos = current.gridPosition + item;
            if (room.InGrid(pos))
            {
                WaveCollapseTile gridT = room.grid[pos.x, pos.y].GetComponent<WaveCollapseTile>();
                gridT.ResetCombinations();

            }
        }
    }
    public WaveCollapseTile Min(List<WaveCollapseTile> list)
    {
        WaveCollapseTile min = list[0];
        foreach (var item in list)
        {
            if (min.possibleCombinations.Count > item.possibleCombinations.Count)
            {
                min = item;
            }
        }
        return min;
    }

    public void AddNeighborsToList(Room room,WaveCollapseTile current, List<WaveCollapseTile> list)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int pos = current.gridPosition + directions[i];
            if (room.InGrid(pos))
            {
                WaveCollapseTile gridT = room.grid[pos.x, pos.y].GetComponent<WaveCollapseTile>();
                if (!gridT.collapsed && !list.Contains(gridT))
                {
                    list.Add(gridT);
                }

            }
        }
    }
    public void UpdateNeighborCompatibility(Room room, WaveCollapseTile currentTile)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int pos = currentTile.gridPosition + directions[i];
            if (room.InGrid(pos))
            {
                WaveCollapseTile gridT = room.grid[pos.x, pos.y].GetComponent<WaveCollapseTile>();
                if (!gridT.collapsed)
                {
                    CheckCompatibility(room, gridT);
                }

            }
        }
    }

    public float [] GivePalleteChance()
    {
        return tileChancePresets[Random.Range(0, tileChancePresets.Count)];
    }
    public void CheckCompatibility(Room room, WaveCollapseTile currentTile)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int pos = currentTile.gridPosition + directions[i];
            if (room.InGrid(pos))
            {
                WaveCollapseTile gridT = room.grid[pos.x, pos.y].GetComponent<WaveCollapseTile>();
                Vector2Int temp = currentTile.gridPosition - gridT.gridPosition;
                            
                if (gridT.collapsed)
                {
                    if (gridT.type >= 0)
                    {
                        if (tileCombinations[gridT.type].TryGetValue(i, out List<int> value))
                        {
                            currentTile.CrossCheckCompatibility(value);
                        }
                        else
                        {
                            Debug.Log("Couldnt get value");
                        }
                    }
                    else
                    {


                    }
                }

            }
        }
    }
}
