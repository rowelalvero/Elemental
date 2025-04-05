public interface IEnemy
{
    // Property to check if the enemy is currently in the process of attacking.
    bool IsAttacking { get; }

    // Method to execute the attack, with a callback for when the attack is complete.
    void Attack(System.Action onAttackComplete);
}
