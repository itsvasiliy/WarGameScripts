using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private Text FPS_CountText;

    private void Update()
    {
        int FPS_count = (int)(1f / Time.unscaledDeltaTime);
        FPS_CountText.text = FPS_count.ToString();
    }
}
