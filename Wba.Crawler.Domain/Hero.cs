using System;
using System.Collections.Generic;
using System.Text;

namespace Wba.Crawler.Domain
{
    class Hero
    {
        public string Name { get; set; }
        public string PrimaryAttribute { get; set; }
        public int startStrength { get; set; }
        public int startAgility { get; set; }
        public int startIntelligence { get; set; }
        public double StrengthPerLevel { get; set; }
        public double AgilityPerLevel { get; set; }
        public double IntelligencePerLevel { get; set; }
        public int Health { get; set; }
        public double HealthRegen { get; set; }
        public double MagicalResistance { get; set; }
        public int Mana { get; set; }
        public double ManaRegen { get; set; }
        public double SpellDamageAmplification { get; set; }
        public double Armor { get; set; }
        public double AttacksPerSecond { get; set; }
        public double MovementSpeedAmplification { get; set; }
        public double Damage { get; set; }
        public int MovementSpeed { get; set; }
        public double AttackSpeed { get; set; }
        public double TurnRate { get; set; }
        public int DayVisionRange { get; set; }
        public int NightVisionRange { get; set; }
        public int AttackRange { get; set; }
        public int ProjectileSpeed { get; set; }
        public string AttackAnimation { get; set; }
        public double BaseAttackTIme { get; set; }
        public int DamageBlock { get; set; }
        public int CollisionSize { get; set; }
        public int Legs { get; set; }
        public string GibType { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string AttackType { get; set; }
        public string Image { get; set; }
        public string Lore { get; set; }
        public string VoiceActor { get; internal set; }
    }
}
