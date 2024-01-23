
using System;
using Unity.Netcode;
using UnityEngine;

    public class markerSpawn : NetworkBehaviour
    {
        public GameObject marker;
        public bool DestroyWithSpawner;
        private GameObject m_PrefabInstance;
        private NetworkObject m_SpawnedNetworkMarker;

        public override void OnNetworkSpawn()
        {
            // Only the server spawns, clients will disable this component on their side
            enabled = IsServer;
            if (!enabled || marker == null)
            {
                return;
            }
            // Instantiate the GameObject Instance
            m_PrefabInstance = Instantiate(marker);

            // Optional, this example applies the spawner's position and rotation to the new instance
            m_PrefabInstance.transform.position = transform.position;
            m_PrefabInstance.transform.rotation = transform.rotation;

            // Get the instance's NetworkObject and Spawn
            m_SpawnedNetworkMarker = m_PrefabInstance.GetComponent<NetworkObject>();
            try{
                m_SpawnedNetworkMarker.Spawn();
            }catch (Exception e){
                 Debug.LogError($"marker tak spawned :  {e}");
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer && DestroyWithSpawner && m_SpawnedNetworkMarker != null && m_SpawnedNetworkMarker.IsSpawned)
            {
                m_SpawnedNetworkMarker.Despawn();
            }
            base.OnNetworkDespawn();
        }
    }
