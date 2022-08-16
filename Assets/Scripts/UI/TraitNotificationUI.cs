using System;
using GameDevTV.UI;
using RPG.Quests;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitNotificationUI : MonoBehaviour
    {
       [SerializeField] private ShowHideUI toggleContainerUI;
       private BaseStats playerStats;
       [SerializeField] private Image imageNotification;

       private void Start()
       {
           playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
           if (playerStats == null) return;
           playerStats.onLevelUp += ActivateImage;
           toggleContainerUI.onToggleContainer += DesactivateImage;
       } 

       private void ActivateImage()
       {
           imageNotification.gameObject.SetActive(true);
       }
       
       private void DesactivateImage()
       {
           imageNotification.gameObject.SetActive(false);
       }
    }
}