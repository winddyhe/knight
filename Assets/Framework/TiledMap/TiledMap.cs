using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

namespace Framework.TiledMap
{
    public enum TiledDirection
    {
        Up,
        Right,
        Down,
        Left,
        UpLeft,
        UpRight,
        DownRight,
        DownLeft,
    }

    [System.Serializable]
    public class TiledIndex
    {
        public static Dict<TiledDirection, Func<TiledIndex, TiledIndex>> TiledMoveActions = new Dict<TiledDirection, Func<TiledIndex, TiledIndex>>()
        {
            { TiledDirection.Up,        MoveUp          },
            { TiledDirection.Right,     MoveRight       },
            { TiledDirection.Down,      MoveDown        },
            { TiledDirection.Left,      MoveLeft        },
            { TiledDirection.UpLeft,    MoveUpLeft      },
            { TiledDirection.UpRight,   MoveUpRight     },
            { TiledDirection.DownRight, MoveDownRight   },
            { TiledDirection.DownLeft,  MoveDownLeft    },
        };

        public static Dict<TiledDirection, int[]> TiledDirIndices = new Dict<TiledDirection, int[]>()
        {
            { TiledDirection.Up,    new int[2] { 0, 1 } },
            { TiledDirection.Right, new int[2] { 1, 3 } },
            { TiledDirection.Down,  new int[2] { 2, 3 } },
            { TiledDirection.Left,  new int[2] { 0, 2 } }
        };

        public int X;
        public int Z;

        public static TiledIndex MoveUp(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X,
                Z = rTiledIndex.Z + 1,
            };
        }

        public static TiledIndex MoveRight(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X + 1,
                Z = rTiledIndex.Z,
            };
        }

        public static TiledIndex MoveDown(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X,
                Z = rTiledIndex.Z - 1,
            };
        }

        public static TiledIndex MoveLeft(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X - 1,
                Z = rTiledIndex.Z,
            };
        }

        public static TiledIndex MoveUpLeft(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X - 1,
                Z = rTiledIndex.Z + 1,
            };
        }

        public static TiledIndex MoveUpRight(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X + 1,
                Z = rTiledIndex.Z + 1,
            };
        }

        public static TiledIndex MoveDownRight(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X + 1,
                Z = rTiledIndex.Z - 1,
            };
        }

        public static TiledIndex MoveDownLeft(TiledIndex rTiledIndex)
        {
            return new TiledIndex()
            {
                X = rTiledIndex.X - 1,
                Z = rTiledIndex.Z - 1,
            };
        }

        public static bool IsEquals(TiledIndex rTiledIndex1, TiledIndex rTiledIndex2)
        {
            return rTiledIndex1.X == rTiledIndex2.X && rTiledIndex1.Z == rTiledIndex2.Z;
        }

        public static TiledDirection GetRevertDirection(TiledDirection rTiledDirection)
        {
            if (rTiledDirection == TiledDirection.Up)
                return TiledDirection.Down;
            else if (rTiledDirection == TiledDirection.Down)
                return TiledDirection.Up;
            else if (rTiledDirection == TiledDirection.Right)
                return TiledDirection.Left;
            else if (rTiledDirection == TiledDirection.Left)
                return TiledDirection.Right;
            return 0;
        }
    }

    /// <summary>
    /// Tiled Index : 
    ///     ---------
    ///     |  0 1  |
    ///     |  2 3  |
    ///     ---------
    /// </summary>
    public class TiledMap : MonoBehaviour
    {
        public List<int>    Indices;
        public TiledIndex   TiledIndex;
        public int          RandWeight  = 1;
        
        public bool CheckIsSameEdge(TiledDirection rTiledDir, TiledMap rTiledMap)
        {
            TiledDirection rRevertDir = TiledIndex.GetRevertDirection(rTiledDir);

            var rIndices = TiledIndex.TiledDirIndices[rTiledDir];
            var rRevertIndices = TiledIndex.TiledDirIndices[rRevertDir];

            return this.Indices[rRevertIndices[0]] == rTiledMap.Indices[rIndices[0]] && 
                   this.Indices[rRevertIndices[1]] == rTiledMap.Indices[rIndices[1]];
        }
    }
}

