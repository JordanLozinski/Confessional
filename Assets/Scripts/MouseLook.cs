using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseLook : MonoBehaviour
{

    public Camera cam;
    // Update is called once per frame
    Rigidbody rb;

    public AudioSource shotsound;
    public AudioSource shellfallsound;

    public Sprite defaultSprite;
    public Sprite flareSprite;

    public GameObject fadeImage;

    bool coroutineStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
        StartCoroutine(Timer());
    }
    
    void FixedUpdate()
    {
        Vector3 targetPosition = cam.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;
        transform.position = targetPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !coroutineStarted)
        {
            StartCoroutine(ShootGun());
            coroutineStarted = true;
        }
    }


    IEnumerator Timer()
    {
        yield return new WaitForSeconds(12f);
        StartCoroutine(FadeOut());
    }    

    IEnumerator ShootGun()
    {
        shotsound.PlayOneShot(shotsound.clip);
        GetComponent<SpriteRenderer>().sprite = flareSprite;
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
        yield return new WaitForSeconds(0.04f);
        shellfallsound.PlayOneShot(shellfallsound.clip);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        fadeImage.SetActive(true);
        Image fadeImageImage = fadeImage.GetComponent<Image>();
        Color tmpa = fadeImageImage.color;
        tmpa.a = 0.0f;
        fadeImageImage.color = tmpa;
        while (fadeImageImage.color.a < 1.0f)
        {
            Color tmp = fadeImageImage.color;
            tmp.a += (1.1f - fadeImageImage.color.a)*0.1f;
            fadeImageImage.color = tmp;
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);
        // Display "Fin"
        fadeImage.transform.GetChild(0).gameObject.SetActive(true);
    }
}
