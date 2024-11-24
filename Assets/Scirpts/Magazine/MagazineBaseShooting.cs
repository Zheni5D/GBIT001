using UnityEngine;

public class MagazineBaseShooting : BaseBehavior
{
    public static event System.Action<int> OnAmmoChangedStatic;
    [SerializeField]
    protected int Ammo;
    public void AmmoAmountChanged(int _amount)
    {
        Ammo += _amount;
        OnAmmoChangedStatic?.Invoke(Ammo);
    }
}
