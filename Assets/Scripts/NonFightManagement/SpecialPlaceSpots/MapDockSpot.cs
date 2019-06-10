using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDockSpot : MapSpecialPlaceSpot
{
    [Header("Shop Parameters")]
    SellerShopParameters sellerShopParameters;
    public SellerShopParameters GetShopParameters
    {
        get
        {
            return sellerShopParameters;
        }
    }

    public override void StartSpotInteraction(PlayerShip player)
    {
        base.StartSpotInteraction(player);

        MapManager.mapManager.DocksManager.OpenDocksPanel(cameraWhenOnSpot, this);
    }

    public void SetShopParameters(SellerShopParameters shopParameters)
    {        
        sellerShopParameters = /*new SellerShopParameters(shopParameters)*/shopParameters;
        sellerShopParameters.SetUpSelectedObjects();
    }
}