using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class controller : MonoBehaviour
{
    [SerializeField] private InputActionAsset inp;
    private InputAction look;
    private InputAction move;
    [SerializeField] private GameObject squid;
    [SerializeField] private GameObject[] deactglass;
    Vector3 original_my_squid;
    [SerializeField] private Material white_squid;
    void Start()
    {
        trol = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        look = inp.FindAction("Player/Look");
        move = inp.FindAction("Player/Move");
        look.Enable();
        move.Enable();
        v = new Vector4[8];
        for (int i = 0; i < 8; i++)
        {
            v[i] = Vector4.zero;
        }
        og_markp = marker.transform.localPosition;
        original_my_squid = my_squid.transform.localPosition;

        transform.position = maingame.data.player_position;
        weapon = maingame.data.weapon;
        if(weapon != 0)
        {
            gun.SetActive(false);
            gunref.SetActive(true);
            gunref.transform.position = maingame.data.gun_position;
        }
        if(weapon != 2)
        {
            my_squid.SetActive(false);
            squid.SetActive(true);
            squid.transform.position = maingame.data.squid_position;
        }
        if(weapon != 1)
        {
            marker.SetActive(false);
            heistm.SetActive(true);
        }
        if(weapon == 0)
        {
            gunref.SetActive(false);
            gun.SetActive(true);
        }
        else if(weapon == 1)
        {
            marker.SetActive(true);
            heistm.SetActive(false);
        }
        else if(weapon == 2)
        {
            squid.SetActive(false);
            my_squid.SetActive(true);
        }
        if (maingame.data.squid_active)
        {
            squid.GetComponent<Animator>().enabled = false;
            squid.GetComponent<BoxCollider>().enabled = true;
            squidw.GetComponent<SkinnedMeshRenderer>().material = white_squid;
            for (int i = 0; i < deactglass.Length; i++)
            {
                deactglass[i].SetActive(false);
            }
        }
    }
    public int weapon = 0;
    private CharacterController trol;
    [SerializeField] private GameObject squidw;
    [SerializeField] private float speed;
    [SerializeField] private float sens;
    [SerializeField] private float gravity;
    [SerializeField] private float jum;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject gunhead;
    [SerializeField] private int max_bul;
    [SerializeField] private Material wallmat;
    [SerializeField] private float fadespeed;
    [SerializeField] private GameObject gunref;
    [SerializeField] private GameObject heistm;
    [SerializeField] private GameObject marker;
    [SerializeField] private GameObject markd;
    [SerializeField] private GameObject cap;
    [SerializeField] private Color empc;
    [SerializeField] private GameObject my_squid;
    Vector3 og_markp;
    int bullets = 10;
    private bool markeron = false;
    float veloc = 0;
    float xrot = 0;
    float coold = 0;
    Vector4[] v;
    [SerializeField]Texture2D whiteboard;
    [SerializeField] ParticleSystem bulletcase;
    Vector2 last_penpos;
    bool lastposreal = false;
    void Draw(Vector2 coords)
    {
        int uvx = (int)(coords.x * whiteboard.width);
        int uvy = (int)(coords.y * whiteboard.height);
        uvx = Mathf.Clamp(uvx, 0, whiteboard.width - 1);
        uvy = Mathf.Clamp(uvy, 0, whiteboard.height - 1);
        int siz = Keyboard.current.shiftKey.isPressed ? 4 : 1;
        for (int i = -siz; i <= siz; i++)
        {
            for (int j = -siz; j <= siz; j++)
            {
                int uvxn = uvx + i;
                int uvyn = uvy + j;
                if (uvxn >= 0 && uvxn < whiteboard.width && uvyn >= 0 && uvyn < whiteboard.height)
                {
                    if (Keyboard.current.shiftKey.isPressed)
                    {
                        whiteboard.SetPixel(uvxn, uvyn, empc);
                    }
                    else
                    {
                        whiteboard.SetPixel(uvxn, uvyn, Color.black);

                    }

                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 delt = look.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * delt.x * sens * Time.deltaTime);
        xrot -= delt.y * sens * Time.deltaTime;
        xrot = Mathf.Clamp(xrot, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        Vector2 wasd = move.ReadValue<Vector2>();
        veloc -= gravity * Time.deltaTime;
        if (Keyboard.current.spaceKey.isPressed && trol.isGrounded)
        {
            veloc = jum;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if(coold <= 0 && weapon == 0)
            {
                StartCoroutine(Twirl());

            }
            if(weapon == 1)
            {
                if (markeron)
                {
                    StartCoroutine(MarkerOff());
                }
                else
                {
                    StartCoroutine(MarkerOn());
                }
                markeron = !markeron;
            }
        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            maingame.SaveGame();
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (coold < 10 && weapon == 0)
            {
                coold = 0.2f;
                StartCoroutine(Shoot());
            }
        }
        if (Mouse.current.leftButton.isPressed)
        {
            if (coold <= 0 && weapon == 0)
            {
                coold = 0.15f;
                StartCoroutine(Shoot());
            }
            else if(weapon == 1)
            {
                RaycastHit hit;
                if (markeron && Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3) && hit.collider.CompareTag("whiteboard"))
                {
                    if (lastposreal)
                    {
                        for(float i = 0;  i <= 3; i++)
                        {
                            Vector2 newppos = Vector2.Lerp(hit.textureCoord, last_penpos, i / 3);
                            Draw(newppos);
                        }
                    }
                    else
                    {
                        Draw(hit.textureCoord);
                    }
                        last_penpos = hit.textureCoord;
                    lastposreal = true;
                    marker.transform.position = hit.point;

                    whiteboard.Apply();
                }
                else
                {
                    marker.transform.localPosition = og_markp;
                    lastposreal = false;
                }
            }
        }
        else
        {
            marker.transform.localPosition = og_markp;
            lastposreal = false;
        }
        if (coold > 0)
        {
            coold -= Time.deltaTime;
        }
        for (int i = 0; i < 8; i++)
        {
            if (v[i].w > 0)
            {
                v[i].w -= Time.deltaTime * fadespeed;
            }
            wallmat.SetVector("_v" + (i + 1), v[i]);
        }
        //shift for eraser
        if (Keyboard.current.shiftKey.wasPressedThisFrame || Keyboard.current.shiftKey.wasReleasedThisFrame)
        {
            StartCoroutine(MarkerTurn());
        }
            trol.Move((wasd.x * transform.right + Vector3.up * veloc * Time.deltaTime + wasd.y * transform.forward) * Time.deltaTime * speed);
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            int lsd = weapon;
            DropWeapon(weapon);
            RaycastHit hit;
            if (lsd == -1 && Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3))
            {
                if(hit.collider.gameObject == gunref)
                {
                    gunref.SetActive(false);
                    gun.SetActive(true);
                    weapon = 0;
                }
                else if(hit.collider.gameObject == heistm && lsd != 1)
                {
                    heistm.SetActive(false);
                    marker.SetActive(true);
                    weapon = 1;
                }
                else if (hit.collider.CompareTag("squid"))
                {
                    squid.SetActive(false);
                    my_squid.SetActive(true);
                    my_squid.transform.localPosition = original_my_squid;
                    weapon = 2;
                }
            }
        }
        maingame.data.player_position = transform.position;
        maingame.data.gun_position = gunref.transform.position;
        maingame.data.squid_position = squid.transform.position;
        maingame.data.weapon = weapon;
    }
    IEnumerator MarkerOn()
    {
        float ysp = 0;
        for(int i = 0; i < 25; i++)
        {
            ysp -= 0.04f;
            cap.transform.Translate(0, ysp, 0.5f);
            cap.transform.Rotate(0, -5, 0);
            yield return new WaitForSeconds(0.015f);
        }
    }
    IEnumerator MarkerTurn()
    {
        for(int i = 0; i < 18; i++)
        {
            yield return null;
            markd.transform.Rotate(0, 10, 0);
        }
    }
    IEnumerator MarkerOff()
    {
        float ysp = -1f;
        for(int i = 0; i < 25; i++)
        {
            cap.transform.Rotate(0, 5, 0);
            cap.transform.Translate(0, -ysp, -0.5f);
            ysp += 0.04f;
            yield return new WaitForSeconds(0.015f);
        }
    }
    void DropWeapon(int weaponnum)
    {
        if(weaponnum == 0)
        {
            gunref.SetActive(true);
            gunref.transform.position = gun.transform.position;
            gunref.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 200);
            gunref.GetComponent<Rigidbody>().AddTorque(Vector3.forward * 40);
            gun.SetActive(false);
            weapon = -1;
        }
        else if(weaponnum == 1)
        {
            heistm.SetActive(true);
            marker.SetActive(false);
            weapon = -1;
            if (markeron)
            {
                markeron = false;
                StartCoroutine(MarkerOff());
            }
        }
        else if(weaponnum == 2)
        {
            StartCoroutine(SquidDrop());
        }
    }
    IEnumerator SquidDrop()
    {
        for(int i = 0; i < 10; i++)
        {
            my_squid.transform.Translate(0, -Time.deltaTime * 10, 0);
            yield return null;
        }
        my_squid.transform.localPosition = original_my_squid;
        my_squid.SetActive(false);
        weapon = -1;
        squid.SetActive(true);
        Vector3 squid_p = transform.position;
        squid_p.y = -1.75f;
        squid.transform.position = squid_p;
    }
    IEnumerator Shoot()
    {
        bullets -= 1;
        if(bullets <= 0)
        {
            bullets = max_bul;
            coold = 100000;
            Quaternion lsr = gun.transform.localRotation;
            Vector3 lsp = gun.transform.localPosition;
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gunhead.transform.Translate(-0.1f, 0, 0);
            }
            for (int i = 0; i < 5; i++)
            {
                gun.transform.Translate(-0.03f, -0.1f, 0);
                gun.transform.Rotate(-6f, 0, 0);
                yield return new WaitForSeconds(0.015f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gun.transform.Translate(0, 0.01f, 0);
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gun.transform.Translate(0, -0.01f, 0);
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < 5; i++)
            {
                gun.transform.Translate(0.03f, 0.1f, 0);
                gun.transform.Rotate(6f, 0, 0);
                yield return new WaitForSeconds(0.015f);
            }
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gunhead.transform.Translate(0.1f, 0, 0);
            }
            gun.transform.localRotation = lsr;
            gun.transform.localPosition = lsp;

            yield return new WaitForSeconds(0.2f);
            coold = 0.1f;
            yield break;
        }
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            if (hit.collider.CompareTag("waterglass"))
            {
                maingame.data.squid_active = true;
                squid.GetComponent<Animator>().enabled = false;
                squid.GetComponent<BoxCollider>().enabled = true;
                squidw.GetComponent<SkinnedMeshRenderer>().material = white_squid;
                squid.transform.position = new Vector3(squid.transform.position.x, -1.75f, squid.transform.position.z);
                for(int i =0; i < deactglass.Length;i++)
                {
                    deactglass[i].SetActive(false);
                }
            }
            if (hit.collider.CompareTag("wall"))
            {
                int ind = -1;
                int maxind = 0;
                float minim = 10;
                for(int i = 0; i < 8; i++)
                {
                    if (v[i].w < minim)
                    {
                        maxind = i;
                        minim = v[i].w;
                    }
                    if(v[i].w <= 0)
                    {
                        ind = i;
                        break;
                    }
                }
                if(ind == -1)
                {
                    ind = maxind;
                }
                v[ind] = new Vector4(hit.point.x, hit.point.y, hit.point.z, 1);
            }
            if (hit.collider.CompareTag("squid"))
            {
                squid.transform.Rotate(0, UnityEngine.Random.Range(-20f, 20f), 0);
            }
        }
        bulletcase.Emit(1);
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.015f);
            gunhead.transform.Translate(-0.1f, 0, 0);
            gun.transform.Rotate(0, 0, 4.5f);
        }
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.015f);
            gunhead.transform.Translate(0.1f, 0, 0);
            gun.transform.Rotate(0, 0, -4.5f);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("lenstable"))
        {
            if (Keyboard.current.qKey.isPressed && weapon == 2)
            {
                maingame.SaveGame();
                SceneManager.LoadScene(1);
            }
        }
    }
    IEnumerator Twirl()
    {
        float wai = -24.5f;
        for(int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(0.02f);
            wai += 1f;
            gun.transform.Rotate(0, 0, wai);
        }
        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(0.02f);
            wai -= 1f;
            gun.transform.Rotate(0, 0, wai);
        }
    }
}
