using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewindLineController : MonoBehaviour
{
        [SerializeField] private GameObject _rewindLine;
    
        private void Awake()
        {
            //It is subscribing to the event
            GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        }
        private void OnDestroy()
        {
            //It is unsubscribing to the event
            GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
        }
        private void GameManagerOnGameStateChanged(GameState state)
        {
            
                if (state == GameState.Paradox)
                {
                    _rewindLine.SetActive(true);

                }
                else
                {
                    _rewindLine.SetActive(false);
                }
        }
        
}
