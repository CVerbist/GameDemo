using UnityEngine;
using System.Collections;

public class JitterObject : MonoBehaviour 
{
	public float speed;
    public float JitterWait;
    public float MaxMov;
    private Rigidbody2D rb;
    private Vector3 StartPos;
	void Start ()
	{
        rb = GetComponent<Rigidbody2D>();
	    StartPos = rb.position;
        StartCoroutine(Jitter());
	}

    void Update()
    {
        Vector3 curPos = rb.position;
        if (Mathf.Abs(StartPos.x - curPos.x) > MaxMov)
        {
            rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
        }
        if (Mathf.Abs(StartPos.y - curPos.y) > MaxMov)
        {
            rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
        }
    }
	

    IEnumerator Jitter()
    {
        while (true)
        {
            rb.velocity = new Vector2(Random.Range(-speed, speed), Random.Range(-speed, speed));
            yield return new WaitForSeconds(JitterWait);
        }
        
    }

}
