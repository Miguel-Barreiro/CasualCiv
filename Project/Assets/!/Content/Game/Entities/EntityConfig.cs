using Core.Model.Data;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Entities
{
    [CreateAssetMenu(fileName = "New EntityConfig", menuName = "Game/Entities/EntityConfig")]
    public sealed class EntityConfig : DataConfig
    {
        public GameObject Prefab;

    }
}
