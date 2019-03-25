using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smooth = 0.5f;
    public float zoomSpeed = 0.1f;

    public Transform target;
    [HideInInspector]
    public float viewSize = 5;

    private Vector3 offset;

    private void Awake()
    {
        offset = new Vector3(0, 0, -10);
    }

    private void LateUpdate()
    {
        Vector3 newPos = Vector3.Lerp(transform.position, target.position + offset, smooth);

        transform.position = newPos;

        ZoomToTarget();
    }

    private void ZoomToTarget()
    {
        float newZoom = Mathf.Lerp(Camera.main.orthographicSize, viewSize, zoomSpeed);

        Camera.main.orthographicSize = newZoom;
    }
}
