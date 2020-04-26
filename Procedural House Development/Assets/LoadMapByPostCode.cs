using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Geocoding;
using TMPro;

public class LoadMapByPostCode : MonoBehaviour
{
    private ForwardGeocodeResource m_resource;
    public int m_totalTiles;

    public void GenerateMap(TMP_InputField location)
    {
        ForwardGeocodeResource resource = new ForwardGeocodeResource(location.text);
        Mapbox.Unity.MapboxAccess.Instance.Geocoder.Geocode(resource, LoadImage);
    }

    public void LoadImage(ForwardGeocodeResponse data)
    {
        var abstractMap = GetComponent<Mapbox.Unity.Map.AbstractMap>();

        var ext = abstractMap.Options.extentOptions.defaultExtents.rangeAroundCenterOptions;
        m_totalTiles = (ext.east + ext.west) * (ext.north + ext.south);
        abstractMap.OnTileFinished += FadeOverlays;
        
        abstractMap.Initialize(data.Features[0].Center, 18);
    }

    public void FadeOverlays(Mapbox.Unity.MeshGeneration.Data.UnityTile tile)
    {
        if(--m_totalTiles == 0)
            FindObjectOfType<FadeMaterial>().m_target = 0;
    }
}
