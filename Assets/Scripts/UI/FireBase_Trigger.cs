using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FireBaseEndTrigger : MonoBehaviour
{
    public string nextScene = "Level2";
    public GameObject loadingScreen;

    bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;
        if (!other.CompareTag("Player")) return;

        used = true;
        StartCoroutine(Transition(other.gameObject));
    }

    IEnumerator Transition(GameObject player)
    {
      
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

       
        player.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (loadingScreen)
            loadingScreen.SetActive(true);

        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(nextScene);
    }
}
