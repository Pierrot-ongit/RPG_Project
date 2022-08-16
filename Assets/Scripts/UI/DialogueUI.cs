using System.Collections;
using System.Collections.Generic;
using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant playerConversant;
        [SerializeField] private TextMeshProUGUI AIText;
        [SerializeField] private TextMeshProUGUI currentSpeaker;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private GameObject AIResponse;
        
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
           // nextButton.onClick.AddListener(() => playerConversant.Next()); //Exemple lambda function
            nextButton.onClick.AddListener(playerConversant.Next); // Same as Lambda function above
            quitButton.onClick.AddListener(playerConversant.Quit);
            UpdateUI();
        }

        void Quit()
        {
            playerConversant.Quit();
        }


        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            
            if (!playerConversant.IsActive()) return;

            currentSpeaker.text = playerConversant.GetCurrentSpeaker();
            AIText.text = playerConversant.GetText();
            bool choicesExists = BuildChoicesList();
            if (choicesExists)
            {
                nextButton.gameObject.SetActive(false);
            }
            else {
                nextButton.gameObject.SetActive(playerConversant.HasNext());

            }

            //AIResponse.SetActive(!playerConversant.IsChoosing());
            //choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());
        
        }

        private bool BuildChoicesList()
        {
            bool choicesExists = false;
            // We clear the previous choices to make new ones.
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.GetText();
                // Lambda function
                choiceInstance.GetComponentInChildren<Button>().onClick.AddListener(
                    () =>
                    {
                        playerConversant.SelectChoice(choice);
                    });
                    choicesExists = true; // We have at least one choice.
            }

            return choicesExists;
        }
    }
}

