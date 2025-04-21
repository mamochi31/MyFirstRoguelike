using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomNodeUI : MonoBehaviour
{
    private RoomNode linkedNode;

    public void Setup(RoomNode node)
    {
        linkedNode = node;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.MoveToRoom(linkedNode);
    }
}
