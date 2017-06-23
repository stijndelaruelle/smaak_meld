using System.Collections.Generic;
using System.Linq;
using Mapbox.VectorTile;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;
using Mapbox.MeshGeneration.Components;

//Custom modifier written by Stijn. When updating MapBox please don't forget about this script!
//Makes sure every road has a road script attached
namespace Mapbox.MeshGeneration.Modifiers
{
    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Add Road Script Modifier")]
    public class AddRoadScriptModifier : GameObjectModifier
    {
        public override void Run(FeatureBehaviour fb)
        {
            Road road = fb.GameObject.AddComponent<Road>();
        }
    }
}
