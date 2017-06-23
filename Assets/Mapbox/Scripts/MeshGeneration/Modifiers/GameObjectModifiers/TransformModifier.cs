using System.Collections.Generic;
using System.Linq;
using Mapbox.VectorTile;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEngine;
using Mapbox.MeshGeneration.Components;

//Custom modifier written by Stijn. When updating MapBox please don't forget about this script!
namespace Mapbox.MeshGeneration.Modifiers
{
    [CreateAssetMenu(menuName = "Mapbox/Modifiers/Transform Modifier")]
    public class TransformModifier : GameObjectModifier
    {
        [SerializeField]
        private Vector3 _positionOffset;

        [SerializeField]
        private Vector3 _rotationOffset;

        [SerializeField]
        private Vector3 _scaleOffset;

        public override void Run(FeatureBehaviour fb)
        {
            Transform goTransform = fb.GameObject.transform;

            goTransform.localPosition = goTransform.localPosition + _positionOffset;
            goTransform.localRotation = Quaternion.Euler(goTransform.localRotation.eulerAngles + _rotationOffset);
            goTransform.localScale = goTransform.localScale + _scaleOffset;
        }
    }
}
