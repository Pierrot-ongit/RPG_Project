using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(fileName = "Spawn Target Prefab Effect", menuName = "RPG/Abilities/Effects/SpawnTargetPrefabEffect", order = 0)]
    public class SpawnTargetPrefabEffect : EffectStategy
    {
        [SerializeField]Transform prefabToSpawn = null;
        [SerializeField]float destroyDelay = -1;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(Effect(data, finished));
        }

        private IEnumerator Effect(AbilityData data, Action finished)
        {
            Transform prefabInstance = Instantiate(prefabToSpawn, data.GetTargetedPoint(), Quaternion.identity);
            if (destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(prefabInstance.gameObject);
            }
            finished();
        }
    }
}