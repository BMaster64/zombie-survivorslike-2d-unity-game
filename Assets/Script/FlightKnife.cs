using UnityEngine;

public class FlightKnife : MonoBehaviour
{
    public float rotateSpeed = 100f;  // Tốc độ quay của dao bay
    public Transform player;  // Tham chiếu đến player
    private Vector3 offset;  // Khoảng cách ban đầu giữa dao bay và player

    void Start()
    {
        // Lưu vị trí ban đầu của dao bay so với player
        offset = transform.position - player.position;
    }

    void Update()
    {
        // Quay dao bay xung quanh player, không bị ảnh hưởng bởi góc quay của player
        transform.position = player.position + offset;  // Đảm bảo dao bay giữ khoảng cách với player
        transform.RotateAround(player.position, Vector3.forward, rotateSpeed * Time.deltaTime);  // Quay dao xung quanh player
    }
}
