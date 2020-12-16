using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviourSingleton<LoadManager>
{
    float minTimeToLoad = 3.0f;
    [SerializeField] GameObject loadingScene;

    public void LoadMenu()
    {
        SceneManager.LoadScene(1);
    }

    public IEnumerator Preload(int scene, Transform parent)
    {
        GameObject ls = Instantiate(loadingScene,parent);
        float loadingProgress;
        float timeLoading = 0;
        yield return null;
        if (!SceneManager.GetSceneByBuildIndex(scene).isLoaded)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            ao.allowSceneActivation = false;

            while (!ao.isDone)
            {
                timeLoading += Time.deltaTime;
                loadingProgress = ao.progress + 0.1f;
                loadingProgress = loadingProgress * timeLoading / minTimeToLoad;

                // Loading completed
                if (loadingProgress >= 1)
                {
                    ao.allowSceneActivation = true;
                }
                yield return null;
            }
        }
        Destroy(ls);
        AkSoundEngine.PostEvent("Inicia_menu", gameObject);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
    }

    public IEnumerator UnloadMenu(Image generalBackground)
    {
        float t = 0;
        float fadeInTime = 3.0f;
        Color backgroundVisible = generalBackground.color;
        Color backgroundNotVisible = new Color(generalBackground.color.r, generalBackground.color.g, generalBackground.color.b, 0);
        while (t < 1)
        {
            t += Time.deltaTime / fadeInTime;
            generalBackground.color = Color.Lerp(backgroundVisible, backgroundNotVisible, t);
            yield return null;
        }
        AsyncOperation ao = SceneManager.UnloadSceneAsync(1);
        ao.allowSceneActivation = false;
        while (!ao.isDone)
        {
            yield return null;
        }
        ao.allowSceneActivation = true;
    }
}
