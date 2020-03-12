using System.Collections.Generic;
using InfallibleCode.Completed;
using System.Collections;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.TestTools;

namespace Editor.Tests
{
    public class BuildingsTest
    {
        private BuildingUpdateJob _job;
        private NativeArray<Building.Data> _buildingDataArray;
        
        [UnityTest]
        public IEnumerator PowerConsumptionTest()
        {
            var buildManagerGameObject = new GameObject();
            var manager = buildManagerGameObject.AddComponent<BuildingManager>();
            var buildingComponent = buildManagerGameObject.AddComponent<Building>();
            buildingComponent.floors = 10;

            manager.buildings = new List<Building>
            {
                buildingComponent
            };

            var buildingData = new Building.Data[manager.buildings.Count];
            for (var i = 0; i < buildingData.Length; i++)
            {
                buildingData[i] = new Building.Data(manager.buildings[i]);
            }
            
            _buildingDataArray = new NativeArray<Building.Data>(buildingData, Allocator.Persistent);
            
            _job = new BuildingUpdateJob
            {
                BuildingDataArray = _buildingDataArray
            };
            
            var jobHandle = _job.Schedule(manager.buildings.Count, 1);
            jobHandle.Complete();
            
            Assert.True(buildingData[0].PowerUsage > 0 ); // surprise. it is zero.
            
            yield return null;
        }
    }
}
