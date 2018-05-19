using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy {

    // Use this for initialization
    Node GetNode();
    void Kill();
    bool CanKill(); 
}
