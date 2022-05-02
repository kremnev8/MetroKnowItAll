using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public enum PainType
    {
        Physical,
        Mental
    }
    
    /// <summary>
    /// Represebts the current vital statistics of some game entity.
    /// </summary>
    public class Health : MonoBehaviour
    {
        [Serializable]
        public class IntEvent : UnityEvent<int>
        {
            
        }
        
        [Serializable]
        public class PainEvent : UnityEvent<PainType>
        {
            
        }
        
        private int currentHp;
        private int lastHP;
        public int maxHp = 1;
        public bool IsAlive => currentHp > 0;

        public bool regenerate = false;
        public int regenerateRate = 1;

        private float timeElapsed;

        public IntEvent hurt;
        public PainEvent hurtBy;
        
        public IntEvent healed;
        public UnityEvent isZero;

        /// <summary>
        /// Increment the HP of the entity.
        /// </summary>
        public void Increment(int heal = 1)
        {
            currentHp = Mathf.Clamp(currentHp + heal, 0, maxHp);
            lastHP = currentHp;
            healed?.Invoke(currentHp);
        }

        /// <summary>
        /// Decrement the HP of the entity. Will trigger a HealthIsZero event when
        /// current HP reaches 0.
        /// </summary>
        public void Decrement(int damage = 1, PainType type = PainType.Physical) 
        {
            currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);
            if (currentHp == lastHP)
            {
                lastHP = currentHp;
                return;
            };
            lastHP = currentHp;
            
            if (currentHp == 0)
                isZero?.Invoke();
            hurt?.Invoke(currentHp);
            hurtBy?.Invoke(type);
            
        }

        /// <summary>
        /// Decrement the HP of the entitiy until HP reaches 0.
        /// </summary>
        public void Die()
        {
            while (currentHp > 0) Decrement();
        }

        void Awake()
        {
            currentHp = maxHp;
            lastHP = maxHp;
        }

        private void Update()
        {
            if (!regenerate) return;
            if (currentHp >= maxHp) return;
            
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= 1)
            {
                Increment(regenerateRate);
                timeElapsed = 0;
            }
        }
    }
}
