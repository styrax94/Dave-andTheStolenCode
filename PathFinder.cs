using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFinder : MonoBehaviour
{  
    public static List<Vector2Int> directions = new List<Vector2Int>()
        {
            new Vector2Int(-1, 0),
          //  new Vector2Int(-1, 1),
            new Vector2Int( 0, 1),
            //new Vector2Int( 1, 1),
            new Vector2Int( 1, 0),
          //  new Vector2Int( 1,-1),
            new Vector2Int( 0,-1),
          //  new Vector2Int(-1,-1)
        };
    public static List<Vector2Int> allDirections = new List<Vector2Int>()
        {
           // new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
           // new Vector2Int( 0, 1),
            new Vector2Int( 1, 1),
           // new Vector2Int( 1, 0),
            new Vector2Int( 1,-1),
           // new Vector2Int( 0,-1),
            new Vector2Int(-1,-1)
        };

    //main function that gets called from outside the script
    public List<Vector2Int> SearchPath(Room room, GridTile startNode, GridTile goalNode, EnemyBrain en)
    {
       
        List<Vector2Int> path = new List<Vector2Int>();

        GridTile goal = FindShortestPathAStar(room, startNode, goalNode);

        if (goal == startNode)
        {
            path.Add(startNode.gridPos);         
            return path;
        }
        GridTile current = goal;

        while (current.parent != current)
        {
            path.Add(current.gridPos);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    //use for debugging
    IEnumerator FindTheShortestPathCoro(Enemy en, Room room, GridTile startingNode, GridTile playerTile)
    {
        List<GridTile> OpenList = new List<GridTile>();
        List<GridTile> ClosedList = new List<GridTile>();

        GridTile startNode = startingNode;
        GridTile goalNode = GetGoalNode(room, startNode, playerTile, 1);
        //
        if (IsStartCloserThanGoal(startNode, goalNode, playerTile))
        {
            en.pathTracker.transform.position = new Vector3(startingNode.gridPos.x + 0.5f, startingNode.gridPos.y + 0.5f, 0f);
        }
        startNode.parent = startingNode;

        startNode.SetTotalWeight(0 + ManhattanEstimate(startingNode.gridPos, goalNode.gridPos));
        OpenList.Add(startNode);


        while (OpenList.Count != 0)
        {
            yield return new WaitForSeconds(0.5f);
            GridTile currentNode = AStarLeastCostNode(OpenList);
            en.pathTracker.transform.position = new Vector3(currentNode.gridPos.x + 0.5f, currentNode.gridPos.y + 0.5f, 0f);
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);
            if (currentNode == goalNode)
            {
                OpenList.Clear();
                ClosedList.Clear();
                continue;
                //enemy.pathTracker.transform.position = new Vector3(currentNode.gridPos.x + 0.5f, currentNode.gridPos.y + 0.5f, 0f);
            }

            List<GridTile> nodes = GetWalkableNodes(room, currentNode);

            foreach (GridTile neighbor in nodes)
            {
                //float curDist = EuclideanEstimate(currentNode.gridPos, neighbor.gridPos);
                int curDist = 1;
                if (!ClosedList.Contains(neighbor) && !OpenList.Contains(neighbor))
                {
                    neighbor.weight = curDist + currentNode.tileWeight;
                    neighbor.totalWeight = neighbor.weight + ManhattanEstimate(neighbor.gridPos, goalNode.gridPos);

                    //Store a reference to the previous node

                    neighbor.parent = currentNode;

                    //Add this to the queue of nodes to examine
                    OpenList.Add(neighbor);
                    // ClosedList.Add(neighbor);
                }
                else
                {
                    if (curDist + currentNode.weight < neighbor.weight)
                    {
                        neighbor.weight = curDist + currentNode.tileWeight;

                        neighbor.parent = currentNode;
                        en.pathTracker.transform.position = new Vector3(neighbor.gridPos.x + 0.5f, neighbor.gridPos.y + 0.5f, 0f);
                        neighbor.totalWeight = neighbor.weight + ManhattanEstimate(neighbor.gridPos, goalNode.gridPos);

                        if (OpenList.Contains(neighbor))
                        {
                            OpenList.Remove(neighbor);
                        }
                        if (ClosedList.Contains(neighbor))
                        {
                            ClosedList.Remove(neighbor);
                        }
                        OpenList.Add(neighbor);
                    }

                }
            }
        }
        Debug.Log("End Coro");
        //enemy.pathTracker.transform.position = new Vector3(startingNode.gridPos.x+0.5f, startingNode.gridPos.y + 0.5f, 0f);
    }

    //A* algorithm
    GridTile FindShortestPathAStar(Room room, GridTile startingNode, GridTile playerTile)
    {
        List<GridTile> OpenList = new List<GridTile>();
        List<GridTile> ClosedList = new List<GridTile>();

        GridTile startNode = startingNode;
        //Replaces the playerTile with a nearby tile if the player is sorrounded
        GridTile goalNode = GetGoalNode(room, startNode, playerTile, 1);
        
        //due to replacing the goal node, check if goal ended being further from start
        if (IsStartCloserThanGoal(startingNode, goalNode, playerTile))
        {
            Debug.Log("StartIsCloserThanGoal");
            return startNode;
        }

        startNode.parent = startingNode;
        //set starting weight for the start node -> add to the open list
        startNode.SetTotalWeight(0 + ManhattanEstimate(startingNode.gridPos, goalNode.gridPos));
        OpenList.Add(startNode);


        while (OpenList.Count != 0)
        {
            //Get the node with the least total weight from the open list
            //remove from the open list and add it to the closed list
            GridTile currentNode = AStarLeastCostNode(OpenList);
            OpenList.Remove(currentNode);
            ClosedList.Add(currentNode);

            //if its goal, return it
            if (currentNode == goalNode)
            {
                OpenList.Clear();
                ClosedList.Clear();
        
                return currentNode;
            }

            //Get all neighbour tiles of the current tile that are eligble to move on
            List<GridTile> nodes = GetWalkableNodes(room,currentNode);

            foreach (GridTile neighbor in nodes)
            {
                //float curDist = EuclideanEstimate(currentNode.gridPos, neighbor.gridPos);
                //Rather than using a Heuristic approach for the tile to neighbor distance, since it is a 2d game
                //I am manually setting it to one since each neighbour would be of distance of 1, this way it also allows
                //the algorthim to take into consideration the ice tile effects 
                int curDist = 1;
                //if the current neighbor hasnt been through the algorthim, record its weight and add it to the open list
                if (!ClosedList.Contains(neighbor) && !OpenList.Contains(neighbor))
                {
                    neighbor.weight = curDist +currentNode.tileWeight;
                    neighbor.totalWeight = neighbor.weight + ManhattanEstimate(neighbor.gridPos, goalNode.gridPos);

                    //Saves the reference off the current node inside the neighbor for later
                    neighbor.parent = currentNode;

                    
                    OpenList.Add(neighbor);
                    
                }
                else
                {
                    //if the neighbor has already been seen
                    //check if the current node - neighbor relationship is lower than the neighbor and parent relationship
                    //if so, update the neighbor and add it back to the open list
                    if (curDist + currentNode.weight < neighbor.weight)
                    {
                        neighbor.weight = curDist + currentNode.tileWeight;

                        neighbor.parent = currentNode;

                        neighbor.totalWeight = neighbor.weight + ManhattanEstimate(neighbor.gridPos, goalNode.gridPos);

                        if (OpenList.Contains(neighbor))
                        {
                            OpenList.Remove(neighbor);
                        }
                        if (ClosedList.Contains(neighbor))
                        {
                            ClosedList.Remove(neighbor);
                        }
                        OpenList.Add(neighbor);
                    }

                }
            }
        }
        //if no path was found, return starting node     
        return startingNode;
    }

    //use to get a new goal if the current goal is sorrounded
    //main function
    public GridTile GetGoalNode(Room room, GridTile startNode, GridTile pNode, int multi)
    {
        GridTile min = pNode;
        List<GridTile> availableTiles = new List<GridTile>();
        bool sorrounded = true;
        foreach (var dir in directions)
        {
            Vector2Int pos = pNode.gridPos + (dir * multi);
            if (room.InGrid(pos))
            {
                GridTile temp = room.grid[pos.x, pos.y].GetComponent<GridTile>();
                if (temp.tileType >= 0 && temp.contains == null)
                {
                    sorrounded = false;
                    if (multi > 1)
                    {
                        availableTiles.Add(temp);
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }

        if (sorrounded)
        {
            foreach (var dir in allDirections)
            {
                Vector2Int pos = pNode.gridPos + (dir * multi);
                if (room.InGrid(pos))
                {
                    GridTile temp = room.grid[pos.x, pos.y].GetComponent<GridTile>();
                    if (temp.tileType >= 0 && temp.contains == null && !IsPathAvailable(room, temp))
                    {
                        sorrounded = false;
                        availableTiles.Add(temp);
                    }
                }
            }
        }
        if (availableTiles.Count > 0)
        {
            return ShortestNewGoal(availableTiles, startNode);
        }
        if (sorrounded)
        {
            if (multi > 5) return min;  //failsafe to reduce n loops       
            return min = GetGoalNode(room, startNode, pNode, multi + 1);
        }

        return min;
    }
    //Check if theres a path from the N-W-S-E direcion in a magnitude of 1
    public bool IsPathAvailable(Room room, GridTile tile)
    {
        foreach (var dir in directions)
        {
            Vector2Int pos = tile.gridPos + dir;
            if (room.InGrid(pos))
            {
                GridTile temp = room.grid[pos.x, pos.y].GetComponent<GridTile>();
                if (temp.tileType >= 0 && temp.contains == null)
                {
                    return false;
                }
            }
        }
        return true;
    }
    // get the heuristic shortest goal if more than one is available
    public GridTile ShortestNewGoal(List<GridTile> list, GridTile startNode)
    {
        GridTile min = startNode;
        int distance = 10000;


        for (int i = 0; i < list.Count; i++)
        {
            int temp = ManhattanEstimate(list[i].gridPos, startNode.gridPos);
            if (temp < distance)
            {
                min = list[i];
                distance = temp;
            }
        }

        return min;
    }


    public GridTile AStarLeastCostNode(List<GridTile> tiles)
    {
        GridTile minimum = tiles[0];
        foreach (GridTile tile in tiles)
        {
            if (minimum.totalWeight > tile.totalWeight)
            {
                minimum = tile;
            }
        }

        return minimum;
    }   
    
    List<GridTile> GetWalkableNodes(Room room, GridTile curr)
    {
        List<GridTile> walkableNodes = new List<GridTile>();

        foreach (Vector2Int dir in directions)
        {
            Vector2Int nCoordinate = curr.gridPos + dir;

            //if the neighbor position is part of the grid and its available to enter, add it to the walkable nodes
            if (room.InGrid(nCoordinate))
            {
                GridTile currentN = room.grid[nCoordinate.x, nCoordinate.y].GetComponent<GridTile>();
                if(currentN.tileType >= 0)
                {
                    //If its an ice tile, return the outcome of sliding on the ice tile and add it as the neighbor
                    if (currentN.tileType == 1 && currentN.contains == null)
                    {                     
                        GridTile iceTileOutcome = IceNeighbor(room, currentN, dir);
                        iceTileOutcome.iceEnd = true;
                        walkableNodes.Add(iceTileOutcome);
                    }
                    else if (currentN.contains != null)
                    {
                        if (currentN.contains.gameObject.tag == "Player")
                        {
                            walkableNodes.Add(currentN);
                        }
                    }
                    else
                    {
                        walkableNodes.Add(currentN);
                    }                  
                }
            }
        }

        return walkableNodes;
    }

    //Continues checking if a tile is ice and free to move on, until it is not ice or cant be move on, stop and return 
    //the tile to be used as the new neighbor
    public GridTile IceNeighbor(Room room, GridTile tile, Vector2Int direction)
    {
        Vector2Int tempPos = tile.gridPos + direction;
       
        if (room.InGrid(tempPos))
        {          
            GridTile temp = room.grid[tempPos.x, tempPos.y].GetComponent<GridTile>();
            GridTile oldTemp = temp;
            if (temp.tileType >= 0 && temp.contains == null)
            {
                while (temp.tileType == 1)
                {
                    tempPos += direction;
                    if (room.InGrid(tempPos))
                    {
                        temp = room.grid[tempPos.x, tempPos.y].GetComponent<GridTile>();
                        if (temp.tileType >= 0 && temp.contains == null)
                        {
                            oldTemp = temp;
                        }
                        else return oldTemp;
                    }
                    else return oldTemp;
                }

                return temp;
            }    
            else return tile;
        }
        else
            return tile;
       
    }


    public bool IsStartCloserThanGoal(GridTile start, GridTile goal, GridTile player)
    {
       // Debug.Log($"{start.contains.GetComponent<EnemyBrain>().enemyID}");
       
        return ManhattanEstimate(start.gridPos, player.gridPos) <= ManhattanEstimate(goal.gridPos, player.gridPos);     
    }
    
   
    //Uses the euclidean estimate for the hueristic method
    public int ManhattanEstimate(Vector2Int node, Vector2Int goal)
    {
        return Mathf.Abs(node.x - goal.x) + Mathf.Abs(node.y - goal.y);
    }


    public float EuclideanEstimate(Vector2Int node, Vector2Int goal)
    {
        return Mathf.Sqrt(Mathf.Pow(node.x - goal.x, 2) +
        Mathf.Pow(node.y - goal.y, 2));

    }
}

