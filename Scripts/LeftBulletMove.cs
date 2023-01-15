using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBulletMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * 4 * Time.deltaTime;
    }
    
}
