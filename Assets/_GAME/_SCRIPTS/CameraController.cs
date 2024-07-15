using UnityEngine;

namespace TuringSimulator
{
    ///<summary>
    /// control camera movement for 2D game mode
    ///</summary>
    [AddComponentMenu("UPDB/Data/NativeTools/SimpleGridLevel/CameraController")]
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Tooltip("speed of zoom in and out")]
        private float _scrollSpeed = 0.5f;

        [SerializeField, Tooltip("speed of mouse drag")]
        private float _dragSpeed = 1;


        private Camera _camera;

        private Vector2 _camLastPos = Vector2.zero;
        private Vector2 _mouseLastPos = Vector2.zero;

        private void Awake()
        {
            _camera = gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - _scrollSpeed, 0.1f, 1000);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + _scrollSpeed, 0.1f, 1000);

            if (Input.GetKey(KeyCode.Mouse2))
            {
                transform.position = new Vector3(_camLastPos.x + (((_mouseLastPos.x - Input.mousePosition.x) * _camera.orthographicSize) * _dragSpeed), _camLastPos.y + (((_mouseLastPos.y - Input.mousePosition.y) * _camera.orthographicSize) * _dragSpeed), -100);
            }
            else
            {
                _camLastPos = transform.position;
                _mouseLastPos = Input.mousePosition;
            }
        }
    } 
}
