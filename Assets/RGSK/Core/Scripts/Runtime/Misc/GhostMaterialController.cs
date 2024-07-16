using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RGSK
{
    public class GhostMaterialController : MonoBehaviour
    {
        Dictionary<Renderer, Material[]> _defaultMaterials = new Dictionary<Renderer, Material[]>();
        Dictionary<Renderer, Material[]> _ghostMaterials = new Dictionary<Renderer, Material[]>();
        MeshColorizer[] _meshColorizers;

        void Initialize()
        {
            var material = RGSKCore.Instance.VehicleSettings.ghostMaterial;

            if (material == null)
                return;

            var renderers = GetComponentsInChildren<Renderer>().Where(x =>
            !x.GetComponent<ParticleSystem>() && !x.GetComponent<TMPro.TMP_Text>()).ToList();

            _meshColorizers = GetComponentsInChildren<MeshColorizer>();

            if (renderers.Count == 0)
                return;

            foreach (var r in renderers)
            {
                if (!_defaultMaterials.ContainsKey(r))
                {
                    _defaultMaterials.Add(r, r.materials);
                }

                var materials = r.materials;

                for (int i = 0; i < materials.Length; i++)
                {
                    var m = new Material(material);

                    if (materials[i].HasProperty("_Color"))
                    {
                        m.color = new Color(
                            materials[i].color.r,
                            materials[i].color.g,
                            materials[i].color.b,
                            materials[i].color.a);
                    }

                    if (materials[i].HasProperty("_MainTex"))
                    {
                        m.mainTexture = materials[i].mainTexture;
                    }

                    materials[i] = m;
                }

                if (!_ghostMaterials.ContainsKey(r))
                {
                    _ghostMaterials.Add(r, materials);
                }
            }
        }

        public void ToggleGhostMaterials(bool toggle)
        {
            Initialize();

            foreach (var r in !toggle ? _defaultMaterials : _ghostMaterials)
            {
                r.Key.materials = r.Value;
            }
        }
    }
}