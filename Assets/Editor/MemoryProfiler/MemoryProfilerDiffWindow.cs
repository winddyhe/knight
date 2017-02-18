using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assets.Editor.Treemap;
using Treemap;
using UnityEditor;
using UnityEngine;
using System;
using System.Net;
using NUnit.Framework.Constraints;
using UnityEditor.MemoryProfiler;
using Object = UnityEngine.Object;
using System.IO;

namespace MemoryProfilerWindow
{
    public class MemoryProfilerDiffWindow : IMemoryProfilerWindow
    {
        [SerializeField]
        UnityEditor.MemoryProfiler.PackedMemorySnapshot _snapshot;
        [SerializeField]
        UnityEditor.MemoryProfiler.PackedMemorySnapshot _snapshot_prev;

        [SerializeField]
        PackedCrawlerData                               _packedCrawled;
        [SerializeField]
        PackedCrawlerData                               _packedCrewled_prev;

        [NonSerialized]
        CrawledMemorySnapshot                           _unpackedCrawl;
        [NonSerialized]
        CrawledMemorySnapshot                           _unpackedCrawl_prev;

        [SerializeField]
        string                                          _snap_time = "";
        [SerializeField]
        string                                          _snap_prev_time = ""; 

        Vector2 _scrollPosition;

        [NonSerialized]
        private bool _registered = false;
        
        public CrawledMemorySnapshot                    unpackedCrawl       { get { return _unpackedCrawl;      } }
        public CrawledMemorySnapshot                    unpackedCrawlPrev   { get { return _unpackedCrawl_prev; } }

        [MenuItem("Window/MemoryProfilerDiff")]
        static void Create()
        {
            EditorWindow.GetWindow<MemoryProfilerDiffWindow>();
        }

        public void OnEnable()
        {
            Initialize();
        }

        public void OnDisable()
        {
            //    UnityEditor.MemoryProfiler.MemorySnapshot.OnSnapshotReceived -= IncomingSnapshot;
            if (_treeMapView != null)
                _treeMapView.CleanupMeshes ();
        }

        public void Initialize()
        {
            if (_treeMapView == null)
                _treeMapView = new TreeMapView ();
            
            if (!_registered)
            {
                UnityEditor.MemoryProfiler.MemorySnapshot.OnSnapshotReceived += IncomingSnapshot;
                _registered = true;
            }

            if (_unpackedCrawl == null && _packedCrawled != null && _packedCrawled.valid)
                Unpack();

            if (_unpackedCrawl_prev == null && _packedCrewled_prev != null && _packedCrewled_prev.valid)
            {
                _unpackedCrawl_prev = CrawlDataUnpacker.Unpack(_packedCrewled_prev);
            }
        }

        void OnGUI()
        {
            Initialize();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Take Snapshot", GUILayout.Width(100)))
            {
                UnityEditor.MemoryProfiler.MemorySnapshot.RequestNewSnapshot();
            }
            if (GUILayout.Button("Save Snapshot", GUILayout.Width(100))) 
            {
                if (_snapshot != null)
                {
                    string fileName = EditorUtility.SaveFilePanel("Save Snapshot", null, "MemorySnapshot", "memsnap"); 
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        using (Stream stream = File.Open(fileName, FileMode.Create))
                        {
                            bf.Serialize(stream, _snapshot);
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("No snapshot to save.  Try taking a snapshot first.");
                }
            }
            if (GUILayout.Button("Load Snapshot", GUILayout.Width(100)))
            {
                string fileName = EditorUtility.OpenFilePanel("Load Snapshot", null, "memsnap");
                if (!string.IsNullOrEmpty(fileName))
                {
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    using (Stream stream = File.Open(fileName, FileMode.Open))
                    {
                        IncomingSnapshot(bf.Deserialize(stream) as PackedMemorySnapshot);
                    }
                }
            }

            if (GUILayout.Button("Snap Diff Compare", GUILayout.Width(130)))
            {
                if (_snapshot_prev == null)
                {
                    Debug.LogError("Has not prev snap to compare..");
                    return;
                }
                MemorySnapDiffWindow.Create();
                MemorySnapDiffWindow.Instance.SetSnapshot(this);
            }

            if (_snapshot != null)
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("CurrSnap " + _snap_time, GUILayout.Width(120)); 
                GUI.color = Color.white;
            }
            if (_snapshot_prev != null)
            {
                GUI.color = Color.red;
                EditorGUILayout.LabelField("PrevSnap " + _snap_prev_time, GUILayout.Width(120));
                GUI.color = Color.white;
            }
            GUILayout.EndHorizontal();

            if (_treeMapView != null)
                _treeMapView.Draw();
            if (_inspector != null)
                _inspector.Draw();

            //RenderDebugList();
        }

        public override void SelectThing(ThingInMemory thing)
        {
            _inspector.SelectThing(thing);
            _treeMapView.SelectThing(thing);
        }

        public override void SelectGroup(Group group)
        {
            _treeMapView.SelectGroup(group);
        }

        private void RenderDebugList()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            foreach (var thing in _unpackedCrawl.allObjects)
            {
                var mo = thing as ManagedObject;
                if (mo != null)
                    GUILayout.Label("MO: " + mo.typeDescription.name);

                var gch = thing as GCHandle;
                if (gch != null)
                    GUILayout.Label("GCH: " + gch.caption);

                var sf = thing as StaticFields;
                if (sf != null)
                    GUILayout.Label("SF: " + sf.typeDescription.name);
            }

            GUILayout.EndScrollView();
        }

        void Unpack()
        {
            _unpackedCrawl = CrawlDataUnpacker.Unpack(_packedCrawled);
            _inspector = new Inspector(this, _unpackedCrawl, _snapshot);

            if(_treeMapView != null)
                _treeMapView.Setup(this, _unpackedCrawl);
        }

        void IncomingSnapshot(PackedMemorySnapshot snapshot)
        {
            SetPrevSnapshot(_snapshot);

            _snapshot = snapshot;
            _snap_time = DateTime.Now.ToString("H:mm:ss");

            _packedCrawled = new Crawler().Crawl(_snapshot);
            Unpack();
        }

        void SetPrevSnapshot(PackedMemorySnapshot snapshot)
        {
            if (snapshot == null) return;

            _snap_prev_time = _snap_time;
            _snapshot_prev = snapshot;
            _packedCrewled_prev = new Crawler().Crawl(_snapshot_prev);
            _unpackedCrawl_prev = CrawlDataUnpacker.Unpack(_packedCrewled_prev); 
        }
    }
}
