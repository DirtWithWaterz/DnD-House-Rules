using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject PlayerObj;
    [SerializeField] private Vector3 SpawnPoint;

    [SerializeField] Interpreter interpreter;

    [Rpc(SendTo.Server)]
    public void OnIHateMyselfSpawnRpc()
    {
        if (!IsServer) return;
        
        NetworkObject player = Instantiate(PlayerObj, SpawnPoint, Quaternion.identity);
        SpawnPoint+=Vector3.right*20;
        player.SpawnAsPlayerObject(NetworkManager.LocalClientId);
        GameManager.Singleton.AddPlayerDataRpc(player, interpreter.GetUsername, NetworkManager.LocalClientId);
        
        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
        
    }
    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null && IsServer && SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Game"))
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
    private void SpawnPlayer(ulong clientId)
    {
        NetworkObject player = Instantiate(PlayerObj, SpawnPoint, Quaternion.identity);
        SpawnPoint+=Vector3.right*20;
        player.SpawnAsPlayerObject(clientId);
        UpdateGameManagerPlayerDataRpc(player, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }
    [Rpc(SendTo.SpecifiedInParams)]
    public void UpdateGameManagerPlayerDataRpc(NetworkObjectReference reference, RpcParams rpcParams){
        GameManager.Singleton.AddPlayerDataRpc(reference, interpreter.GetUsername, NetworkManager.LocalClientId);
    }
}