using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class  SavingWrapper : MonoBehaviour
    {
        private const string CurrentSaveName = "currentSaveName";
        [SerializeField] float fadeInTime = 0.2f;
        [SerializeField] private float fadeOutTime = 0.2f;
        [SerializeField] private int firstLevelBuildIndex = 1;
        [SerializeField] private int menuLevelBuildIndex = 0;

        public void ContinueGame()
        {
            if (!PlayerPrefs.HasKey(CurrentSaveName)) return;
            if (!GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave())) return;
            StartCoroutine(LoadLastScene());
        }

        public bool CanContinueGame()
        {
            return GetComponent<SavingSystem>().SaveFileExists(GetCurrentSave());
        }

        public void NewGame(string saveFile)
        {
            if (String.IsNullOrEmpty(saveFile)) return;
            SetCurrentSave(saveFile);
            StartCoroutine(LoadSceneByIndex(firstLevelBuildIndex));
        }

        public void LoadGame(string saveFile)
        {
            SetCurrentSave(saveFile);
            ContinueGame();
        }
        
        
        public void LoadMainMenu()
        {
            StartCoroutine(LoadSceneByIndex(menuLevelBuildIndex));
        }

        private void SetCurrentSave(string saveFile)
        {
            PlayerPrefs.SetString(CurrentSaveName, saveFile);
        }
        
        private string GetCurrentSave()
        {
            return PlayerPrefs.GetString(CurrentSaveName);
        }

        private IEnumerator LoadLastScene()
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return GetComponent<SavingSystem>().LoadLastScene(GetCurrentSave());
            yield return fader.FadeIn(fadeInTime);
        }

        private IEnumerator LoadSceneByIndex(int sceneBuildIndex)
        {
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            yield return SceneManager.LoadSceneAsync(sceneBuildIndex);
            yield return fader.FadeIn(fadeInTime);
        }
        
        private void Update()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0) return;
           
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(GetCurrentSave());
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(GetCurrentSave());
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(GetCurrentSave());
        }

        public IEnumerable<string> ListSaves()
        {
            return GetComponent<SavingSystem>().ListSaves();
        }

    }
}