using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe_Attack : EnemyAttack
{
    public override void Start()
    {
        base.Start();
    }


    void Update()
    {
        
    }

    public override void Attack(Vector2Int currentPos)
    {
        base.Attack(currentPos);
        GetComponentInParent<Turn>().TurnFinished.Invoke();
    }


    public override bool CanAttack(Vector2Int currentPos, Vector2Int targetPos)
    {

        if (attackState == AttackState.charging) return true;
        for (int i = 0; i < directions.Length; i++)
        {
            int count = 0;
            Vector2Int tempDir = currentPos;
            while (count < attackPattern.attackRange)
            {
                tempDir += directions[i];
                if (tempDir == targetPos)
                {
                    faceDirection = i;
                    if (i == 1) GetComponentInParent<EnemyBrain>().FlipSprites(true);
                    else if (i == 3) GetComponentInParent<EnemyBrain>().FlipSprites(false);
                    return true;
                }
                count++;
            }
        }
        return false;
    }
}
