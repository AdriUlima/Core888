using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Adri888
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton
        public static AudioManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                BuildLookup();
            }
            else Destroy(gameObject);
        }
        #endregion

        [HideInInspector] public int MasterVolume = 100, BgmVolume = 100, SfxVolume = 100;
        private Bus masterBus, bgmBus, sfxBus;
        private EventInstance bgmInstance;
        [SerializeField] private Dictionary<ParamEnum, string> paramDict;
        private Dictionary<ParamEnum,PARAMETER_ID> pidDict;
        [SerializeField] private Dictionary<BGMEnum, EventReference> bgmDict;
        [SerializeField] private Dictionary<SFXEnum, EventReference> sfxDict;
        [SerializeField] private Dictionary<int, EventReference> sceneDict;

        private void BuildLookup()
        {
            BuildLookup(paramDict, pidDict, p =>
            {
                RuntimeManager.StudioSystem.getParameterDescriptionByName(p.Value, out var desc);
                return desc.id;
            });
        }
        private void BuildLookup<K, Vi, Vf>(Dictionary<K, Vi> dictI, Dictionary<K, Vf> dictF, Func<KeyValuePair<K, Vi>, Vf> lookupAction)
        {
            dictF = new();
            if (dictI == null || dictI.Count == 0) return;
            foreach (KeyValuePair<K, Vi> p in dictI)
            {
                Vf v = lookupAction(p);
                dictF[p.Key] = v;
            }
            foreach (KeyValuePair<K, Vf> aux in dictF)
                Debug.Log(aux.Key + " " + aux.Value);
        }

        void Start()
        {
            masterBus = RuntimeManager.GetBus("bus:/");
            bgmBus = RuntimeManager.GetBus("bus:/BGM");
            sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }

        void OnEnable() => SceneManager.sceneLoaded += SceneLoaded;
        void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            ReleaseBGMInstance();
        }
        private void SceneLoaded(Scene scene, LoadSceneMode loadMode) => PlayBGM(sceneDict[scene.buildIndex]);

        public EventInstance CreateEventInstance(EventReference soundEvent) => RuntimeManager.CreateInstance(soundEvent);
        private void ReleaseEventInstance(EventInstance eventInstance, bool allowFadeOut = false)
        {
            eventInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        public void SetMasterVolume(int value) => SetVolume(ref MasterVolume, masterBus, "vol_master", value);
        public void SetMusicVolume(int value) => SetVolume(ref BgmVolume, bgmBus, "vol_music", value);
        public void SetSfxVolume(int value) => SetVolume(ref SfxVolume, sfxBus, "vol_sfx", value);
        private void SetVolume(ref int volume, Bus bus, string key, int value)
        {
            volume = value;
            bus.setMute(volume == 0);
            bus.setVolume(value / 100f);
            PlayerPrefs.SetInt(key, value);
        }

        public EventReference GetBgmFromEnum(BGMEnum bgmEnum) => bgmDict[bgmEnum];
        public void PlayBGM(BGMEnum bgmEnum) => PlayBGM(GetBgmFromEnum(bgmEnum));
        public void PlayBGM(EventReference bgmEvent)
        {
            if (bgmInstance.isValid()) ReleaseBGMInstance(true);
            bgmInstance = CreateEventInstance(bgmEvent);
            bgmInstance.start();
        }
        private void ReleaseBGMInstance(bool allowFadeOut = false) => ReleaseEventInstance(bgmInstance, allowFadeOut);

        public EventReference GetSfxFromEnum(SFXEnum sfxEnum) => sfxDict[sfxEnum];
        public void PlaySfx(SFXEnum sfxEnum, Vector3 position) => PlaySfx(GetSfxFromEnum(sfxEnum), position);
        public void PlaySfx(EventReference sfxEvent, Vector3 position) => RuntimeManager.PlayOneShot(sfxEvent, position);

        public PARAMETER_ID GetParameterFromEnum(ParamEnum paramEnum) => pidDict[paramEnum];
        public void SetParameter(ParamEnum paramEnum, float value) => SetParameter(GetParameterFromEnum(paramEnum), value);
        public void SetParameter(PARAMETER_ID pId, float value) => RuntimeManager.StudioSystem.setParameterByID(pId, value);
    }

    public enum BGMEnum
    {
        Menu
    }

    public enum SFXEnum
    {
        Click,
        Baseball,
        Bottle,
        Chapita,
        Bravo,
        Boo,
        Kids
    }

    public enum ParamEnum
    {
        PlayState,
        SportState
    }
}