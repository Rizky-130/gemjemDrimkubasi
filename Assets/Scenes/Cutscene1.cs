using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene1 : MonoBehaviour
{
    [Header("UI")]
    public Image cutsceneImage;
    public CanvasGroup canvasGroup;

    [Header("Images")]
    public Sprite image1;
    public Sprite image2;

    private bool showingFirst = true;
    private bool isTransitioning = false;

    private void Start()
    {
        cutsceneImage.sprite = image1;
        canvasGroup.alpha = 1f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTransitioning)
        {
            if (showingFirst)
            {
                StartCoroutine(ChangeImage(image2));
                showingFirst = false;
            }
        }
    }

    IEnumerator ChangeImage(Sprite newImage)
    {
        isTransitioning = true;

        // Fade Out
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - t;
            yield return null;
        }

        cutsceneImage.sprite = newImage;

        // Fade In
        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t;
            yield return null;
        }

        canvasGroup.alpha = 1;
        isTransitioning = false;
    }
}