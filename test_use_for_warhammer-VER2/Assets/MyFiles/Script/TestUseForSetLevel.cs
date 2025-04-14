using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUseForSetLevel : MonoBehaviour
{
    public EnemyControl enemyControl;
    public GameObject enemyPrefab;
    private void Start()
    {
        enemyControl = enemyPrefab.GetComponent<EnemyControl>();
       // enemyControl.level = 0;
    }
}
