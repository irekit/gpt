using Unity.VisualScripting;
using UnityEngine;

public class voltometer : MonoBehaviour
{
    private LineRenderer linerend;
    [SerializeField] private realneuron neuronscript;
    Vector3[] positions;
    private lens master;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        linerend = GetComponent<LineRenderer>();

        positions = new Vector3[20];
        for(int i = 0; i < 20; i++)
        {
            positions[i] = new Vector3(-1.1f + i * 0.115f, 0, -1);
        }
        master = GameObject.FindGameObjectWithTag("Player").GetComponent<lens>();
    }
    int et = 0;
    // Update is called once per frame
    void Update()
    {
        if (master.started)
        {
            for(int i = 0; i < 20; i++)
            {
                positions[i].y = 0;
            }
        }
        if (et == 10)
        {
            et = 0;
            for (int i = 0; i < 19; i++)
            {
                positions[i].y = positions[i + 1].y;
            }
            positions[19].y = neuronscript.voltage * 0.05f;
            Vector3[] vecs = new Vector3[20];
            for (int i = 0; i < 20; i++)
            {
                vecs[i] = transform.TransformPoint(positions[i]);
            }
            linerend.SetPositions(vecs);
        }
        else
        {
            et++;
        }
    }
}
