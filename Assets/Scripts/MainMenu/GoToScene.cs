using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToScene : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private Button button;

    // Update is called once per frame
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadSceneAsync(sceneName);
        });
    }
}
