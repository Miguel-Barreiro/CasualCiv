using System;
using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using Game.Board;
using Global;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game
{
    [Flags]
    public enum WorldObjectType : byte
    {
        Floor   = 1 << 0,
        Surface = 1 << 1,
        Object  = 1 << 2,
        Air     = 1 << 3,
    }

    public interface IWorldComponent : Component<WorldEntityComponentData> { }

    [StructLayout(LayoutKind.Auto)]
    public struct WorldEntityComponentData : IComponentData
    {

        public EntId ID { get; set; }
        public Vector2Int TilePosition;
        public WorldObjectType ObjectType;
        public TileBase Tile;

        public void Init()
        {
            TilePosition = BoardSystem.FLOATING_POSITION;
        }
        
        public bool IsFloating() => TilePosition == BoardSystem.FLOATING_POSITION;
    }
    
}
