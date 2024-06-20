using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AuditGame : MonoBehaviour
{
    const int NUM_SEQUENCES = 5;

    [SerializeField] private PlayerData player;
    [SerializeField] private Image[] buttonFlashes;
    [SerializeField] private Button[] buttons;
    
    [Header("Aggression Values")]
    [SerializeField] private int aggressionWonNoWl = -10;
    [SerializeField] private int aggressionWonWl = -30;
    [SerializeField] private float aggressionMultiplierLost = 50;


    private Queue<int> sequence;

    private bool isLastSequence;
    private bool sequenceFinished = true;
    private int registeredButtonsThisSequence;
    private static readonly int FlashAnimation = Animator.StringToHash("Flash");

    private async void OnEnable()
    {
        sequence = new Queue<int>();
        sequenceFinished = true;
        for (int i = 0; i < NUM_SEQUENCES; i++)
        {
            while (!sequenceFinished) await Task.Yield();
            sequenceFinished = false;
            registeredButtonsThisSequence = 0;
            ResetFlashes();

            foreach (Button item in buttons) item.enabled = false;

            await Task.Delay(1000);

            sequence.Enqueue(Random.Range(1, 10));

            PlaySequence(sequence);
        }
    }

    private async void PlaySequence(Queue<int> seq)
    {
        for (int i = 0; i < seq.Count; i++)
        {
            int number = seq.ToArray()[i];
            buttonFlashes[number].GetComponent<Animator>().SetTrigger(FlashAnimation);
            if (i < seq.Count - 1) await Task.Delay(1500);
        }

        foreach (Button item in buttons) item.enabled = true;
    }
    public void RegisterButton(int number)
    {
        buttonFlashes[number].GetComponent<Animator>().SetTrigger(FlashAnimation);

        int correctNumber = sequence.ToArray()[registeredButtonsThisSequence];
        registeredButtonsThisSequence++;

        isLastSequence = sequence.Count == NUM_SEQUENCES;
        sequenceFinished = registeredButtonsThisSequence == sequence.Count;

        if (number == correctNumber && isLastSequence && sequenceFinished)
        {
            // Won
            player.MoveAggression(player.IsOnWatchlist() ? aggressionWonWl : aggressionWonNoWl);

            ResetFlashes();
            gameObject.SetActive(false);
        }

        if (number != correctNumber)
        {
            // Lost
            player.MoveAggression(GetAggressionOnLost());

            ResetFlashes();
            gameObject.SetActive(false);
        }
    }

    private int GetAggressionOnLost()
    {
        int progress = sequence.Count;
        return (int)(1f / progress * aggressionMultiplierLost);
    }

    private void ResetFlashes()
    {
        foreach (Image img in buttonFlashes) img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
    }
}