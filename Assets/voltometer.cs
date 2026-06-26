using Unity.VisualScripting;
using UnityEngine;

public class voltometer : MonoBehaviour
{
    private LineRenderer linerend;
    [SerializeField] private realneuron neuronscript;
    Vector3[] positions;
    private lens master;
    [SerializeField] private float scal = 0.007f;
    float summary = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        linerend = GetComponent<LineRenderer>();

        positions = new Vector3[60];
        for(int i = 0; i < 60; i++)
        {
            positions[i] = new Vector3(-1.1f + i * 0.037f, 0, -1);
        }
        master = GameObject.FindGameObjectWithTag("Player").GetComponent<lens>();
    }
    int et = 0;
    // Update is called once per frame
    void Update()
    {
        if (master.started)
        {
            //for(int i = 0; i < 40; i++)
            //{
              //  positions[i].y = 0;
            //}
        }
        summary = Mathf.Lerp(summary, neuronscript.voltage, 1f);
        if (et == 3)
        {
            et = 0;
            for (int i = 0; i < 59; i++)
            {
                positions[i].y = positions[i + 1].y;
            }
            
            positions[59].y = summary * scal;

            Vector3[] vecs = new Vector3[60];
            for (int i = 0; i < 60; i++)
            {
                vecs[i] = transform.TransformPoint(positions[i]);
            }
            linerend.SetPositions(vecs);
        }
        et++;
    }
}
