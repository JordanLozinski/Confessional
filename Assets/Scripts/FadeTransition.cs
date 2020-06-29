using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn;
using Yarn.Unity;

public class FadeTransition : MonoBehaviour
{
    public GameObject fadeImage;
    private Text childText;

    [YarnCommand("transition")]
    public void DoSceneTransition(string[] textToDisplay)
    {
        Debug.Log(textToDisplay);
        childText = fadeImage.GetComponentsInChildren<Text>()[0];
        childText.text = textToDisplay[0];
        StartCoroutine(FadeBetween());
    }

    public IEnumerator FadeBetween()
    {
        fadeImage.SetActive(true);
        Image fadeImageImage = fadeImage.GetComponent<Image>();
        Color tmpa = fadeImageImage.color;
        tmpa.a = 0.0f;
        Color tmpb = childText.color;
        tmpb.a = 0.0f;
        fadeImageImage.color = tmpa;
        childText.color = tmpb;
        while (fadeImageImage.color.a < 1.0f)
        {
            Color tmp = fadeImageImage.color;
            tmp.a += (1.1f-fadeImageImage.color.a)*0.1f;
            fadeImageImage.color = tmp;
            Color tmp2 = childText.color;
            tmp2.a = tmp.a;
            childText.color = tmp2;
            
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        Debug.Log("sadknadaoisjdoa");
        while (fadeImageImage.color.a > 0.0f)
        {
            Color tmp = fadeImageImage.color;
            tmp.a -= fadeImageImage.color.a*0.05f + 0.001f;
            fadeImageImage.color = tmp;
            Color tmp2 = childText.color;
            tmp2.a = tmp.a;
            childText.color = tmp2;

            yield return null;
        }
        fadeImage.SetActive(false);
    }
}
