using UnityEngine;

public class RailLogic : MonoBehaviour
{
    private GameObject player;
    private float yOffset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        yOffset = transform.position.y - player.transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector2 pos = transform.position;
        pos.y = player.transform.position.y + yOffset;
        transform.position = pos;
    }
}
