using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Geocoding;
using TMPro;

public class LoadMapByPostCode : MonoBehaviour
{
    private ForwardGeocodeResource m_resource;
    public GameObject m_loading;
    public int m_totalTiles;

    // Called via text input complete
    public void GenerateMap(TMP_InputField location)
    {
        ForwardGeocodeResource resource = new ForwardGeocodeResource(location.text);
        Mapbox.Unity.MapboxAccess.Instance.Geocoder.Geocode(resource, LoadImage);
        m_loading.SetActive(true);
    }

    // Loads the map, called via coroutine
    public void LoadImage(ForwardGeocodeResponse data)
    {
        var abstractMap = GetComponent<Mapbox.Unity.Map.AbstractMap>();

        var ext = abstractMap.Options.extentOptions.defaultExtents.rangeAroundCenterOptions;
        m_totalTiles = (ext.east + ext.west) * (ext.north + ext.south);
        abstractMap.OnTileFinished += FadeOverlays;
        
        abstractMap.Initialize(data.Features[0].Center, 18);
    }

    // Fades the grid-map
    public void FadeOverlays(Mapbox.Unity.MeshGeneration.Data.UnityTile tile)
    {
        if (--m_totalTiles == 0)
        {
            m_loading.SetActive(false);
            FindObjectOfType<FadeMaterial>().m_target = 0;
        }
    }
}
