﻿using System.Linq;
using Assets.Scripts.GamePlay;
using PathCreation;
using UnityEngine;

public class SpaceShipEnemy : Enemy
{
    private float _nextShootTime;
    private GameObject[] _bulletPoint;
    private AudioSource _bulletAudio;
    public int MaxHealth = 100;

    public bool CanShoot = true;
    public float ShootRate = 1;
    public GameObject BulletTemplate;

    public SpaceShipEnemy()
    {
        Health = 2;
        ScoreValue = 20;
    }


    protected override void Start()
    {
        base.Start();
        // 查找当前对象的子物体,即敌人子弹的发射位置
        _bulletPoint = gameObject.FindComponentsInChildWithTag(ObjectTags.BulletPoints);
        _bulletAudio = GetComponent<AudioSource>();
    }

    protected override void Update()
    {
        base.Update();

        var isTimeToShoot = Time.time >= _nextShootTime;
        if (isTimeToShoot && CanShoot)
            Shoot();
    }


    public void Shoot()
    {
        for (int i = 0; i < _bulletPoint.Length; i++)
        {
            // 初始化 敌人的子弹，根据敌人的位置和方向 确定子弹的位置和方向
            var bullet = Instantiate(BulletTemplate, _bulletPoint[i].transform.position, _bulletPoint[i].transform.rotation); // rotation.z为1，即-180度，
                                                                                                                              // var bullet = Instantiate(BulletTemplate, _bulletPoint.transform.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().SetAsEnemyBullet();
        }
        if (_bulletAudio != null &&  _bulletAudio.enabled) _bulletAudio.Play();

        _nextShootTime = Time.time + (1f / ShootRate) + Random.Range(0f, 0.5f); // add some randomness time while shooting
    }

    public override void Spawn(DifficultyLevel difficulty, EnemyModeOptions mode)
    {
        base.Spawn(difficulty, mode);
        //transform.localPosition =  transform.EnsurePositionInScreenBoundaries(transform.position);

        ShootRate = difficulty == DifficultyLevel.Easy ? 0.5f :
                    difficulty == DifficultyLevel.Normal ? ShootRate : 3;

        if (mode.IsValidPathMode)
        {
            Speed *= 2;
        }
    }
}