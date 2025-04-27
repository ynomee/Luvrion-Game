using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    //Scriptable object which holds all the player's movement parameters. If you don't want to use it
    //just paste in all the parameters, though you will need to manuly change all references in this script
    public PlayerData Data;

    //Temporary test player state list
    public PlayerStateList pState;
    public Recoil recoil;

    #region COMPONENTS
    [SerializeField] private PlayerInput _playerInput;
    //[SerializeField] private Attack _attack;
    private PlayerModel _playerModel;
    private IAttack _attack;

    public Rigidbody2D RB { get; private set; }
    //Script to handle all player animations, all references can be safely removed if you're importing into your own project.
    #endregion

    #region STATE PARAMETERS
    //Variables control the various actions the player can perform at any time.
    //These are fields which can are public allowing for other sctipts to read them
    //but can only be privately written to.
    public bool IsFacingRight { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsWallJumping { get; private set; }
    public bool IsDashing { get; set; }
    public bool IsSliding { get; private set; }

    //Timers (also all fields, could be private and a method returning a bool could be used)
    public float LastOnGroundTime { get; private set; }
    public float LastOnWallTime { get; private set; }
    public float LastOnWallRightTime { get; private set; }
    public float LastOnWallLeftTime { get; private set; }

    //Jump
    private bool _isJumpCut;
    private bool _isJumpFalling;

    //Double Jump
    private float _bonusJumpsLeft;

    //Wall Jump
    private float _wallJumpStartTime;
    private int _lastWallJumpDir;

    //Dash
    private int _dashesLeft;
    private bool _dashRefilling;
    private Vector2 _lastDashDir;
    public bool _isDashAttacking;
    public Coroutine DashCoroutine;

    #endregion

    #region INPUT PARAMETERS
    private Vector2 _moveInput;

    public float LastPressedJumpTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    #endregion

    #region CHECK PARAMETERS
    //Set all of these up in the inspector
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    //Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
    [SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
    [Space(5)]
    [SerializeField] private Transform _frontWallCheckPoint;
    [SerializeField] private Transform _backWallCheckPoint;
    [SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
    [SerializeField] private LayerMask _groundLayer;
    #endregion

    #region CAMERA STUFF
    [Header("Camera Stuff")]
    [SerializeField] private GameObject _cameraFollow;

    private CameraFollowObject _cameraFollowObject;

    private float _fallSpeedYDampingChangeThreshold;
    #endregion
    
    #region STAMINA
    [SerializeField] private int _maxStamina = 4;
    [SerializeField] private float _staminaRegenInterval = 0.5f;
    [SerializeField] private float _staminaRegenAmount = 0.1f;
    private float _currentStamina;
    private float _timeSinceLastRegen;
    
    [Header("Stamina UI")]
    [SerializeField] private Image _staminaBarSegment1;
    [SerializeField] private Image _staminaBarSegment2;
    [SerializeField] private Image _staminaBarSegment3;
    [SerializeField] private Image _staminaBarSegment4;
    [SerializeField] private Color _staminaFilledColor = Color.green; // Цвет заполненной ячейки стамины
    [SerializeField] private Color _staminaEmptyColor = Color.red; 

    public float CurrentStamina { get { return _currentStamina; } }
    #endregion

    public void Initialize(PlayerModel model)
    {
        _playerModel = model;
    }

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        _playerInput.onActionTriggered += OnPlayerInputActionTriggered;    
    }

    private void Start()
    {
        SetGravityScale(Data.gravityScale);
        IsFacingRight = true;

        _cameraFollowObject = _cameraFollow.GetComponent<CameraFollowObject>();
        _cameraFollowObject = FindObjectOfType<CameraFollowObject>();

        _fallSpeedYDampingChangeThreshold = CameraManager.instance.fallSpeedYDampingChangeThreshold;

        _attack = GetComponent<IAttack>();
        
        _currentStamina = _maxStamina; 
        _timeSinceLastRegen = 0f;
        UpdateStaminaUI();
    }

    private void FixedUpdate()
    {
        if (pState.cutScene) return;

        UpdateYVelocity();

        //Handle Run
        if (!IsDashing)
        {
            UpdateDashAnimation();

            if (IsWallJumping)
            {
                Run(Data.wallJumpRunLerp);
            }
            else
            {
                Run(1);
            }
        }
        else if (_isDashAttacking)
        {
            Run(Data.dashEndRunLerp);
        }

        //Handle Slide
        if (IsSliding)
            Slide();

        #region COLLISION CHECKS
        if (!IsDashing && !IsJumping)
        {
            // Ground Check
            if (CheckOverlap(_groundCheckPoint.position, _groundCheckSize, _groundLayer))
            {
                if (LastOnGroundTime < -0.1f)
                {
                    // �������� AnimHandler.justLanded = true;
                    UpdateJumpAnimation();
                }

                LastOnGroundTime = Data.coyoteTime; // if so sets the lastGrounded to coyoteTime
            }

            // Wall Checks
            LastOnWallRightTime = CheckWall(IsFacingRight, _frontWallCheckPoint.position, _backWallCheckPoint.position, _wallCheckSize, _groundLayer);
            LastOnWallLeftTime = CheckWall(!IsFacingRight, _frontWallCheckPoint.position, _backWallCheckPoint.position, _wallCheckSize, _groundLayer);

            // Update LastOnWallTime
            LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
            UpdateWallJumpAnimation();
        }
        #endregion

        recoil.HandleRecoil(_moveInput.y, _bonusJumpsLeft, RB.gravityScale, LastOnGroundTime);

    }

    private void Update()
    {
        if (pState.cutScene) return;
        
        if (!pState.alive)
        {
            _moveInput.x = 0;
            _moveInput.y = 0;
        }
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
        LastOnWallTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;

        LastPressedJumpTime -= Time.deltaTime;
        LastPressedDashTime -= Time.deltaTime;
        #endregion

        #region INPUT HANDLER

        if (_moveInput.x != 0)
            CheckDirectionToFace(_moveInput.x > 0);
        #endregion

        #region JUMP CHECKS
        if (IsJumping && RB.velocity.y < 0)
        {
            IsJumping = false;

            _isJumpFalling = true;
        }

        if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
        {
            IsWallJumping = false;
        }

        if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
            _isJumpCut = false;
            _bonusJumpsLeft = Data.airJumps;

            if (!IsJumping)
                _isJumpFalling = false;
        }

        if (!IsDashing)
        {
            //Jump
            if (CanJump() && LastPressedJumpTime > 0)
            {
                IsJumping = true;
                IsWallJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;
                Jump();

                UpdateJumpAnimation();
            }
            //WALL JUMP
            else if (CanWallJump() && LastPressedJumpTime > 0)
            {
                IsWallJumping = true;
                IsJumping = false;
                _isJumpCut = false;
                _isJumpFalling = false;

                _wallJumpStartTime = Time.time;
                _lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

                WallJump(_lastWallJumpDir);

                //UpdateWallJumpAnimation();
            }
            //DOUBLE JUMP
            else if (LastPressedJumpTime > 0 && _bonusJumpsLeft > 0)
            {
                if (TryConsumeStamina(1))
                {
                    IsJumping = true;
                    IsWallJumping = false;
                    _isJumpCut = false;
                    _isJumpFalling = false;

                    _bonusJumpsLeft--;

                    Jump();
                }
                //AnimHandler.startedJumping = true;
            }
        }
        #endregion

        #region DASH CHECKS
        if (CanDash() && LastPressedDashTime > 0)
        {
            if (TryConsumeStamina(1))
            {
                //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
                Sleep(Data.dashSleepTime);

                //Can dash if direction left or right
                if (_moveInput.x > 0)
                    _lastDashDir = Vector2.right;
                else if (_moveInput.x < 0)
                    _lastDashDir = Vector2.left;
                else
                //Dash where avatar facing
                    _lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;


                if(_lastDashDir == Vector2.left || _lastDashDir == Vector2.right)
                {
                    
                        IsDashing = true;
                        IsJumping = false;
                        IsWallJumping = false;
                        _isJumpCut = false;
                
                        UpdateDashAnimation();

                        DashCoroutine = StartCoroutine(nameof(StartDash), _lastDashDir);
                    
                }
            }
        }
        #endregion

        #region SLIDE CHECKS
        if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
            IsSliding = true;
        else
            IsSliding = false;
        #endregion

        #region GRAVITY
        if (!_isDashAttacking)
        {
            //Higher gravity if we've released the jump input or are falling
            if (IsSliding)
            {
                SetGravityScale(0);
            }
            else if (RB.velocity.y < 0 && _moveInput.y < 0)
            {
                //Much higher gravity if holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (_isJumpCut)
            {
                //Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.velocity.y < 0)
            {
                //Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                //Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            //No gravity when dashing (returns to normal once initial dashAttack phase over)
            SetGravityScale(0);
        }
        #endregion

        #region UPDATE VERTICAL VELOCITY
        //UpdateYVelocity();
        #endregion
        
        #region STAMINA
        if (_currentStamina < _maxStamina)
        {
            _timeSinceLastRegen += Time.deltaTime;
            if (_timeSinceLastRegen >= _staminaRegenInterval)
            {
                RegenStamina(_staminaRegenAmount);
                _timeSinceLastRegen = 0f;
            }
        }
        #endregion
    }

    private void LateUpdate()
    {
        #region CAMERA
        //if we are falling past a certaing speed threshold
        if (RB.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        //if we are standing still or moving up
        if (RB.velocity.y >= 0 && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            //reset so is can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }
        #endregion
    }
    #region WALL CHECKS METHODS
    // single method for overlap check
    private bool CheckOverlap(Vector2 position, Vector2 size, LayerMask layer, bool condition = true)
    {
        return Physics2D.OverlapBox(position, size, 0, layer) && condition;
    }

    private float CheckWall(bool isFacingRight, Vector2 frontWallPoint, Vector2 backWallPoint, Vector2 size, LayerMask layer)
    {
        // Check for front and back walls
        bool frontWall = CheckOverlap(frontWallPoint, size, layer, isFacingRight);
        bool backWall = CheckOverlap(backWallPoint, size, layer, !isFacingRight);

        // If there is an intersection and no wall jump, return coyoteTime
        if ((frontWall || backWall) && !IsWallJumping)
        {
            return Data.coyoteTime;
        }

        // If there is no intersection, return the default value
        return -1f;
    }
    #endregion

    #region INPUT ACTION
    private void OnPlayerInputActionTriggered(InputAction.CallbackContext context)
    {
        if (pState.alive)
        {
                switch (context.action.name)
            {
                case "Move":
                    _moveInput = context.action.ReadValue<Vector2>();
                    UpdateRunAnimation();
                    break;
                case "Jump":
                    switch (context.action.phase)
                    {
                        case InputActionPhase.Started:
                            OnJumpInput();
                            break;
                        case InputActionPhase.Canceled:
                            OnJumpUpInput();
                            break;
                    }
                    break;
                case "Dash":
                    if(context.action.phase == InputActionPhase.Started)
                    OnDashInput();
                    break;
                case "Attack":
                    if (context.action.phase == InputActionPhase.Started)
                        UpdateAttackState();
                    break;
                case "Interact":
                    if (context.action.phase == InputActionPhase.Started)
                    {
                        if (GameManager.Instance.checkpoint != null && GameManager.Instance.checkpoint.inRange == true)
                            GameManager.Instance.checkpoint.Interact();                  
                    }
                    break;
            } 
        }
    }
    #endregion

    public void UpdateRunAnimation()
    {
        _playerModel.UpdateSpeed(_moveInput.x);
    }

    public void UpdateJumpAnimation()
    {  
        _playerModel.UpdateJumpState(IsJumping);
    }

    public void UpdateDashAnimation()
    {
        _playerModel.UpdateDashState(IsDashing);
    }

    private void UpdateYVelocity()
    {
        if (_playerModel == null) return;

        //float verticalVel = RB.velocity.y;

        _playerModel.UpdateVerticalVelocity(RB.velocity.y);
    }
    private void UpdateWallJumpAnimation()
    {
        if (LastOnWallTime > 0 && LastOnGroundTime < 0 && !IsJumping && !IsDashing)
        {
            _playerModel.UpdateWallJumpState(true);     
        }
        else
        {
            _playerModel.UpdateWallJumpState(false);
        }
        //Debug.Log($"UpdateWallJumpAnimation: IsWallJumping = {IsWallJumping}");

    }

    private void RegenStamina(float amount)
    {
        _currentStamina = Mathf.Min(_currentStamina + amount, _maxStamina);
        UpdateStaminaUI();
    }
    
    public bool TryConsumeStamina(int amount)
    {
        if (_currentStamina >= amount)
        {
            _currentStamina -= amount;
            UpdateStaminaUI();
            return true;
        }
        else
        {
            // TODO добавить звук или другое информирование игрока о неудаче.
            return false;
        }
    }
    
    private void UpdateStaminaUI()
    {
        float fillAmount1 = Mathf.Clamp01(Mathf.Min(_currentStamina, 1f));
        float fillAmount2 = Mathf.Clamp01(Mathf.Min(Mathf.Max(_currentStamina - 1f, 0f), 1f));
        float fillAmount3 = Mathf.Clamp01(Mathf.Min(Mathf.Max(_currentStamina - 2f, 0f), 1f));
        float fillAmount4 = Mathf.Clamp01(Mathf.Min(Mathf.Max(_currentStamina - 3f, 0f), 1f));
        
        _staminaBarSegment1.color = (_currentStamina >= 1f) ? _staminaFilledColor : _staminaEmptyColor;
        _staminaBarSegment2.color = (_currentStamina >= 2f) ? _staminaFilledColor : _staminaEmptyColor;
        _staminaBarSegment3.color = (_currentStamina >= 3f) ? _staminaFilledColor : _staminaEmptyColor;
        _staminaBarSegment4.color = (_currentStamina >= 4f) ? _staminaFilledColor : _staminaEmptyColor;
        
        _staminaBarSegment1.fillAmount = fillAmount1;
        _staminaBarSegment2.fillAmount = fillAmount2;
        _staminaBarSegment3.fillAmount = fillAmount3;
        _staminaBarSegment4.fillAmount = fillAmount4;
    }
    
    private void UpdateAttackState()
    {
        _attack.HandleAttack(_moveInput.y, LastOnGroundTime);
        _playerModel.UpdateAttackState();
    }

    #region INPUT CALLBACKS
    //Methods which whandle input detected in Update()
    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut() || CanWallJumpCut())
            _isJumpCut = true;
    }

    public void OnDashInput()
    {
        LastPressedDashTime = Data.dashInputBufferTime;
    }
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
    {
        RB.gravityScale = scale;
    }

    private void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        //nameof() notation means we don't need to input a string directly.
        //Removes chance of spelling mistakes and will improve error messages if any
        StartCoroutine(nameof(PerformSleep), duration);
    }

    private IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
        Time.timeScale = 1;
    }
    #endregion

    //MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = _moveInput.x * Data.runMaxSpeed;
        //We can reduce are control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (LastOnGroundTime > 0)
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
        else
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
        #endregion

        #region Add Bonus Jump Apex Acceleration
        //Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
        {
            accelRate *= Data.jumpHangAccelerationMult;
            targetSpeed *= Data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        //We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
        {
            //Prevent any deceleration from happening, or in other words conserve are current momentum
            //You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
            accelRate = 0;
        }
        #endregion

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

        /*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
    }

    public void Turn()
    {
        //flips player by rotating the "y" axis by 180 degress 
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            pState.lookingRight = false;
            //turn the camera follow object
            //_cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            pState.lookingRight = true;           
            //turn the camera follow object
            //_cameraFollowObject.CallTurn();
        }
            if (_cameraFollowObject == null)
            {
                FindCamera();
            }

            // turn camera if it's find
            _cameraFollowObject?.CallTurn();
            
    }

    private void FindCamera()
    {
        _cameraFollowObject = CameraFollowObject.Instance;

        if (_cameraFollowObject == null)
        {
            Debug.Log("Camera not found!");
            return;
        }
    }
    #endregion

    #region JUMP METHODS
    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;

        #region Perform Jump
        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = Data.jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }

    private void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;

        #region Perform Wall Jump
        Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
            force.x -= RB.velocity.x;

        if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= RB.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring masss
        RB.AddForce(force, ForceMode2D.Impulse);
        #endregion
    }
    #endregion

    #region DASH METHODS
    //Dash Coroutine
    private IEnumerator StartDash(Vector2 dir)
    {
        //Overall this method of dashing aims to mimic Celeste, if you're looking for
        // a more physics-based approach try a method similar to that used in the jump

        LastOnGroundTime = 0;
        LastPressedDashTime = 0;

        float startTime = Time.time;

        _dashesLeft--;
        _isDashAttacking = true;

        SetGravityScale(0);

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= Data.dashAttackTime)
        {
            RB.velocity = dir.normalized * Data.dashSpeed;
            //Pauses the loop until the next frame, creating something of a Update loop. 
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }

        startTime = Time.time;

        _isDashAttacking = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        SetGravityScale(Data.gravityScale);
        RB.velocity = Data.dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
        {
            yield return null;
        }
        //Dash over
        IsDashing = false;
    }

    //Short period before the player is able to dash again
    private IEnumerator RefillDash(int amount)
    {
        //SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
        _dashRefilling = true;
        yield return new WaitForSeconds(Data.dashRefillTime);
        _dashRefilling = false;
        _dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
    }
    #endregion

    #region OTHER MOVEMENT METHODS
    private void Slide()
    {
        //We remove the remaining upwards Impulse to prevent upwards sliding
        if (RB.velocity.y > 0)
        {
            RB.AddForce(-RB.velocity.y * Vector2.up, ForceMode2D.Impulse);
        }

        //Works the same as the Run but only in the y-axis
        //THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
        float speedDif = Data.slideSpeed - RB.velocity.y;
        float movement = speedDif * Data.slideAccel;
        //So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
        //The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

        RB.AddForce(movement * Vector2.up);
    }
    #endregion


    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != IsFacingRight)
            Turn();
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }

    private bool CanWallJump()
    {
        return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
             (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
    }

    private bool CanJumpCut()
    {
        return IsJumping && RB.velocity.y > 0;
    }

    private bool CanWallJumpCut()
    {
        return IsWallJumping && RB.velocity.y > 0;
    }

    private bool CanDash()
    {
        if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
        {
            StartCoroutine(nameof(RefillDash), 1);
        }

        return _dashesLeft > 0;
    }

    public bool CanSlide()
    {
        if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
            return true;
        else
            return false;
    }
    #endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
        Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
    }
    #endregion
}

