using UnityEngine;
using UnityEngine.UI;

public class SimpleFPS : MonoBehaviour
{
    public Text display_Text;

    int avgFrameRate, currentFrameRate;

    public void Update()
    {
        avgFrameRate = (int)(Time.frameCount / Time.time);
        currentFrameRate = (int)(1f / Time.unscaledDeltaTime);

        display_Text.text = "Average: " + avgFrameRate.ToString() + " FPS  " + "Curent: " + currentFrameRate.ToString() + " FPS";
    }
}
