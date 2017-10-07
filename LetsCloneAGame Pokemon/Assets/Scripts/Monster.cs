using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokemonTypes {
  Normal,  Fighting,
  Ice,     Ghost,
  Bug,     Flying,
  Steel,   Grass,
  Dragon,  Electric,
  Fire,    Fairy,
  Poison,  Ground,
  Water,   Psychic,
  Rock,    Dark
}

public enum SpecialConditions {
  Asleep, Confused,  
  Burned, Poisoned,
  Paralyzed
}

public enum PokemonStages{ Basic, Stage1, Stage2 }

[System.Serializable]
public class Monster {
  // Declare basic stats
  // The following are modifiable values for the "pristine" statistics
  public string Name = "MissngNo";
  public int    PokemonNumber = -1;

  public float  HP        = 0;
  public float  Attack    = 0;
  public float  SpAttack  = 0;
  public float  Defense   = 0;
  public float  SpDefense = 0;
  public float  Speed     = 0;
  public List<PokemonTypes> PokemonType = new List<PokemonTypes>();

  // The following are hidden and reflect the pokemon's current values
   public float HPNow         = 0;
   public float AttackNow     = 0;
   public float SpAttackNow   = 0;
   public float DefenseNow    = 0;
   public float SpDefenseNow  = 0;
   public float SpeedNow      = 0;
   public List<SpecialConditions> CurrentCondition = new List<SpecialConditions>();
}