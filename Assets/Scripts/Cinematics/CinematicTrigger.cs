using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GameDevTV.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        public bool wasPlayed = false;
        
        private void OnTriggerEnter(Collider other)
        {
           if (other.gameObject.tag != "Player") return;
            PlayCinematic();
        }
        
        
        public void PlayCinematic()
        {
            if (wasPlayed) return;
            GetComponent<PlayableDirector>().Play();
            wasPlayed = true;
        }
        
        public bool WasPlayed()
        {
            return wasPlayed;
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Cinematic"] = wasPlayed;
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            wasPlayed = (bool)data["Cinematic"];
        }


    }
}

