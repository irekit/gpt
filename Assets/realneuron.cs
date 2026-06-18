using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
public class realneuron : MonoBehaviour
{
    [SerializeField] private realneuron[] passon;
    [SerializeField] private float[] weights;
    [SerializeField] private SpriteRenderer[] axons;
    private MaterialPropertyBlock[] blocks;
    public float voltage = 0;
    public float incoming = 0;
    [SerializeField] private float threshold = 1;
    private bool spiking = false;
    [SerializeField] private float resting_volt = 0;
    [SerializeField] private GameObject aura;
    [SerializeField] private GameObject circleglow;
    private SpriteRenderer circleglowrender;
    [SerializeField] private SpriteRenderer sprend;
    private lens master;
    public float ininc = 0;
    private InputAction scroll;
    [SerializeField] private InputActionAsset inp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wai = new WaitForSeconds(0.01f);
        blocks = new MaterialPropertyBlock[axons.Length];
        for(int i = 0; i < axons.Length; i++)
        {
            blocks[i] = new MaterialPropertyBlock();
            axons[i].GetPropertyBlock(blocks[i]);
        }
        circleglowrender = circleglow.GetComponent<SpriteRenderer>();
        circleglow.SetActive(false);
        master = GameObject.FindGameObjectWithTag("Player").GetComponent<lens>();
        scroll = inp.FindActionMap("UI").FindAction("ScrollWheel");
    }
    float startsuppress = 0;
    [SerializeField]bool first_neuron = false;
    [SerializeField] private GameObject electric;
    // Update is called once per frame
    void Update()
    {
        Vector2 del = scroll.ReadValue<Vector2>();
        Vector2 mp = Mouse.current.position.ReadValue();
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(new Vector3(mp.x, mp.y, 10f));
        if (master.started && startsuppress <= 0)
        {
            incoming = ininc;
            voltage = 0;
            startsuppress = 1;
        }
        if(startsuppress > 0)
        {
            startsuppress -= Time.deltaTime;
        }
        if (!spiking)
        {
            if (incoming > 0.05f || incoming < -0.05f)
            {
                float inc = incoming * 0.01f;
                voltage += inc;
                incoming -= inc;
            }
            else
            {
                incoming = 0;
                if (voltage < resting_volt - 0.1f)
                {
                    voltage += 0.005f;
                }
                else if (voltage > resting_volt + 0.1f)
                {
                    voltage -= 0.005f;
                }
                else
                {
                    voltage = resting_volt;
                }
            }
            
        }
        if (voltage > threshold && !spiking)
        {
            incoming += voltage - threshold;
            voltage = threshold;
            StartCoroutine(Fire());
        }
        float avoltage = Mathf.Clamp(voltage + incoming, -2, 1000000);
        float sca = Mathf.Max(voltage * 0.05f + 1.5f, 1.5f);
        float lightness = Mathf.Clamp(0.75f+avoltage*0.05f, 0.1f, 1);
        float scaura = Mathf.Max(Mathf.Log(0.053f*avoltage - 0.045f) * 0.95f + 3, 0);
        aura.transform.localScale = new Vector3(scaura, scaura, 1);
        sprend.color = new Color(lightness, lightness, lightness, 1);
        transform.localScale = new Vector3(sca, sca, 1);
        if (!spiking)
        {
            for (int i = 0; i < axons.Length; i++)
            {
                blocks[i].SetFloat("_lerpos", -10);
                axons[i].SetPropertyBlock(blocks[i]);
            }
        }
        Debug.Log(mousepos);
        if (Vector3.Distance(transform.position, mousepos) < 1.2f && first_neuron)
        {
            ininc += del.y * 2f;
            ininc = Mathf.Clamp(ininc, 0, 75);
        }
        if (first_neuron)
        {
            electric.transform.localScale = new Vector3(electric.transform.localScale.x, ininc * 0.01f, 1);

        }
    }
    WaitForSeconds wai;
    IEnumerator Fire()
    {
        circleglow.SetActive(true);
        circleglowrender.color = new Color(1, 1, 1, 0.1f);
        circleglow.transform.localScale = new Vector3(0.51f, 0.51f, 1);
        spiking = true;
        voltage = threshold;
        float plac = 0;
        
        for(int i = 0; i < 8; i++)
        {
            voltage += 1f;
            circleglowrender.color = new Color(1, 1, 1, i * 0.125f + 0.125f);
            yield return wai;
        }
        yield return null;
        yield return null;
        yield return null;
        voltage = threshold + 1;
        for (int i = 0; i < 10; i++)
        {
            voltage -= (float)(threshold + 5) / 10f;
            circleglow.transform.localScale = new Vector3(0.51f + i * 0.1f, 0.51f + i * 0.1f, 1);
            circleglowrender.color = new Color(1, 1, 1, 0.9f - i * 0.1f);
            for (int j = 0; j < axons.Length; j++)
            {
                blocks[j].SetFloat("_lerpos", plac);
                axons[j].SetPropertyBlock(blocks[j]);
            }
            plac += 0.1f;
            yield return wai;
        }
        for (int j = 0; j < axons.Length; j++)
        {
            blocks[j].SetFloat("_lerpos", -100);
            axons[j].SetPropertyBlock(blocks[j]);
        }
        voltage = -1;
        for(int i = 0; i < passon.Length; i++)
        {
            passon[i].incoming += weights[i];
        }
        circleglow.SetActive(false);
        spiking = false;
    }
}
