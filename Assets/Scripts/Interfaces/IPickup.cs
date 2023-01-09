using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickup
{
    public bool Pickupable{
        get;
    }

    public Pickup_Type PickupType{
        get;
    }

    public void Activate(bool specialBehavior = true);

    public bool Pickup();

}

public enum Pickup_Type{
    Heart,
    Coin,
    Potion
}
