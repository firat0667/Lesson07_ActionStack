using UnityEngine;

namespace Game.Combat
{
    public interface IDamageable
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }

        void TakeDamage(int amount);
        void OnDeath();
    }
}
