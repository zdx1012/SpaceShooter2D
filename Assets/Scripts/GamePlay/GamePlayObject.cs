﻿using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.GamePlay
{
    public abstract class GamePlayObject : MonoBehaviour
    {
        private int _sourceLayer;
        private float _invulnerableTime;
        private SpriteRenderer _renderer;
        private Sequence _invulnerableEffect;

        public int Health = 1;
        public float Speed = 1;
        public GameObject ExplosionTemplate;
        public GameObject HitTemplate;

        [HideInInspector]
        public bool Destroyed { get; internal set; }

        // unity events
        protected virtual void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            // Debug.Log($"_renderer.color={_renderer.color}");
            if (IsInvulnerable())
            {
                var invulnerabilityExpired = Time.time > _invulnerableTime;
                if (invulnerabilityExpired)
                {
                    gameObject.layer = _sourceLayer;
                    Debug.Log($"{gameObject.name} no longer invulnerable. time '{Time.time}'");
                }
            }

            if (Health <= 0)
            {
                Die();
            }
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (IsHittableTag(other.tag))
            {
                Health--;
                OnHit(other);
                PlayHitEffect(other);
            }
        }

        // custom methods
        public bool IsInvulnerable() => gameObject.layer == (int)Layers.Invulnerable;

        protected virtual bool IsHittableTag(string otherTag) => true;

        protected virtual void OnHit(Collider2D other)
        {
        }

        protected void MakeInvulnerable(float duration = 2f)
        {
            if (IsInvulnerable())
                return;

            // 显示无敌时的闪烁效果
            _invulnerableEffect = Effetcs.StartInvulnerableEffect(_renderer, duration);

            _invulnerableTime = Time.time + duration;
            _sourceLayer = gameObject.layer;
            gameObject.layer = (int)Layers.Invulnerable;

            Debug.Log($"{gameObject.name} invulnerable until {_invulnerableTime}");
        }



        // 被撞击后，闪烁效果
        protected virtual void PlayHitEffect(Collider2D other)
        {
            if (HitTemplate == null)
                return;

            var hit = Instantiate(HitTemplate, other.transform.position, Quaternion.identity);
            Destroy(hit, hit.GetComponent<ParticleSystem>().main.duration);
        }

        // 对象死亡，触发爆炸效果
        protected virtual void Die()
        {
            Destroyed = true;

            var explosionFX = Instantiate(ExplosionTemplate, transform.position, transform.rotation);
            Destroy(explosionFX, explosionFX.GetComponent<ParticleSystem>().main.duration);
            Destroy(gameObject);
            // Debug.Log(gameObject.name);
        }

        public virtual void SelfDestroy(){}
    }
}
