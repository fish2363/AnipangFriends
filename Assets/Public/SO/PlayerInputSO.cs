using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/PlayerInput", order = 0)]
public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
{
    [SerializeField] private LayerMask whatIsGround;

    public event Action OnAttackPressed;
    public event Action OnChangePressed;
    public event Action OnSkillPressed;

    public Vector2 MovementKey { get; private set; }
    private Controls _controls;

    private Vector3 _worldPosition; //이게 마우스의 월드 좌표
    private Vector2 _screenPosition; //이게 마우스가 위치한 화면좌표

    private void OnEnable()
    {
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementKey = context.ReadValue<Vector2>();
        MovementKey = movementKey;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnAttackPressed?.Invoke();
    }

    public Vector3 GetWorldPosition()
    {
        Camera mainCam = Camera.main; //Unity2022부터 내부 캐싱이 되서 그냥 써도 돼.
        Debug.Assert(mainCam != null, "No main camera in this scene");

        Ray cameraRay = mainCam.ScreenPointToRay(_screenPosition);
        if (Physics.Raycast(cameraRay, out RaycastHit hit, mainCam.farClipPlane, whatIsGround))
        {
            _worldPosition = hit.point;
        }

        return _worldPosition;
    }

    public void OnPointer(InputAction.CallbackContext context)
    {
        _screenPosition = context.ReadValue<Vector2>();
    }

    public void SetActive(bool isActive)
    {
        if (isActive)
            _controls.Player.Enable();
        else
            _controls.Player.Disable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
    }

    public void OnChange(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnChangePressed?.Invoke();
    }

    public void OnSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSkillPressed?.Invoke();
    }
}
