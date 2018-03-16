using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.TiledMap;
using Core;

namespace Test
{
    public class TiledMapTest : MonoBehaviour
    {
        public  TiledMapManager TiledMapManager;
        public  uint            RandSeed            = 123456;
        public  int             TileNum             = 17;
        public  int             TileWidth           = 10;
        public  int             TileHeight          = 10;

        private RandGenerator   mRandGenerator;

        void Start()
        {
            mRandGenerator = new RandGenerator(this.RandSeed);
            this.TiledMapManager.Initialize(Vector3.zero, this.TileWidth, this.TileHeight, this.TileNum, mRandGenerator);
        }

        void OnGUI()
        {
            if (GUILayout.Button("Random Generate"))
            {
                this.TiledMapManager.Destroy();

                this.RandSeed = (uint)Random.Range(0, 100000000);
                this.mRandGenerator = new RandGenerator(this.RandSeed);
                this.TiledMapManager.Initialize(Vector3.zero, this.TileWidth, this.TileHeight, this.TileNum, mRandGenerator);
            }
        }
    }
}
