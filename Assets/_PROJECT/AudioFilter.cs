using UnityEngine;

public class AudioFilter : MonoBehaviour
{
    void OnAudioFilterRead(float[] data, int channels)
    {
        float max = 0f;

        for (int i = 0; i < data.Length; i++)
        {
            max = Mathf.Max(max, Mathf.Abs(data[i]));
        }

        if (max > 0.01f)
        {
            //Debug.Log($"Active sound detected. Max amplitude: {max}");
        }
    }
}
