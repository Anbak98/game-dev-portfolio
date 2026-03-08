using System.Collections.Generic;
using Fusion;
using INFEST.Game;
using UnityEngine;

public class Store : NetworkBehaviour 
{
    public StoreController _storeController;
    public List<int> idList;
    public GameObject activatelighting;
    public SphereCollider col;


    #region  상점 콜라이더 트리거 메소드

    #region 상호작용시
    /// <summary>
    /// 상호작용시 요청하는 메소드
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_RequestInteraction(PlayerRef _playerRef)
    {
        RPC_Interaction(_playerRef);
        //if (_storeController.activeTime)
        //{
        //    _storeController.AddTimer();
        //}
    }

    /// <summary>
    /// 상호작용 로직
    /// </summary>
    /// 
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_Interaction([RpcTarget] PlayerRef _playerRef)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Global.Instance.UIManager.Hide<UIInteractiveView>();
        _storeController.uIShopView = Global.Instance.UIManager.Show<UIShopView>();
        NetworkGameManager.Instance.inputManager.ShopSetActive(false);
        _storeController.uIShopView.StoreInIt(this);
        _storeController.uIShopView.UpdateButtonState();
    }
    #endregion

    #region 상호작용해제시
    /// <summary>
    /// 상호작용해제시 요청하는 메소드
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_RequestStopInteraction(PlayerRef _playerRef)
    {
        RPC_StopInteraction(_playerRef);
    }
    /// <summary>
    /// 상호작용 해제로직
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_StopInteraction([RpcTarget] PlayerRef _playerRef)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Global.Instance.UIManager.Show<UIInteractiveView>();
        Global.Instance.UIManager.Hide<UIShopView>();
        NetworkGameManager.Instance.inputManager.ShopSetActive(true);
    }
    #endregion

    #region OnTriggerEnter 호출
    /// <summary>
    /// 상점 영역 로직
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_EnterShopZone(Player _player, [RpcTarget] PlayerRef _playerRef)
    {
        Global.Instance.UIManager.Show<UIInteractiveView>();
    }
    #endregion

    #region OnTriggerExit 호출
    /// <summary>
    /// 상점을 떠났을때
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_LeaveShopZone(Player _player, [RpcTarget] PlayerRef _playerRef)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _player.isInteraction = false;
        Global.Instance.UIManager.Hide<UIInteractiveView>();
        Global.Instance.UIManager.Hide<UIShopView>();
    }
    #endregion

    #endregion

    #region 상점 구매 & 판매 메소드

    #region 구매
    /// <summary>
    /// 구매 메소드
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    /// <param name="index"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TryBuy(Player _player, int index)
    {
        if (idList[index] == 0) return;

        if (_player == null) return;


        if (idList[index] % 10000 < 700) // 무기
        {
            Weapon _buyWeapon = null;

            for (int i = 0; i < _player.Weapons.Weapons.Count; i++)
            {
                if (_player.Weapons.Weapons[i].key == idList[index])
                {
                    _buyWeapon = _player.Weapons.Weapons[i];
                    break;
                }
            }
            AnalyticsManager.analyticsPurchase(_buyWeapon.instance.data.key);
            _player.statHandler.CurGold -= _buyWeapon.instance.data.Price;
            _player.inventory.AddWeponItme(_buyWeapon);
            _buyWeapon.IsCollected = true;
            _player.Weapons._weapons.Add(_buyWeapon);
        }
        else if (idList[index] % 10000 < 1000) // 아이템
        {
            Consume _buyConsume = null;

            for (int i = 0; i < _player.Consumes.Consumes.Count; i++)
            {
                if (_player.Consumes.Consumes[i].key == idList[index])
                {
                    _buyConsume = _player.Consumes.Consumes[i];
                    break;
                }
            }

            _player.statHandler.CurGold -= _buyConsume.instance.data.Price;
            _player.inventory.AddConsumeItme(_buyConsume);
        }
        var inv = _player.inventory;
        int[] invKey = {inv.auxiliaryWeapon[0] != null? inv.auxiliaryWeapon[0].instance.data.key : 0,
                        inv.weapon[0] != null? inv.weapon[0].instance.data.key : 0,
                        inv.weapon[1] != null? inv.weapon[1].instance.data.key : 0,
                        inv.consume[0] != null? inv.consume[0].instance.data.key : 0,
                        inv.consume[1] != null? inv.consume[1].instance.data.key : 0,
                        inv.consume[2] != null? inv.consume[2].instance.data.key : 0};

        for (int i = 0; i < invKey.Length; i++)
        {
            if (invKey[i] == idList[index])
            {
                _storeController.uIShopView.SaleSet(i);
                if (i < 3)
                    _storeController.uIShopView.WeaponSet(i);
                else
                    _storeController.uIShopView.ItemSet(i - 3);
            }
        }

        _storeController.uIShopView.UpdateButtonState();
        _storeController.uIShopView.UpdateSaleButtonState();
    }


    #endregion

    #region 판매
    /// <summary>
    /// 판매 메소드
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_playerRef"></param>
    /// <param name="index"></param>
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TrySale(PlayerRef _playerRef, int index)
    {
        Player _player = NetworkGameManager.Instance.gamePlayers.GetPlayerObj(_playerRef);

        if (_player == null) return;


        switch (index)
        {
            case 0: // 보조무기
                if (_player.inventory.auxiliaryWeapon[0] == null) return;
                _player.statHandler.CurGold += _player.inventory.auxiliaryWeapon[0].instance.data.Price / 2;
                AnalyticsManager.analyticsPurchase(_player.inventory.auxiliaryWeapon[0].instance.data.key);
                if (_player.inventory.equippedWeapon == _player.inventory.auxiliaryWeapon[0])
                    _player.Weapons.Swap(-1, true);
                else
                {
                    _player.inventory.auxiliaryWeapon[0].curBullet = _player.inventory.auxiliaryWeapon[0].instance.data.MaxBullet;
                    _player.inventory.auxiliaryWeapon[0].FPSWeapon.activeAmmo = _player.inventory.auxiliaryWeapon[0].instance.data.MagazineBullet;
                    _player.inventory.auxiliaryWeapon[0].curMagazineBullet = _player.inventory.auxiliaryWeapon[0].instance.data.MagazineBullet;
                    _player.inventory.auxiliaryWeapon[0].IsCollected = false;
                    _player.Weapons.ExpectedRemove(_player.inventory.auxiliaryWeapon[0]);
                }
                _player.inventory.RemoveWeaponItem(_player.inventory.auxiliaryWeapon[0], 0);

                _storeController.uIShopView.WeaponSet(0);
                break;
            case 1: // 주무기 1
                if (_player.inventory.weapon[0] == null) return;
                _player.statHandler.CurGold += _player.inventory.weapon[0].instance.data.Price / 2;
                AnalyticsManager.analyticsPurchase(_player.inventory.weapon[0].instance.data.key);
                if (_player.inventory.equippedWeapon == _player.inventory.weapon[0])
                    _player.Weapons.Swap(-1, true);
                else
                {
                    _player.inventory.weapon[0].curBullet = _player.inventory.weapon[0].instance.data.MaxBullet;
                    _player.inventory.weapon[0].FPSWeapon.activeAmmo = _player.inventory.weapon[0].instance.data.MagazineBullet;
                    _player.inventory.weapon[0].curMagazineBullet = _player.inventory.weapon[0].instance.data.MagazineBullet;
                    _player.inventory.weapon[0].IsCollected = false;
                    _player.Weapons.ExpectedRemove(_player.inventory.weapon[0]);

                }
                _player.inventory.RemoveWeaponItem(_player.inventory.weapon[0], 0);

                _storeController.uIShopView.WeaponSet(1);
                break;
            case 2: // 주무기 2
                if (_player.inventory.weapon[1] == null) return;
                _player.statHandler.CurGold += _player.inventory.weapon[1].instance.data.Price / 2;
                AnalyticsManager.analyticsPurchase(_player.inventory.weapon[1].instance.data.key);
                if (_player.inventory.equippedWeapon == _player.inventory.weapon[1])
                    _player.Weapons.Swap(-1, true);
                else
                {
                    _player.inventory.weapon[1].curBullet = _player.inventory.weapon[1].instance.data.MaxBullet;
                    _player.inventory.weapon[1].FPSWeapon.activeAmmo = _player.inventory.weapon[1].instance.data.MagazineBullet;
                    _player.inventory.weapon[1].curMagazineBullet = _player.inventory.weapon[1].instance.data.MagazineBullet;
                    _player.inventory.weapon[1].IsCollected = false;
                    _player.Weapons.ExpectedRemove(_player.inventory.weapon[1]);
                }
                _player.inventory.RemoveWeaponItem(_player.inventory.weapon[1], 1);

                _storeController.uIShopView.WeaponSet(2);
                break;
            case 3: // 아이템 1
                if (_player.inventory.consume[0] == null) return;
                _player.statHandler.CurGold += _player.inventory.consume[0].instance.data.Price / 2;
                AnalyticsManager.analyticsPurchase(_player.inventory.consume[0].instance.data.key);
                _player.inventory.RemoveConsumeItem(0);
                _storeController.uIShopView.ItemSet(0);
                break;
            case 4: // 아이템 2
                if (_player.inventory.consume[1] == null) return;
                _player.statHandler.CurGold += _player.inventory.consume[1].instance.data.Price / 2;
                AnalyticsManager.analyticsPurchase(_player.inventory.consume[1].instance.data.key);
                _player.inventory.RemoveConsumeItem(1);
                _storeController.uIShopView.ItemSet(1);
                break;
            case 5: // 아이템 3
                if (_player.inventory.consume[2] == null) return;
                _player.statHandler.CurGold += _player.inventory.consume[2].instance.data.Price / 2;
                AnalyticsManager.analyticsPurchase(_player.inventory.consume[2].instance.data.key);
                _player.inventory.RemoveConsumeItem(2);
                _storeController.uIShopView.ItemSet(2);
                break;
        }
        _storeController.uIShopView.UpdateButtonState();
        _storeController.uIShopView.UpdateSaleButtonState();

    }
    #endregion
    #endregion

    #region 보충 메소드

    #region 전부 보충
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TryAllSupplement(Player _player, PlayerRef _playerRef)
    {
        Weapon[] weaponInv = { _player.inventory.auxiliaryWeapon[0], _player.inventory.weapon[0], _player.inventory.weapon[1] };
        Consume[] itemInv = { _player.inventory.consume[0], _player.inventory.consume[1], _player.inventory.consume[2] };

        int weaponPrice = 0;
        int itemPrice = 0;
        int totalprice = 0;

        #region 무기
        if (weaponInv[0] != null)
        {
            weaponPrice += (weaponInv[0].instance.data.MaxBullet - weaponInv[0].curBullet) * weaponInv[0].instance.data.BulletPrice;
        }

        if (weaponInv[1] != null)
        {
            weaponPrice += (weaponInv[1].instance.data.MaxBullet - weaponInv[1].curBullet) * weaponInv[1].instance.data.BulletPrice;
        }

        if (weaponInv[2] != null)
        {
            weaponPrice += (weaponInv[2].instance.data.MaxBullet - weaponInv[2].curBullet) * weaponInv[2].instance.data.BulletPrice;
        }
        #endregion
        #region 아이템
        if (itemInv[0] != null)
        {
            itemPrice += (itemInv[0].instance.data.MaxNum - itemInv[0].curNum) * itemInv[0].instance.data.Price;
        }

        if (itemInv[1] != null)
        {
            itemPrice += (itemInv[1].instance.data.MaxNum - itemInv[1].curNum) * itemInv[1].instance.data.Price;
        }

        if (itemInv[2] != null)
        {
            itemPrice += (itemInv[2].instance.data.MaxNum - itemInv[2].curNum) * itemInv[2].instance.data.Price;
        }
        #endregion

        if (_player.statHandler.CurDefGear >= 200)
        {
            totalprice += weaponPrice + itemPrice;
        }
        else
        {
            totalprice += 500 + weaponPrice + itemPrice;
        }

        if (totalprice > _player.statHandler.CurGold) return;

        for (int i = 0; i < 3; i++)
        {
            if (weaponInv[i] != null)
            {
                while (weaponInv[i].instance.data.MaxBullet > weaponInv[i].curBullet)
                {
                    weaponInv[i].SupplementBullet();
                }
                _storeController.uIShopView.WeaponSet(i);
            }

            if (itemInv[i] != null)
            {
                while (itemInv[i].instance.data.MaxNum > itemInv[i].curNum)
                {
                    itemInv[i].AddNum();
                }
                _storeController.uIShopView.ItemSet(i);
            }

        }

        _player.statHandler.CurGold -= totalprice;
        _player.statHandler.CurDefGear += 200;
        _player.statHandler.CurDefGear = Mathf.Min(_player.statHandler.CurDefGear, 200);
        _storeController.uIShopView.UpdateButtonState();
    }
    #endregion

    #region 방어구 보충
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TryDefSupplement(Player _player)
    {
        if (_player.statHandler.CurDefGear >= 200) return;
        if (_player.statHandler.CurGold < 500) return;


        _player.statHandler.CurGold -= 500;
        _player.statHandler.CurDefGear += 200;
        _player.statHandler.CurDefGear = Mathf.Min(_player.statHandler.CurDefGear, 200);
        _storeController.uIShopView.UpdateButtonState();
    }

    #endregion

    #region 탄약 보충
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TryBulletSupplement(Player _player, int index)
    {
        Weapon[] weaponInv = { _player.inventory.auxiliaryWeapon[0], _player.inventory.weapon[0], _player.inventory.weapon[1] };

        if (weaponInv[index] == null) return;
        if (weaponInv[index].curBullet >= weaponInv[index].instance.data.MaxBullet) return;

        int differenceBullet = 0;
        if (weaponInv[index].curBullet + weaponInv[index].instance.data.MagazineBullet >= weaponInv[index].instance.data.MaxBullet)
            differenceBullet = weaponInv[index].curBullet % weaponInv[index].instance.data.MagazineBullet;

        if (_player.statHandler.CurGold < (weaponInv[index].instance.data.MagazineBullet - differenceBullet) * weaponInv[index].instance.data.BulletPrice) return;

        if (weaponInv[index].curBullet + weaponInv[index].instance.data.MagazineBullet >= weaponInv[index].instance.data.MaxBullet)
            _player.statHandler.CurGold -= weaponInv[index].instance.data.BulletPrice * (weaponInv[index].instance.data.MaxBullet - weaponInv[index].curBullet);
        else
            _player.statHandler.CurGold -= weaponInv[index].instance.data.BulletPrice * (weaponInv[index].instance.data.MagazineBullet);

        weaponInv[index].SupplementBullet();
        _storeController.uIShopView.WeaponSet(index);
        _storeController.uIShopView.UpdateButtonState();
    }
    #endregion

    #region 아이템 보충
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TryItmeSupplement(Player _player, int index)
    {
        Consume[] itemInv = { _player.inventory.consume[0], _player.inventory.consume[1], _player.inventory.consume[2] };

        if (itemInv[index] == null) return;
        if (itemInv[index].curNum >= itemInv[index].instance.data.MaxNum) return;
        if (_player.statHandler.CurGold < itemInv[index].instance.data.Price) return;

        _player.statHandler.CurGold -= itemInv[index].instance.data.Price;
        itemInv[index].AddNum();
        _storeController.uIShopView.ItemSet(index);
        _storeController.uIShopView.UpdateButtonState();
    }
    #endregion

    #endregion










}
