using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AuditGame : MonoBehaviour
{
    const int NUM_SEQUENCES = 5;

    [SerializeField] private PlayerData player;
    [SerializeField] private PlayerStatsObject playerStats;
    [SerializeField] private Image[] buttonFlashes;
    [SerializeField] private Button[] buttons;
    
    [Header("Aggression Values")]
    [SerializeField] private int aggressionWonNoWl = -10;
    [SerializeField] private int aggressionWonWl = -30;
    [SerializeField] private float aggressionMultiplierLost = 10;


    private Queue<int> sequence;

    private int numSequences;
    private bool isLastSequence;
    private bool sequenceFinished = true;
    private int registeredButtonsThisSequence;
    private static readonly int FlashAnimation = Animator.StringToHash("Flash");

    private async void OnEnable() {
        numSequences = NUM_SEQUENCES - playerStats.auditDifficultyDecrease;
        sequence = new Queue<int>();
        sequenceFinished = true;
        for (int i = 0; i < numSequences; i++)
        {
            while (!sequenceFinished) await Task.Yield();
            sequenceFinished = false;
            registeredButtonsThisSequence = 0;
            ResetFlashes();

            foreach (Button item in buttons) item.enabled = false;

            await Task.Delay(750);
            
            int nextValue;
            do {
                nextValue = Random.Range(1, 10);
            } while(sequence.Count > 0 && sequence.Contains(nextValue));
            
            sequence.Enqueue(nextValue);

            PlaySequence(sequence);
        }
    }

    private async void PlaySequence(Queue<int> seq)
    {
        for (int i = 0; i < seq.Count; i++)
        {
            int number = seq.ToArray()[i];
            buttonFlashes[number].GetComponent<Animator>().SetTrigger(FlashAnimation);
            if (i < seq.Count - 1) await Task.Delay(250);
        }

        foreach (Button item in buttons) item.enabled = true;
    }
    public void RegisterButton(int number)
    {
        buttonFlashes[number].GetComponent<Animator>().SetTrigger(FlashAnimation);

        int correctNumber = sequence.ToArray()[registeredButtonsThisSequence];
        registeredButtonsThisSequence++;

        isLastSequence = sequence.Count == numSequences;
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

    private int GetAggressionOnLost() {
        return (int)(aggressionMultiplierLost * sequence.Count);
    }

    private void ResetFlashes()
    {
        foreach (Image img in buttonFlashes) img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
    }
}