using System;
using System.Collections;
using GameDevTV.UI;
using RPG.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class QuestNotification : MonoBehaviour
    {
       [SerializeField] private GameObject rootNotifications;
       [SerializeField] private float popupTime = 3f;
       [SerializeField] private ShowHideUI toggleQuestsUI;
       [SerializeField] private Image buttonToggleNotification;
       [SerializeField] private Image imageNotificationType;
       [SerializeField] private TMP_Text textNotificationType;
       [SerializeField] private TMP_Text textNotificationMessage;
       [SerializeField] private Sprite spriteNewQuest;
       [SerializeField] private Sprite spriteObjectiveComplete;
       [SerializeField] private Sprite spriteQuestComplete;
       
       

       private QuestList questList;
       private const string QuestReceveidText = "New Quest";
       private const string QuestCompletedText = "Quest Completed";
       private const string ObjectiveCompletedText = "Objective Completed";

       private void Start()
       {
           questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
           questList.OnQuestReceived += QuestReceveid;
           questList.OnQuestCompleted += QuestCompleted;
           questList.OnObjectiveCompleted += ObjectiveCompleted;
           toggleQuestsUI.onToggleContainer += DesactivateToggleImage;
           rootNotifications.SetActive(false);
       }
       
       private void QuestReceveid(Quest quest)
       {
           ActivateToggleImage();
           imageNotificationType.sprite = spriteNewQuest;
           textNotificationType.text = QuestReceveidText;
           textNotificationMessage.text = quest.GetTitle();
           StartCoroutine(PopupNotification());
       }
       
       private void QuestCompleted(Quest quest)
       {
           ActivateToggleImage();
           imageNotificationType.sprite = spriteQuestComplete;
           textNotificationType.text = QuestCompletedText;
           textNotificationMessage.text = quest.GetTitle();
           StartCoroutine(PopupNotification());
       }
       
       private void ObjectiveCompleted(string objective)
       {
           ActivateToggleImage();
           imageNotificationType.sprite = spriteObjectiveComplete;
           textNotificationType.text = ObjectiveCompletedText;
           textNotificationMessage.text = objective;
           StartCoroutine(PopupNotification());
       }

       private void ActivateToggleImage()
       {
           if (buttonToggleNotification != null)
           {
               buttonToggleNotification.gameObject.SetActive(true);
           }
       }
       
       private void DesactivateToggleImage()
       {
           if (buttonToggleNotification != null)
           {
               buttonToggleNotification.gameObject.SetActive(false);
           }
       }

       private IEnumerator PopupNotification()
       {
           rootNotifications.SetActive(true);
           yield return new WaitForSeconds(popupTime);
           rootNotifications.SetActive(false);
       }
    }
}