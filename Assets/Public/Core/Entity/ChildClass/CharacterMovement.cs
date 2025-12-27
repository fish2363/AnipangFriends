using UnityEngine;

public class CharacterMovement : MonoBehaviour, IEntityComponent, IAfterInitialize, IKnockBackable
{
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private CharacterController characterController;

    public bool CanManualMovement { get; set; } = true;

    public float MoveSpeed { get; set; } = 8f;

    private Vector3 _autoMovement;
    private float _autoMoveStartTime;
    private MovementDataSO _movementData;

    public bool IsGround => characterController.isGrounded;

    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;

    private float _verticalVelocity;
    private Vector3 _movementDirection;

    private Entity _entity;
    private EntityStatCompo _statCompo;
    public void Initialize(Entity entity)
    {
        _entity = entity;
        _statCompo = entity.GetCompo<EntityStatCompo>();
    }

    public void AfterInitialize()
    {
        MoveSpeed = 10f;
        //_moveSpeed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 4f);
    }

    private void OnDestroy()
    {
        //_statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
    }

    public void MoveSpeedChangeHandle(float value)
    {
        MoveSpeed = value;
    }

    public void SetMovementDirection(Vector2 movementInput)
    {
        _movementDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
    }

    private void FixedUpdate()
    {
        CalculateMovement();
        ApplyGravity();
        Move();
    }

    private void CalculateMovement()
    {
        if (CanManualMovement)
        {
            _velocity = Quaternion.Euler(0, -45f, 0) * _movementDirection;
            _velocity *= MoveSpeed * Time.fixedDeltaTime;
        }
        else
        {
            //0~1사이의 시간으로 정규화한다.
            float normalizeTime = (Time.time - _autoMoveStartTime) / _movementData.duration;
            float currentSpeed = _movementData.maxSpeed
                                 * _movementData.moveCurve.Evaluate(normalizeTime);
            Vector3 currentMovement = _autoMovement * currentSpeed;
            _velocity = currentMovement * Time.fixedDeltaTime;
        }

        if (_velocity.magnitude > 0 && CanManualMovement)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_velocity);
            Transform parent = _entity.transform;
            parent.rotation = Quaternion.Lerp(parent.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    private void ApplyGravity()
    {
        if (IsGround && _verticalVelocity < 0)
            _verticalVelocity = -0.03f; //살짝 아래로 당겨주는 힘
        else
            _verticalVelocity += gravity * Time.fixedDeltaTime;

        _velocity.y = _verticalVelocity;
    }

    private void Move()
    {
        characterController.Move(_velocity);
    }

    public void StopImmediately()
    {
        _movementDirection = Vector3.zero;
    }

    //public void SetAutoMovement(Vector3 autoMovement) => _autoMovement = autoMovement;

    public void KnockBack(Vector3 direction, MovementDataSO knockbackMovement)
    {
        _autoMoveStartTime = Time.time;
        _movementData = knockbackMovement;
        _autoMovement = direction;
    }

    public void ApplyMovementData(Vector3 playerDirection, MovementDataSO movementData)
    {
        _autoMovement = playerDirection;
        _autoMoveStartTime = Time.time;
        _movementData = movementData;
    }
}
