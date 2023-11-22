using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class Authenticator
{
    public Authenticator()
    {
        Initializer();
    }
    public async void Initializer()
    {
        if (UnityServices.State.Equals(ServicesInitializationState.Uninitialized))
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError("Erro ao inicializar o UnityServices");
                Debug.LogError(ex);
            }
        }
    }

    public async void SignIn(Action callback = null)
    {
        #if UNITY_EDITOR
            if (ParrelSync.ClonesManager.IsClone())
            {
                string customArgument = ParrelSync.ClonesManager.GetArgument();
                AuthenticationService.Instance.SwitchProfile($"Clone_{customArgument}_Profile");
            }
        #else
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                //if (clearCache)
                    //AuthenticationService.Instance.ClearSessionToken();
                    AuthenticationService.Instance.SwitchProfile($"Clone_Profile");
                //else
                    //return;
            }
        #endif
        if (callback != null)
        {
            AuthenticationService.Instance.SignedIn += callback;
        }
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public static async Task<Allocation> AllocateServer(int maxPlayers, string region = "southamerica-east1")
    {
        Allocation alloc;
        try
        {
            alloc = await RelayService.Instance.CreateAllocationAsync(maxPlayers, region);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ocorreu um erro durante a alocação do serviço. Error {ex.Message} ");
            throw ex;
        }
        Debug.Log($"Allocation ID {alloc.AllocationId}");
        return alloc;
    }

    public static async Task<string> GetJoinCode(Allocation alloc)
    {
        string code = string.Empty;
        try
        {
            code = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ocorreu um erro durante a criação de um código para a sala. Error {ex.Message} ");
            return null;
        }
        return code;
    }

    public static async Task<JoinAllocation> JoinToRelayServer(string joinCode)
    {
        JoinAllocation alloc;
        try
        {
            alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ocorreu um erro durante a tentativa de se concectar com o host. Error {ex.Message} ");
            throw;
        }
        return alloc;
    }
}
