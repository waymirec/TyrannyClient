using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Events;
using NLog;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Managers
{
    public class WorldEntityManager : Singleton<WorldEntityManager>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private readonly IDictionary<Guid, GameObject> _entities = new ConcurrentDictionary<Guid, GameObject>();
        private NetworkEventManager _networkEventManager;
        
        protected override void OnAwake()
        {
            _networkEventManager = Registry.Get<NetworkEventManager>();
        }


        private void OnEnable()
        {
            _networkEventManager.OnEnterWorld += OnEnterWorld;
            _networkEventManager.OnSpawnWorldEntity += OnSpawnWorldEntity;
            _networkEventManager.OnDestroyWorldEntity += OnDestroyWorldEntity;
            _networkEventManager.OnMoveWorldEntity += OnMoveWorldEntity;
        }

        private void OnDisable()
        {
            _networkEventManager.OnEnterWorld -= OnEnterWorld;
            _networkEventManager.OnSpawnWorldEntity -= OnSpawnWorldEntity;
            _networkEventManager.OnDestroyWorldEntity -= OnDestroyWorldEntity;
            _networkEventManager.OnMoveWorldEntity -= OnMoveWorldEntity;
        }

        private void OnEnterWorld(object source, NetworkEnterWorldEventArgs args)
        {
            var guid = args.Guid;
            var position = args.Position;
            
            Debug.Log($"Creating player object {position.x},{position.y},{position.z}");
            var entity = CreateWorldEntity(guid, position, "Hero");
            entity.AddComponent<ClickToMove>();
            
            SetFollowCamera(entity.transform);
        }
        
        private void OnSpawnWorldEntity(object source, NetworkSpawnWorldEntityEventArgs args)
        {
            var guid = args.Guid;
            var position = args.Position;
            
            Debug.Log($"Spawning GO {guid.ToString()} at {position.x},{position.y},{position.z}");
            var entity = CreateWorldEntity(guid, position, "Zombie");
        }

        private void OnDestroyWorldEntity(object source, NetworkDestroyWorldEntityEventArgs args)
        {
            Assert.IsTrue(_entities.ContainsKey(args.Guid));
            Destroy(_entities[args.Guid]);
        }

        private void OnMoveWorldEntity(object source, NetworkMoveWorldEntityArgs args)
        {
            Assert.IsTrue(_entities.ContainsKey(args.Guid));
            var entity = _entities[args.Guid];
            entity.GetComponent<NavMeshAgent>().SetDestination(args.Destination);
        }
        
        private GameObject CreateWorldEntity(Guid id, Vector3 position, string resourceName)
        {
            var entity = Instantiate(Resources.Load("Hero"), position, Quaternion.identity) as GameObject;
            Assert.IsNotNull(entity);
            entity.name = id.ToString();
            _entities[id] = entity;
            return entity;
        }

        private void SetFollowCamera(Transform target)
        {
            if (Camera.main != null)
            {
                Camera.main.GetComponent<FollowCamera>().target = target;
            }
        }
    }
}