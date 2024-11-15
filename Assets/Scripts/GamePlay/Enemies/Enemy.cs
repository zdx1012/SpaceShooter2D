﻿using System.Collections.Generic;
using Assets.Scripts.GamePlay;
using PathCreation;
using UnityEngine;

public abstract class Enemy : GamePlayObject
{
    public int ScoreValue = 10;

    protected EnemyModeOptions ModeOpts = EnemyModeOptions.Default;

    private readonly HashSet<string> _hittableTags = new HashSet<string> { ObjectTags.Player, ObjectTags.PlayerBullet };
    private float _distanceTraveled;

    // unity events
    protected override void Start()
    {
        base.Start();

        // set the default mode
        transform.rotation = new Quaternion(0, 0, 180, 0);
        GetComponent<Rigidbody2D>().velocity = transform.up * Speed;
    }

    protected override void Update()
    {
        base.Update();

        // 设置按指定路径移动的位置
        if (ModeOpts.IsValidPathMode)
        {
            _distanceTraveled += Speed * Time.deltaTime;
            // 移动
            transform.position = ModeOpts.Path.path.GetPointAtDistance(_distanceTraveled, ModeOpts.EndOfPathMode);
        }
    }

    // methods
    public virtual void Spawn(DifficultyLevel difficulty, EnemyModeOptions mode)
    {
        ModeOpts = mode;
    }

    protected override bool IsHittableTag(string otherTag) => _hittableTags.Contains(otherTag);

    protected override void Die()
    {
        // TODO: Improve it to use a messaging systems
        Game.Current.EnemyKilled(this);
        base.Die();
    }
    public override void SelfDestroy(){
        Die();
    }
}

public class EnemyModeOptions
{
    public EnemyMode Mode { get; private set; }
    public PathCreator Path { get; private set; }
    public EndOfPathInstruction EndOfPathMode { get; private set; }
    public bool IsValidPathMode => Mode == EnemyMode.Path && Path != null;

    public EnemyModeOptions(EnemyMode mode, GameObject path, EndOfPathInstruction endOfPathMode)
    {
        Mode = mode;
        Path = path != null ? path.GetComponent<PathCreator>() : null;
        EndOfPathMode = endOfPathMode;
    }

    public static EnemyModeOptions Default => new EnemyModeOptions(EnemyMode.Default, null, EndOfPathInstruction.Stop);
}