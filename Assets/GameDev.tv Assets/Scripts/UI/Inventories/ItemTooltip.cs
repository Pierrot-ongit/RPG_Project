using UnityEngine;
using TMPro;
using GameDevTV.Inventories;
using RPG.Abilities;
using RPG.Combat;
using RPG.Inventories;
using UnityEngine.UI;

namespace GameDevTV.UI.Inventories
{
    /// <summary>
    /// Root of the tooltip prefab to expose properties to other classes.
    /// </summary>
    public class ItemTooltip : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;
        [SerializeField] Image iconImage = null;
        [SerializeField] TextMeshProUGUI conditionsText = null;
        [SerializeField] TextMeshProUGUI weaponAttributsText = null;
        [SerializeField] TextMeshProUGUI modifiersText = null;
        [SerializeField] TextMeshProUGUI abilityAttributsText = null;

        // PUBLIC

        public void Setup(InventoryItem item)
        {
            titleText.text = item.GetDisplayName();
            bodyText.text = item.GetDescription();
            iconImage.sprite = item.GetIcon();

            if (item is EquipableItem)
            {
                string conditions = (item as EquipableItem).GetConditionText();
                if (conditions != "")
                {
                    conditionsText.gameObject.SetActive(true);
                    conditionsText.text = conditions;
                }
            }

            if (item is WeaponConfig)
            {
                weaponAttributsText.gameObject.SetActive(true);
                string text = "Weapon damage : " + (item as WeaponConfig).GetWeaponDamage().ToString() + " \n";
                text += "Weapon range : " + (item as WeaponConfig).GetWeaponRange().ToString();
                weaponAttributsText.text = text;
            }
            
            if (item is StatsEquipableItem)
            {
                string stats = (item as StatsEquipableItem).GetStatsModifiersText();
                if (stats != "")
                {
                    modifiersText.gameObject.SetActive(true);
                    modifiersText.text = stats;
                }
            }
            
            if (item is Ability)
            {
                abilityAttributsText.gameObject.SetActive(true);
                string text = "Cooldown : " + (item as Ability).GetCooldown().ToString() + " seconds \n";
                text += "Mana cost : " + (item as Ability).GetManaToUse().ToString();
                abilityAttributsText.text = "";
            }
        }
    }
}
