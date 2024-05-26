using UnityEngine;

public class ScorePin : MonoBehaviour
{
    private bool isKnockedOver = false;
    public void ResetPinScore()
    {
        isKnockedOver = false;
        Debug.Log("pin reseted");
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!isKnockedOver && (collision.gameObject.tag == "BowlingBall" || collision.gameObject.tag == "Pin"))
        {
            isKnockedOver = true;
            FindObjectOfType<BowlingScoreManager>().RecordRoll(1);
        }
    }
}
