using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HodimBrodim
{
    public class Fighter
    {
        public float Health { get; protected set; }
        public float Damage { get; protected set; }
        public float Armor { get; protected set; }
        public string Name { get; private set; }

        private string _specialAbilities;
        public Fighter(string name, float health, float armor, float damage,
            string special = " отсутствуют")
        {
            Name = name;
            Health = health;
            Armor = armor;
            Damage = damage;
            _specialAbilities = special;
        }
        public void ShowFighterStats()
        {
            Console.WriteLine($"Боец {Name} обладает {Health} хп, " +
                $"{Armor} брони и наносит {Damage} урона. \nСпециальные способности: {_specialAbilities}");
        }
        public void TakeDamage(float damage)
        {
            float trueDamage = damage - (damage * (Armor / 10));
            Health -= trueDamage;
        }
        
        public void ShowRoundStatistic(float enemyFighterDamage)
        {
            Console.WriteLine($"По персонажу {Name} нанесено " +
                $"{enemyFighterDamage - (enemyFighterDamage * (Armor / 10))} " +
                $"урона, у него осталось {Health} здоровья");
        }

        public void ChangeHealth(float value) => Health += value;
        public void ChangeArmor(float value) => Armor += value;
        public void ChangeDamage(float value) => Damage += value;
    }
}
