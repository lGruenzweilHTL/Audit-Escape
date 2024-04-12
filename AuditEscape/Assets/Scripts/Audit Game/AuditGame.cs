using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class AuditGame : MonoBehaviour
{
    const int NUM_SEQUENCES = 5;

    [SerializeField] private PlayerData player;
    [SerializeField] private Image[] buttonFlashes;
    [SerializeField] private Button[] buttons;
    private Queue<int> sequence;

    bool onLastSequence = false;
    bool sequenceFinished = true;
    int registeredButtonsThisSeqence = 0;

    private async void OnEnable()
    {
        sequence = new();
        sequenceFinished = true;
        for (int i = 0; i < NUM_SEQUENCES; i++)
        {
            while (!sequenceFinished) await Task.Yield();
            sequenceFinished = false;
            registeredButtonsThisSeqence = 0;
            ResetFlashes();

            foreach (var item in buttons) item.enabled = false;

            await Task.Delay(1000);

            sequence.Enqueue(Random.Range(1, 10));

            PlaySequence(sequence);
        }
    }

    private async void PlaySequence(Queue<int> sequence)
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            int number = sequence.ToArray()[i];
            buttonFlashes[number].GetComponent<Animator>().SetTrigger("Flash");
            if (i < sequence.Count - 1) await Task.Delay(1500);
        }

        foreach (var item in buttons) item.enabled = true;
    }
    public void RegisterButton(int number)
    {
        buttonFlashes[number].GetComponent<Animator>().SetTrigger("Flash");

        int correctNumber = sequence.ToArray()[registeredButtonsThisSeqence];
        registeredButtonsThisSeqence++;

        onLastSequence = sequence.Count == NUM_SEQUENCES;
        sequenceFinished = registeredButtonsThisSeqence == sequence.Count;

        if (number == correctNumber && onLastSequence && sequenceFinished)
        {
            // Won
            if (player.IsOnWatchlist()) player.MoveAggression(-30);
            else player.MoveAggression(-10);

            ResetFlashes();
            gameObject.SetActive(false);
        }

        if (number != correctNumber)
        {
            // Lost
            player.MoveAggression(20);

            ResetFlashes();
            gameObject.SetActive(false);
        }
    }

    public void ResetFlashes()
    {
        foreach (var img in buttonFlashes) img.color = new(img.color.r, img.color.g, img.color.b, 0f);
    }
}