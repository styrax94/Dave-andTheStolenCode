using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public enum AttackState {idle, charging};
    public AttackState attackState = AttackState.idle;
    [SerializeField] public AttackPattern attackPattern;
    [SerializeField] private GameObject attackSquarePrefab;
    public Vector2Int[] directions = new Vector2Int[]
   {
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(1,0)
   };
    public List<GameObject> attackSquares;
    public int faceDirection;
    public int dmg = 1;
    public string whichAttack;

    public virtual void Start()
    {
        attackSquares = new List<GameObject>();
        dmg = 1;
        foreach (var attSqr in attackPattern.attackSquares)
        {
            GameObject obj = Instantiate(attackSquarePrefab, transform);
            attackSquares.Add(obj);
        }
    }
    public virtual void Attack(Vector2Int currentPos)
    {
        switch (attackState)
        {
            case AttackState.idle:
                {
                    for (int i = 0; i < attackSquares.Count; i++)
                    {
                        Vector3 position = Vector3.zero;
                        switch (faceDirection)
                        {
                            case 0:
                                position = new Vector3(-attackPattern.attackSquares[i].x, -attackPattern.attackSquares[i].y, 0);

                                break;
                            case 1:
                                position = new Vector3(-attackPattern.attackSquares[i].y, -attackPattern.attackSquares[i].x, 0);
                                break;
                            case 2:
                                position = new Vector3(attackPattern.attackSquares[i].x, attackPattern.attackSquares[i].y, 0);
                                break;
                            case 3:
                                position = new Vector3(attackPattern.attackSquares[i].y, attackPattern.attackSquares[i].x, 0);
                                break;
                        }
                        position += new Vector3(currentPos.x, currentPos.y, 0);
                        attackSquares[i].transform.position = position;
                        if (GetComponentInParent<EnemyBrain>().room.InGrid(new Vector2Int((int)position.x, (int)position.y)))
                        {
                            attackSquares[i].SetActive(true);

                        }

                    }
                    attackState = AttackState.charging;
                    break;
                }
               
            case AttackState.charging:
                {
                    foreach (var attSq in attackSquares)
                    {
                        if (GetComponentInParent<EnemyBrain>().room.InGrid(new Vector2Int((int)attSq.transform.position.x, (int)attSq.transform.position.y)))
                        {
                            attSq.GetComponent<AttackAnimation>().PlayAnimation();
                            GridTile tile = GetComponentInParent<EnemyBrain>().room.grid[(int)attSq.transform.position.x, (int)attSq.transform.position.y].GetComponent<GridTile>();
                            if(tile.contains!= null)
                            {
                                if(tile.contains.tag == "Player")
                                {
                                    tile.contains.GetComponent<ResourceManager>().UpdateHitPoint(-dmg);
                                }
                            }
                        }
                    }
                    attackState = AttackState.idle;
                    break;
                }

        }

    }
    public virtual bool CanAttack(Vector2Int currentPos, Vector2Int targetPos)
    {
        return true;
    }
}
