using System;
using GameDevTV.Utils;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        private LazyValue<SavingWrapper> savingWrapper;
        [SerializeField] private Button continueButton;
        [SerializeField] private TMP_InputField newGameName;

        private void Awake()
        {
            savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
            continueButton.onClick.AddListener(ContinueGame);
        }

        private void Start()
        {
            continueButton.gameObject.SetActive(savingWrapper.value.CanContinueGame());
        }

        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }

        public void ContinueGame()
        {
            savingWrapper.value.ContinueGame();
        }
        public void NewGame()
        {
            savingWrapper.value.NewGame(newGameName.text);
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            # else
            Application.Quit();
            #endif
        }
        
    }
}