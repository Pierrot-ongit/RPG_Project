using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class TextSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject prefabText = null;

        public void Spawn(string text)
        {
            GameObject instance = Instantiate(prefabText, transform);
            instance.GetComponentInChildren<TMP_Text>().text = text;
            Destroy(instance, 1f);
        }
    }
}