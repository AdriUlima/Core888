using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace Adri888
{
    using static Adri888Core;

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private PanelRenderer panelRenderer;
        [SerializeField] private VisualTreeAsset optionsUXML;
        private VisualElement panelOptions;

        void OnEnable() => panelRenderer.RegisterUIReloadCallback(OnUIReload);
        void OnDisable() => panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        private void OnUIReload(PanelRenderer renderer, VisualElement root, int version)
        {
            //TemplateContainer container = optionsUXML.Instantiate();
            //panelOptions = container.Q<VisualElement>("canvas");
            //root.Add(panelOptions);
            //SetPanelOptions();
            //HideOptions();

            Button btnPlay = root.Q<Button>("play");
            Button btnOptions = root.Q<Button>("options");
            Button btnQuit = root.Q<Button>("quit");

            btnPlay.clicked += OnPlay;
            btnOptions.clicked += ShowOptions;
            btnQuit.clicked += OnQuit;

            void SetPanelOptions()
            {
                Button _btnCloseOptions = root.Q<Button>("btn_close");

                DropdownField _dropdownLanguage = root.Q<DropdownField>("drop_languages");
                _dropdownLanguage.choices = core.LocaleNames.ToList();
                _dropdownLanguage.value = core.CurrentLocaleName;
                _dropdownLanguage.RegisterValueChangedCallback(evt => core.ChangeLocale(evt.newValue));

                // Sliders de volumen
                SliderInt _sliderMaster = root.Q<SliderInt>("vol_master");
                SliderInt _sliderMusic = root.Q<SliderInt>("vol_bgm");
                SliderInt _sliderSfx = root.Q<SliderInt>("vol_sfx");

                // Eventos de cambio de volumen
                _sliderMaster.RegisterValueChangedCallback(evt => audioManager.SetMasterVolume(evt.newValue));
                _sliderMusic.RegisterValueChangedCallback(evt => audioManager.SetMusicVolume(evt.newValue));
                _sliderSfx.RegisterValueChangedCallback(evt => audioManager.SetSfxVolume(evt.newValue));

                _btnCloseOptions.clicked += HideOptions;

                // Inicializar sliders con los valores actuales del AudioManager
                _sliderMaster.value = audioManager.MasterVolume;
                _sliderMusic.value = audioManager.BgmVolume;
                _sliderSfx.value = audioManager.SfxVolume;
            }
        }

        private void OnPlay() => core.NextScene();
        private void OnQuit() => core.QuitGame();
        private void ShowOptions() => panelOptions.style.display = DisplayStyle.Flex;
        private void HideOptions() => panelOptions.style.display = DisplayStyle.None;
    }
}