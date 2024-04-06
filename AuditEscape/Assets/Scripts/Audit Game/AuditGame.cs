using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuditGame : MonoBehaviour
{
    [SerializeField] private PlayerData player;
    [SerializeField] private Image[] buttonFlashes;
    [SerializeField] private Button[] buttons;
    private Queue<int> sequence;

    private void OnEnable()
    {
        foreach (var item in buttons) item.enabled = false;

        sequence = GetSequence(5);
        PlaySequence(sequence);
    }

    private Queue<int> GetSequence(int numButtons)
    {
        Queue<int> sequence = new();

        string sq = Random.Range(0, (int)Mathf.Pow(10, numButtons)).ToString();
        foreach (char number in sq) sequence.Enqueue(number - '0');

        return sequence;
    }
    private async void PlaySequence(Queue<int> sequence)
    {
        foreach (int number in sequence)
        {
            buttonFlashes[number].GetComponent<Animator>().SetTrigger("Flash");
            await System.Threading.Tasks.Task.Delay(1000);
        }

        foreach (var item in buttons) item.enabled = true;
    }
    public void RegisterButton(int number)
    {
        int correctNumber = sequence.Dequeue();

        if (sequence.Count == 0 && number == correctNumber)
        {
            // Won
            if (player.IsOnWatchlist()) player.MoveAggression(-30);
            else player.MoveAggression(-10);

            gameObject.SetActive(false);
            return;
        }

        if (number != correctNumber)
        {
            // Lost
            player.MoveAggression(15);
            gameObject.SetActive(false);
        }
    }
}