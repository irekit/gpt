using UnityEngine;

public class slowlyrotate : MonoBehaviour
{
    [SerializeField] private Vector3 speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime * speed.x, Time.deltaTime * speed.y, Time.deltaTime * speed.z);
    }
}
