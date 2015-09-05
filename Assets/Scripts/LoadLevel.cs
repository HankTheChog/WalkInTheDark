using UnityEngine;

namespace Assets.Scripts
{
    public class LoadLevel : MonoBehaviour
    {
        public void LoadGame()
        {
            Application.LoadLevel("Game");
        }
    }
}