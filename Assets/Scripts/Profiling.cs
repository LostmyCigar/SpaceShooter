using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Mathematics;


public class Profiling : MonoBehaviour
{

    [SerializeField] private TMP_Text FrameCounterText;
    [SerializeField] private TMP_Text EnemyCounterText;
    [SerializeField] private TMP_Text EnemiesPerFrameText;

    public static int FrameCounter;
    public static int EnemyCounter;

    public static int EnemiesPerFrame
    {
        get { return EnemyCounter / FrameCounter; }
    }


    private void Start()
    {
        StartCoroutine(UpdateText());
    }

    private void Update()
    {
        FrameCounter++;
    }

    public static float CalculatePi(int iterations)
    {
        float pi = 0.0f;
        for (int i = 0; i < iterations; i++)
        {
            pi += 4.0f * math.pow(-1, i) / (2 * i + 1);
        }
        return pi;
    }

    private IEnumerator UpdateText()
    {
        yield return new WaitForSeconds(0.5f);

        int fps = (int)(1f / Time.deltaTime);

        FrameCounterText.text = "FPS: " + fps.ToString();
        EnemyCounterText.text = "Active Enemies: " + EnemyCounter.ToString();
        //EnemiesPerFrameText.text = "Spawn Per Interval: " + EnemiesPerFrame.ToString();
    
        StartCoroutine(UpdateText());
    }

}
