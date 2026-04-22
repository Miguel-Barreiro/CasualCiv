using System.Collections.Generic;
using Core.Model.Data;
using Core.Model.Stats;
using UnityEngine;

namespace Game.Entities
{
    [CreateAssetMenu(fileName = "New EntityConfig", menuName = "Game/Entities/EntityConfig")]
    public sealed class EntityConfig : DataConfig
    {
        public GameObject Prefab;


        [Space]
        public List<StatOverride> StatOverrides;
        [Space, Range(1.5f, 4f)]
        [SerializeField] private float _PlayerCameraClearanceRadius = 2;

        
        
        public float PlayerCameraClearanceRadius => _PlayerCameraClearanceRadius;
    }
    
    
    
}
