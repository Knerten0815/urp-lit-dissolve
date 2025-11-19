using UnityEngine;

public class Movement : MonoBehaviour
{
    public float _lookSpeed = 3;
    public float _moveSpeed = 0.1f;
    public float _dashSpeedMultiplier = 1;
    private Vector2 _rotation = Vector2.zero;

    private void Update()
    {
        if (Input.GetMouseButton(1))
            Look();
        else
            Cursor.lockState = CursorLockMode.None;

        Move();
    }
    
    private void Move()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            dir += transform.TransformDirection(Vector3.forward);

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
            dir += transform.TransformDirection(Vector3.left);

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            dir += transform.TransformDirection(Vector3.back);

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
            dir += transform.TransformDirection(Vector3.right);

        if (Input.GetKey(KeyCode.E))
            dir += transform.TransformDirection(Vector3.up);

        if (Input.GetKey(KeyCode.Q))
            dir += transform.TransformDirection(Vector3.down);

        float dash;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            dash = _dashSpeedMultiplier;
        else
            dash = 1;

            transform.position += dir * _moveSpeed * dash;
    }

    private void Look()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _rotation.y += Input.GetAxis("Mouse X");
        _rotation.x += -Input.GetAxis("Mouse Y");
        _rotation.x = Mathf.Clamp(_rotation.x, -15f, 15f);
        transform.eulerAngles = new Vector2(0,_rotation.y) * _lookSpeed;
        Camera.main.transform.localRotation = Quaternion.Euler(_rotation.x * _lookSpeed, 0, 0);
    }
}
