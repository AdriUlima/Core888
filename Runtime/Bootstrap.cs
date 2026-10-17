using UnityEngine;

namespace Adri888
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Core _core;
        [SerializeField] private GameManager _game;
        [SerializeField] private InputManager _input;
        [SerializeField] private AudioManager _audio;
        [SerializeField] private UIManager _ui;
        [SerializeField] private PrefsManager _prefs;
        [SerializeField] private GameObject eventSystem;

        void Awake()
        {
            if (!Adri888Core.Init(_core, _game, _input, _audio, _ui, _prefs))
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }
        void Start() => eventSystem.SetActive(true);
    }
}