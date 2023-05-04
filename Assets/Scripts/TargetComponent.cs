using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetComponent : MonoBehaviour{
    
    public List<string> neighbours;
    public Vector2 position => transform.position;
  
	private void OnDestroy() {
        neighbours.Clear();

    }
}
