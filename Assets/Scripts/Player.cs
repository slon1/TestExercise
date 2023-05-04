using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool pauseInput { get; set; }
    public Vector2 position => transform.position;
    public float speed = 10;
    private Vector2 target;

    public bool CollisionCheck(Vector2 position) {
        return ((Vector2)transform.position - position).sqrMagnitude < 0.007f;
	}
    
    public void MoveTowards(Vector3 position, float speed) {
        transform.position = Vector2.MoveTowards(transform.position, position, Time.deltaTime * speed);
    }
    public void Move(Vector3 position) {
        Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position+ position);        
        if (viewPos.x > 0.05F && viewPos.x < 0.95F&&viewPos.y > 0.05F && viewPos.y < 0.95F)            
            transform.position += position;
    }
    
    void Update()
    {
        if (!pauseInput) {
            Vector2 dt = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (dt.sqrMagnitude > 0) {
                Move(dt * speed * Time.deltaTime);
                target = transform.position;
            }
            else {
                if (Input.GetMouseButton(0)) {
                    target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                if (!CollisionCheck(target)) {
                    MoveTowards(target, speed);
                }
            }
        }

    }

}
