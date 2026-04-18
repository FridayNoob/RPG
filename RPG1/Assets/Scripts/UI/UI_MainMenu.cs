using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;

    [SerializeField] private UI_FadeScreen fadeScreen;

    private void Start()
    {
        if (SaveManager.instance.HasSavedData() == false)
            continueButton.SetActive(false);
    }
    public void ContinueGame()
    {

        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void StartNewGame()
    {
        SaveManager.instance.DeleteSaveData();
        StartCoroutine(LoadSceneWithFadeEffect(1.5f));
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
    }

    IEnumerator LoadSceneWithFadeEffect(float _dalay)
    {
        fadeScreen.FadeOut();

        yield return new WaitForSeconds(_dalay);

        SceneManager.LoadScene(sceneName);
    }
}
