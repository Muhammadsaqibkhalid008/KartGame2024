using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    // firstly the logo_part_1 will be displayed and then play its animation (going a little up)

    [SerializeField] GameObject logo_part_1;
    [SerializeField] GameObject logo_part_2;
    [SerializeField] float logo_part_1_displayTime = 1f;
    [SerializeField] float logo_part_2_displayTime = 1f;
    [SerializeField] float canvasGroupLoadingTime = 1f;

    [SerializeField] CanvasGroup logoParentCanvasGroup;
    [SerializeField] int selectionSceneIndex = 1;
    [SerializeField] Transform loadingIcon;

    [SerializeField] float fillingSpeedMultiplier = 1f;
    [SerializeField] float logoMoverMovingSpeed = 1f;
    [SerializeField] RectTransform endPointOfLogoMover;
    [SerializeField] RectTransform logoMoverTransform;

    [SerializeField] Transform newIconCanvas;

    private bool logo_2_appeared = false;//
    private float timer = 0f;

    private void Start()
    {
        this.logo_part_1.SetActive(false);
        this.logo_part_2.SetActive(false);
        this.Invoke(nameof(DisplayLogo_part_1), this.logo_part_1_displayTime);

        // getting the canvas group parent
        this.logoParentCanvasGroup.GetComponent<Animator>().enabled = false;
        this.loadingIcon.gameObject.SetActive(false);

        this.logo_2_appeared = false;

        newIconCanvas.gameObject.SetActive(false);

    }

    private void Update()
    {
        if (this.logo_2_appeared)
        {
            timer += Time.deltaTime;
            float per = timer / logoMoverMovingSpeed;
            logoMoverTransform.position = Vector2.Lerp(logoMoverTransform.position, endPointOfLogoMover.position, Mathf.SmoothStep(0, 1, per));

            // we need to keep the filling amoung as well
            logo_part_2.GetComponent<Image>().fillAmount = Mathf.SmoothStep(0, 1, per) * fillingSpeedMultiplier;
        }
    }

    void DisplayLogo_part_1()
    {
        this.logo_part_1.SetActive(true);
        // the animation will be played automatically when this gameObject turns on

        this.Invoke(nameof(DisplayLogo_part_2), this.logo_part_2_displayTime);
    }

    void DisplayLogo_part_2()
    {
        this.logo_part_2.SetActive(true);
        this.logo_2_appeared = true;
        // soon as this second logo appears we need to move onto to the selection screen after a short break
        this.Invoke(nameof(DisplayCanvasGroup), this.canvasGroupLoadingTime);
    }

    void DisplayCanvasGroup()
    {
        this.logo_part_1.transform.SetParent(this.logoParentCanvasGroup.transform);
        this.logo_part_2.transform.SetParent(this.logoParentCanvasGroup.transform);
        this.logoParentCanvasGroup.GetComponent<Animator>().enabled = true;
        StartCoroutine(LoadSceneAsync());
    }



    IEnumerator LoadSceneAsync()
    {
        yield return new WaitForSeconds(1f);

        AsyncOperation sceneOperation = SceneManager.LoadSceneAsync(this.selectionSceneIndex);
        newIconCanvas.gameObject.SetActive(true);
        this.loadingIcon.gameObject.SetActive(true);
        while (!sceneOperation.isDone)
        {
            // until the scene operation is done we can do animated icons instead
            // rest of the code ......
            yield return null;
        }
        // scene loaded successfully

    }



}
