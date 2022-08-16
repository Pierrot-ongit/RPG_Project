using GameDevTV.Utils;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
       // private LazyValue<SavingWrapper> savingWrapper;
        [SerializeField] GameObject buttonLoadPrefab;
        [SerializeField] Transform contentRoot;
        
        private void Awake()
        {
   //         savingWrapper = new LazyValue<SavingWrapper>(GetSavingWrapper);
        }
        
        private SavingWrapper GetSavingWrapper()
        {
            return FindObjectOfType<SavingWrapper>();
        }
         
        void OnEnable()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper == null) return;
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject); 
            }

            foreach (string save in savingWrapper.ListSaves())
            {
                GameObject buttonInstance = Instantiate<GameObject>(buttonLoadPrefab, contentRoot);
                buttonInstance.GetComponentInChildren<TMP_Text>().text = save;
                Button button = buttonInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    savingWrapper.LoadGame(save);
                });
            }
        }
    }
}