using System.Collections;
using UnityEngine;

namespace GoGame
{
    // from pressing find match button -> go game end
    public class GoGameHandler : MonoBehaviour
    {

        private HandlePreGoGame _handlePreGoGame;
        private HandleInGoGame _handleInGoGame;

        private void Awake()
        {
            _handlePreGoGame = GetComponent<HandlePreGoGame>();
            _handleInGoGame = GetComponent<HandleInGoGame>();
        }

        public IEnumerator PreGoGameRoutine()
        {
            // handle pre go game
            yield return _handlePreGoGame.DoCoroutine();
            
        }

        public void StartGoGame()
        {
            _handleInGoGame.StartGoGame(_handlePreGoGame.LocalStoneType);
        }
        
    }

}