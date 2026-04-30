using System.Collections.Generic;
using Core.Model.Data;
using Core.Model.Stats;
using UnityEngine;

namespace Game.Entities
{
    [CreateAssetMenu(fileName = "New EntityConfig", menuName = "Game/Entities/EntityConfig")]
    public class EntityConfig : DataConfig
    {
        public GameObject Prefab;
        
        [Space]
        public List<StatOverride> StatOverrides;
        
        [Space]
        public List<VSAbilityDataConfig> Abilities;
        
    }
    
    
    
}
