using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour 
{
	public float speed;
    private Rigidbody2D rb;

	void Start ()
	{
	    if (GetComponent<Transform>().position.x > 0)
	    {
	        speed *= -1;
	    }
	    rb = GetComponent<Rigidbody2D>();
	    rb.velocity = new Vector2(speed, 0);
    }
	

}
