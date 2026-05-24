using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class realneuron : MonoBehaviour
{
    [SerializeField] private GameObject[] passon;
    [SerializeField] private float[] weights;
    float voltage = 0;
    public float incoming = 0;
    [SerializeField] private float threshold = 1;
    private bool spiking = false;
    [SerializeField] private float resting_volt = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(incoming > 0.1f)
        {
            if (!spiking)
            {
                voltage += Time.deltaTime * 50;
                incoming -= Time.deltaTime * 50;
            }
        }

        else if(incoming < -0.1f)
        {
            if (!spiking)
            {
                voltage -= Time.deltaTime * 50;
                incoming += Time.deltaTime * 50;
            }
        }
        else
        {
            incoming = 0;
        }
        if (!spiking)
        {
            if (voltage < resting_volt - 0.1f)
            {
                voltage += Time.deltaTime * 0.5f;
            }
            else if (voltage > resting_volt + 0.1f)
            {
                voltage -= Time.deltaTime * 0.5f;
            }
            else
            {
                voltage = resting_volt;
            }
        }
        if (voltage > threshold && !spiking)
        {
            StartCoroutine(Fire());
        }
        float sca = Mathf.Max(voltage + 1, 0.1f);
        transform.localScale = new Vector3(sca, sca, 1);
    }
    IEnumerator Fire()
    {
        spiking = true;
        voltage = threshold;
        for(int i = 0; i < 5; i++)
        {
            voltage += 0.2f;
            yield return new WaitForSeconds(0.01f);
        }
        for (int i = 0; i < 5; i++)
        {
            voltage -= 0.6f;
            yield return new WaitForSeconds(0.01f);
        }
        spiking = false;
    }
}
