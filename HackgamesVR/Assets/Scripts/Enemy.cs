using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    public float speed = 100f;
    public float rotateSpeed = 10f;
    private float waitTime;
    public float startWaitTime;

    public Vector3 moveSpot;
    public Vector3 pos;

    public float maxRadius;
    public float minRadius;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }
#endif

    private void Start()
    {
        waitTime = startWaitTime;
        spotSearch();
    }

    void spotSearch()
    {
        moveSpot = Random.insideUnitSphere * (maxRadius - minRadius);

        if (moveSpot.x < 0)
            moveSpot.x -= minRadius;
        else
            moveSpot.x += minRadius;
        if (moveSpot.y < 0)
            moveSpot.y -= minRadius;
        else
            moveSpot.y += minRadius;
        if (moveSpot.z < 0)
            moveSpot.z -= minRadius;
        else
            moveSpot.z += minRadius;
    }


    private void Update()
    {
        transform.LookAt(new Vector3(moveSpot.x, moveSpot.x, moveSpot.x));
    }

    private void FixedUpdate()
    {
        Vector3 direction = moveSpot - rb.velocity;
        Vector3 tmp = Vector3.zero;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, direction, ref tmp, 0.1f, speed, Time.deltaTime);
        //rb.angularVelocity = transform.TransformDirection(moveSpot * rotateSpeed);
        if (Vector3.Distance(transform.position, moveSpot) < 1f)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity, direction, ref tmp, 0.1f, speed, Time.deltaTime);
            spotSearch();
            if (waitTime <= 0)
            {
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}
