using UnityEngine;
using UnityEngine.SceneManagement;

public class LostScreen : MonoBehaviour
{
    public void LoadMenu() => SceneManager.LoadScene(0);
    public void Retry() => SceneManager.LoadScene(1);
}