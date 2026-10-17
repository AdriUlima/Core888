using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace Adri888
{
    using static Adri888Core;

    public class Core : MonoBehaviour
    {
        public string[] LocaleNames { get; private set; }
        public string CurrentLocaleName => LocalizationSettings.SelectedLocale.LocaleName;
        public Camera MainCamera { get; private set; }
        [SerializeField] private float duration = 0.6f;
        [SerializeField] private CameraController cameraController;
        public int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;

        async void Start()
        {
            await UniTask.WhenAll(Init(), LocalizationInit());
            if (core.CurrentSceneIndex == 0) core.NextScene();

            async UniTask Init()
            {
                MainCamera = Camera.main;
                prefsManager.LoadPlayerPrefs();
            }

            async UniTask LocalizationInit()
            {
                await LocalizationSettings.InitializationOperation.Task.AsUniTask();
                LocaleNames = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToArray();
            }
        }

        public void ChangeLocale(string localeName)
        {
            Locale newLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(l => l.LocaleName == localeName);
            LocalizationSettings.SelectedLocale = newLocale;
        }

        public void LoadScene(int sceneIndex, TransitionMode loadMode = TransitionMode.None)
        {
            switch (loadMode)
            {
                case TransitionMode.None:
                    SceneManager.LoadScene(sceneIndex);
                    break;
                case TransitionMode.SlideRight:
                    throw new NotImplementedException();
            }
        }
        public void LoadScene(SceneTypes sceneType, TransitionMode loadMode = TransitionMode.None) => LoadScene((int)sceneType, loadMode);
        public void ReloadScene() => LoadScene(CurrentSceneIndex);
        public void NextScene() => LoadScene(CurrentSceneIndex + 1);
        public void PreviousScene() => LoadScene(CurrentSceneIndex - 1);

        /* por si acaso
        public async void LoadSceneAsync(int sceneIndex, TransitionMode tMode, UniTask waitTask)
        {
            bool noTransition = tMode == TransitionMode.None;
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, noTransition? LoadSceneMode.Single : LoadSceneMode.Additive);
            op.allowSceneActivation = false;
            await UniTask.WhenAll(waitTask, AsyncLoaded());
            op.allowSceneActivation = true;
            if (!noTransition) StartCoroutine(MakeTransition());

            async UniTask AsyncLoaded()
            {
                while (op.progress < 0.9f)
                    await Task.Yield();
            }

            IEnumerator MakeTransition()
            {
                Scene oldScene = SceneManager.GetActiveScene(), newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
                while (!op.isDone)
                    yield return null;

                Time.timeScale = 0f;
                //newScene.GetRootGameObjects().
                switch (tMode)
                {
                    case TransitionMode.SlideRight:
                        yield return cameraController.Slide(Vector3.forward * -10, duration);
                        break;
                }
                SceneManager.SetActiveScene(newScene);
                Time.timeScale = 1f;
                SceneManager.UnloadSceneAsync(oldScene);
            }
        }*/
        public bool IsGameplay() => CurrentSceneIndex == (int)SceneTypes.PvPGameplay;

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL
        Application.Quit();
#else
        Debug.LogError("No compatible");
#endif
        }
    }

    public enum SceneTypes : int
    {
        Cinematic = 0,
        Menu = 1,
        Glosary = 2,
        GameMode = 3,
        OnlineLobby = 4,
        PvPGameplay = 5,
        GameResults = 6
    }
    public enum TransitionMode
    {
        None,
        SlideRight
    }
}