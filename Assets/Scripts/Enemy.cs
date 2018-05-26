using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {

    // Use this for initialization
    bool isMoving();
    Node GetNode();
    void Kill();
    void FallAndKill();
    bool CanKill(); 
}
