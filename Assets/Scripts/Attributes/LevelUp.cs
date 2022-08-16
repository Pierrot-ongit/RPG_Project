using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

namespace RPG.Attributes
{
    public class LevelUp : MonoBehaviour
    {
        [SerializeField] ParticleSystem _particleSystem;
        // Start is called before the first frame update
        void Start()
        {
            BaseStats stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            if (stats != null)
            {
                stats.onLevelUp += LevelUpEffect;
            }
        }

        void LevelUpEffect()
        {
            _particleSystem.Play();
        }
    }
}

