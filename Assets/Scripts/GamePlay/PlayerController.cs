using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GamePlay;
using DG.Tweening;
using UnityEngine;

// 控制玩家移动，攻击，防御，升级
public class PlayerController : GamePlayObject
{
    private const float SHIELD_USAGE_POWER = 0.20f;
    private const float SHIELD_MAX_POWER = 3f;
    private const int SHOOT_MAX_POWER = 6;
    private const int HEALTH_MAX_POINTS = 99;

    private Rigidbody2D _rb;
    private Animator _animator;
    private GameObject[] _bulletPoints;
    private AudioSource _bulletAudio;
    private SpriteRenderer _spriteRenderer;

    private Vector2 _movement;
    private bool _shootPressed;
    private bool _canShoot;
    private bool _shielded;
    private float _nextShootTime;
    public int ShootingPower = 1;
    private readonly Dictionary<int, int[]> _shootingPointsPerPower = new Dictionary<int, int[]>
    {
        { 1, new [] { 0 } },
        { 2, new [] { 1, 2 } },
        { 3, new [] { 0, 1, 2 } },
        { 4, new [] { 0, 1, 2, 3, 4 } },
        { 5, new [] { 0, 1, 2, 3, 4, 5, 6 } },
        { 6, new [] { 0, 1, 2, 3, 4, 5, 6, 7, 8 } },
    };
    private readonly HashSet<string> _nonHittableTags = new HashSet<string> { ObjectTags.PlayerBullet, ObjectTags.PowerUp };

    public bool CanTakeDamage = true;
    public float ShootRate = 4f;
    public float ShieldPower = 1f;
    public GameObject ShieldEffect;
    public Color ShieldColor = new Color(130, 255, 250);
    public GameObject BulletTemplate;
    public Ease ShieldEasyFX = Ease.Linear;

    public PlayerController()
    {
        Health = 3;
        Speed = 6;
    }

    // game events
    protected override void Start()
    {
        base.Start();

        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _bulletPoints = gameObject.FindComponentsInChildWithTag(ObjectTags.BulletPoints);
        _bulletAudio = GetComponent<AudioSource>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        // inputs
        //_movement.x = Input.GetAxisRaw("Horizontal");
        //_movement.y = Input.GetAxisRaw("Vertical");
        //MovePlayer();

        // attack
        _canShoot = Time.time >= _nextShootTime;
        _shootPressed = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0);
        if (_canShoot && _shootPressed)
        {
            Shoot();
        }

        // 使用保护罩
        var shieldPressed = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.JoystickButton1);
        _shielded = shieldPressed && ShieldPower >= 0 && !IsInvulnerable();
        ProcessShieldDefense();
    }

    void FixedUpdate()
    {
        // movement
        _movement.x = Input.GetAxis("Horizontal") + Input.GetAxis("CrossX");
        _movement.y = Input.GetAxis("Vertical") + Input.GetAxis("CrossY");
        _rb.velocity = _movement * Speed;                           //Add Velocity to the player ship rigidbody

        // animation
        _animator.SetFloat("DirectionX", _movement.x);
        _rb.position = transform.EnsurePositionInScreenBoundaries(_rb.position);
    }

    // methods
    protected override void OnHit(Collider2D other)
    {
        base.OnHit(other);
        base.MakeInvulnerable();
    }

    protected override bool IsHittableTag(string otherTag) => CanTakeDamage && !_shielded && !_nonHittableTags.Contains(otherTag);

    public void Shoot()
    {
        // shoot accordingly with power 
        var points = _shootingPointsPerPower[ShootingPower];
        foreach (var point in points)
        {
            var bulletPoint = _bulletPoints[point];
            var bullet = Instantiate(BulletTemplate, bulletPoint.transform.position, bulletPoint.transform.rotation);
            bullet.GetComponent<BulletController>().SetAsPlayerBullet();
        }

        // TODO: shoot animation
        _bulletAudio.Play();

        _nextShootTime = Time.time + 1f / ShootRate;
        _canShoot = false;
    }

    public void ApplyPowerUp(PowerUpType powerUp)
    {
        switch (powerUp)
        {
            case PowerUpType.Shooting:
                ShootingPower = Mathf.Clamp(ShootingPower + 1, 1, SHOOT_MAX_POWER);
                break;
            case PowerUpType.Shield:
                ShieldPower = Mathf.Clamp(ShieldPower + 1f, 0, SHIELD_MAX_POWER);
                break;
            case PowerUpType.Health:
                Health = Mathf.Clamp(Health + 1, 1, HEALTH_MAX_POINTS);
                break;
        }
    }

    // helpers
    private void ProcessShieldDefense()
    {
        _spriteRenderer.color = _shielded ? ShieldColor : Color.white;

        ShowShieldEffect();

        if (_shielded)
        {
            ShieldPower -= Mathf.Clamp(SHIELD_USAGE_POWER * Time.deltaTime, 0, SHIELD_MAX_POWER);
            if ( ShieldPower < 0 ) ShieldPower = 0;
            // ShieldPower = float.Parse(ShieldPower.ToString("F2"));
        }
    }

    private void ShowShieldEffect()
    {
        var changed = ShieldEffect.activeInHierarchy != _shielded;
        if (changed)
        {
            ShieldEffect.GetComponentsInChildren<Component>()
                        .Select(c => c.GetComponent<SpriteRenderer>())
                        .Where(c => c != null)
                        .ToList()
                        .ForEach(c =>
                         {
                             c.DOFade(1, 1).From(0).SetEase(ShieldEasyFX);
                         });
        }

        //ShieldEffect.transform.DOScaleX(0, 5).From(1).SetEase(ShieldEasyFX);
        ShieldEffect?.SetActive(_shielded);
    }

    private void MovePlayer()
    {
        var pos = _rb.position + _movement * Speed * Time.fixedDeltaTime;
        pos = transform.EnsurePositionInScreenBoundaries(pos);
        _rb.MovePosition(pos);
    }

}
