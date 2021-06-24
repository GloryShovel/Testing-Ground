using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text display_Text;

    int avgFrameRate;

    public void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.time;
        avgFrameRate = (int)(1f / Time.unscaledDeltaTime);
        display_Text.text = avgFrameRate.ToString() + " FPS";
    }
}
