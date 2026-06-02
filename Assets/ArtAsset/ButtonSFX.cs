using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public void PlayButtonPress()
    {
        if (GameSFXManager.Instance != null)
        {
            GameSFXManager.Instance.PlayButtonPress();
        }
    }
}