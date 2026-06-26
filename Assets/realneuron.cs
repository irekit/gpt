using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float sensitivity = 2;
    private SpriteRenderer circleglowrender;
    [SerializeField] private SpriteRenderer sprend;
    private lens master;
    public float ininc = 0;
    private InputAction scroll;
    float aiset = 1;
    [SerializeField] private InputActionAsset inp;
    [SerializeField] private float scalmult = 0.01f;
    [SerializeField] private float voltspeed = 0.05f;
    [SerializeField] private float incoming_multiplier = 0.03f;
    [SerializeField] private bool l1neuron = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
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
    [SerializeField] private float startsupps = 0.01f;
    // Update is called once per frame
    bool startt = false;
    void Update()
    {
        Vector2 del = scroll.ReadValue<Vector2>();
        Vector2 mp = Mouse.current.position.ReadValue();
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(new Vector3(mp.x, mp.y, 10f));
        if (master.started && startsuppress <= 0 && !(startt && l1neuron))
        {
            incoming += ininc;
            startsuppress = startsupps;
            aiset = 1 + (Random.Range(0, 2) * 2 - 1) * 0.25f;
        }
        if(startsuppress > 0)
        {
            startsuppress -= Time.deltaTime;
        }
        if(aiset < 0.975f)
        {
            aiset += 0.015f;
        }
        else if(aiset > 1.025f)
        {
            aiset -= 0.015f;
        }
        else
        {
            aiset = 1;
        }
        if (!spiking)
        {
            if (incoming > 0.05f || incoming < -0.05f)
            {
                float inc = incoming * incoming_multiplier;
                voltage += inc;
                incoming -= inc;
            }
            else
            {
                incoming = 0;
                voltage = Mathf.Lerp(voltage, resting_volt, voltspeed);
            }
            
        }
        if (voltage > threshold && !spiking)
        {
            incoming += voltage - threshold;
            voltage = threshold;
            StartCoroutine(Fire());
        }
        if (!spiking)
        {

            float avoltage = Mathf.Clamp(voltage + incoming, -2, 1000000);
            float sca = Mathf.Max(voltage * 0.05f + 1.5f, 1.5f);
            float lightness = Mathf.Clamp(0.75f + avoltage * 0.05f, 0.1f, 1);
            float scaura = Mathf.Max(Mathf.Log(0.053f * avoltage - 0.045f) * 0.95f + 3, 0);
            aura.transform.localScale = new Vector3(scaura, scaura, 1);
            sprend.color = new Color(lightness, lightness, lightness, 1);
            transform.localScale = new Vector3(sca, sca, 1);
            for (int i = 0; i < axons.Length; i++)
            {
                blocks[i].SetFloat("_lerpos", -10);
                axons[i].SetPropertyBlock(blocks[i]);
            }
        }
        if (Vector3.Distance(transform.position, mousepos) < 1.2f && first_neuron)
        {
            ininc += del.y * sensitivity;
            ininc = Mathf.Clamp(ininc, 0, 75);
        }
        if (first_neuron)
        {
            electric.transform.localScale = new Vector3(electric.transform.localScale.x, ininc * scalmult * aiset, 1);

        }
    }
    WaitForSeconds wai;
    IEnumerator Fire()
    {
        if (l1neuron && !startt)
        {
            startt = true;
        }
        //neuron firin
        circleglow.SetActive(true);
        circleglowrender.color = new Color(1, 1, 1, 0.1f);
        circleglow.transform.localScale = new Vector3(0.51f, 0.51f, 1);
        spiking = true;
        voltage = threshold;
        float plac = 0;
        
        for(int i = 0; i < 16; i++)
        {
            voltage += 4f;
            circleglowrender.color = new Color(1, 1, 1, i * 0.0625f + 0.0625f);
            yield return null;
        }
        for(int i = 0; i < 3; i++)
        {
            voltage -= 20f;
            yield return null;
        }
        voltage = threshold + 1;
        for (int i = 0; i < 20; i++)
        {
            voltage -= (float)(threshold + 20) / 20f;
            circleglow.transform.localScale = new Vector3(0.505f + i * 0.05f, 0.505f + i * 0.05f, 1);
            circleglowrender.color = new Color(1, 1, 1, 0.95f - i * 0.05f);
            for (int j = 0; j < axons.Length; j++)
            {
                blocks[j].SetFloat("_lerpos", plac);
                axons[j].SetPropertyBlock(blocks[j]);
            }
            plac += 0.1f;
            yield return null;
        }
        
        for (int j = 0; j < axons.Length; j++)
        {
            blocks[j].SetFloat("_lerpos", -100);
            axons[j].SetPropertyBlock(blocks[j]);
        }
        voltage = -10;
        for(int i = 0; i < passon.Length; i++)
        {
            passon[i].incoming += weights[i];
        }
        circleglow.SetActive(false);
        spiking = false;
    }
}
