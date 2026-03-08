using System;
using System.Collections.Generic;
using Fusion;
using INFEST.Game;
using UnityEngine;

public class StoreController : NetworkBehaviour
{
    public UIShopView uIShopView;
    public List<Store> aiiStores;

    public void Deactivate()
    {
        if (HasStateAuthority)
        {
            RPC_Hide();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_Hide(/*int index*/)
    {
        for (int i = 0; i < aiiStores.Count; i++)
        {
            aiiStores[i].activatelighting.SetActive(false);
            aiiStores[i].col.enabled = false;
        }

        if (!NetworkGameManager.Instance.gamePlayers.GetPlayerObj(NetworkGameManager.Instance.Runner.LocalPlayer).inStoreZoon) return;
        NetworkGameManager.Instance.gamePlayers.GetPlayerObj(NetworkGameManager.Instance.Runner.LocalPlayer).inStoreZoon = false;
        Global.Instance.UIManager.Hide<UIInteractiveView>();
        Global.Instance.UIManager.Hide<UIShopView>();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_Show(/*int index*/)
    {
        for (int i = 0; i < aiiStores.Count; i++)
        {
            aiiStores[i].activatelighting.SetActive(true);
            aiiStores[i].col.enabled = true;
        }
    }
}
