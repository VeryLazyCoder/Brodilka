using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HodimBrodim
{
    public class Fighter
    {
        private float _health;
        private float _armor;
        private float _damage;
        private string _name;
        private string _specialAbilities;
        public Fighter(string name, float health, float armor, float damage,
            string special = " отсутствуют")
        {
            _name = name;
            _health = health;
            _armor = armor;
            _damage = damage;
            _specialAbilities = special;
        }
        public void ShowFighterStats()
        {
            Console.WriteLine($"Боец {_name} обладает {_health} хп, " +
                $"{_armor} брони и наносит {_damage} урона. \nСпециальные способности: {_specialAbilities}");
        }
        public void TakeDamage(float damage)
        {
            float trueDamage = damage - (damage * (_armor / 10));
            _health -= trueDamage;
        }
        public float Health
        {
            get { return _health; }
            set { _health += value; }
        }
        public float Damage
        {
            get { return _damage; }
            set { _damage += value; }
        }
        public string Name
        {
            get { return _name; }
        }
        public float Armor
        {
            get { return _armor; }
            set { _armor += value; }
        }
        public void ShowRoundStatistic(float enemyFighterDamage)
        {
            Console.WriteLine($"По персонажу {_name} нанесено " +
                $"{enemyFighterDamage - (enemyFighterDamage * (_armor / 10))} " +
                $"урона, у него осталось {_health} здоровья");
        }
    }
}
