using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Mathematics;


public class Profiling : MonoBehaviour
{
    [SerializeField] private TMP_Text SleepOrPIText;
    [SerializeField] private TMP_Text JobsOnText;
    [SerializeField] private TMP_Text FrameCounterText;
    [SerializeField] private TMP_Text EnemyCounterText;
    [SerializeField] private TMP_Text AddingPIText;

    public static bool JobsON;
    public static int FrameCounter;
    public static int EnemyCounter;

    public static bool DoingPI;
    public static float AddingPI;

    private void Start()
    {
        StartCoroutine(UpdateText());
    }

    private void Update()
    {
        FrameCounter++;
    }

    public static float CalculatePi(int instances)
    {
        float pi = 0.0f;
        for (int i = 0; i < instances; i++)
        {
            pi += 4.0f * math.pow(-1, i) / (2 * i + 1);
        }
        return pi;
    }

    private IEnumerator UpdateText()
    {
        yield return new WaitForSeconds(0.5f);

        int fps = (int)(1f / Time.deltaTime);

        string usedFunc = DoingPI ? "PI" : "Sleep";
        SleepOrPIText.text = "Heavy Function: " + usedFunc;

        FrameCounterText.text = "FPS: " + fps.ToString();
        EnemyCounterText.text = "Active Enemies: " + EnemyCounter.ToString();
        JobsOnText.text = "Using Jobs: " + JobsON.ToString();

        if (DoingPI) {
            AddingPIText.text = "Adding Pi Result: " + AddingPI.ToString();
        }
        else AddingPIText.text = "Not currently calculating PI";

    
        StartCoroutine(UpdateText());
    }

}
