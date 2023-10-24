using System.Collections;
using TMPro;
using UnityEngine;


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


    private IEnumerator UpdateText()
    {
        yield return new WaitForSeconds(0.5f);

        FrameCounterText.text = "FrameCount: " + FrameCounter.ToString();
        EnemyCounterText.text = "EnemyCount: " + EnemyCounter.ToString();
        EnemiesPerFrameText.text = "EnemiesPerFrame: " + EnemiesPerFrame.ToString();
    
        StartCoroutine(UpdateText());
    }

}
