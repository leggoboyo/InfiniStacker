using System.Collections.Generic;
using UnityEngine;

namespace InfiniStacker.Visual
{
    public static class VisualTheme
    {
        private static readonly Dictionary<string, Material> MaterialCache = new();

        public static Material Road => GetLit("Road", new Color(0.2f, 0.23f, 0.28f), 0.05f, 0.62f);
        public static Material UpgradeLane => GetLit("UpgradeLane", new Color(0.18f, 0.26f, 0.34f), 0.06f, 0.68f);
        public static Material CombatLane => GetLit("CombatLane", new Color(0.26f, 0.28f, 0.31f), 0.08f, 0.58f);
        public static Material RoadShoulder => GetLit("RoadShoulder", new Color(0.14f, 0.16f, 0.2f), 0.07f, 0.55f);
        public static Material LaneMark => GetLit("LaneMark", new Color(0.96f, 0.78f, 0.32f), 0f, 0.9f, 0.35f, new Color(1f, 0.84f, 0.33f));
        public static Material RailMetal => GetLit("RailMetal", new Color(0.34f, 0.28f, 0.25f), 0.7f, 0.5f);
        public static Material Water => GetLit("Water", new Color(0.11f, 0.43f, 0.76f), 0.15f, 0.86f, 0.08f, new Color(0.18f, 0.48f, 0.8f));
        public static Material SoldierSuit => GetLit("SoldierSuit", new Color(0.2f, 0.35f, 0.86f), 0.18f, 0.74f);
        public static Material SoldierArmor => GetLit("SoldierArmor", new Color(0.38f, 0.42f, 0.45f), 0.58f, 0.42f);
        public static Material SoldierHelmet => GetLit("SoldierHelmet", new Color(0.13f, 0.15f, 0.18f), 0.68f, 0.52f);
        public static Material SoldierVisor => GetLit("SoldierVisor", new Color(0.06f, 0.32f, 0.52f), 0.15f, 0.95f, 0.32f, new Color(0.18f, 0.56f, 0.82f));
        public static Material Weapon => GetLit("Weapon", new Color(0.24f, 0.24f, 0.26f), 0.8f, 0.36f);
        public static Material ZombieBody => GetLit("ZombieBody", new Color(0.62f, 0.66f, 0.58f), 0.03f, 0.46f);
        public static Material ZombieHead => GetLit("ZombieHead", new Color(0.74f, 0.72f, 0.64f), 0.03f, 0.43f);
        public static Material ZombieCloth => GetLit("ZombieCloth", new Color(0.34f, 0.24f, 0.2f), 0.08f, 0.38f);
        public static Material ZombieMouth => GetLit("ZombieMouth", new Color(0.34f, 0.14f, 0.14f), 0f, 0.28f);
        public static Material ZombieEye => GetLit("ZombieEye", new Color(0.92f, 0.16f, 0.1f), 0f, 0.35f, 1.1f, new Color(1f, 0.22f, 0.16f));
        public static Material ZombiePants => GetLit("ZombiePants", new Color(0.17f, 0.22f, 0.28f), 0.14f, 0.45f);
        public static Material ZombieHair => GetLit("ZombieHair", new Color(0.12f, 0.1f, 0.08f), 0.38f, 0.22f);
        public static Material ZombieShoes => GetLit("ZombieShoes", new Color(0.11f, 0.1f, 0.11f), 0.52f, 0.2f);
        public static Material GateAdd => GetLit("GateAdd", new Color(0.09f, 0.66f, 0.92f), 0.04f, 0.86f, 0.35f, new Color(0.12f, 0.54f, 0.95f));
        public static Material GateMultiply => GetLit("GateMultiply", new Color(0.16f, 0.74f, 0.36f), 0.05f, 0.78f, 0.32f, new Color(0.17f, 0.8f, 0.42f));
        public static Material GateSubtract => GetLit("GateSubtract", new Color(0.93f, 0.32f, 0.27f), 0.04f, 0.78f, 0.3f, new Color(0.88f, 0.3f, 0.22f));
        public static Material GateFrame => GetLit("GateFrame", new Color(0.18f, 0.2f, 0.24f), 0.72f, 0.5f);
        public static Material GateUsed => GetLit("GateUsed", new Color(0.35f, 0.35f, 0.35f), 0.22f, 0.45f);
        public static Material Ice => GetLit("Ice", new Color(0.72f, 0.9f, 1f), 0.06f, 0.98f, 0.3f, new Color(0.55f, 0.84f, 1f));
        public static Material MechBody => GetLit("MechBody", new Color(0.64f, 0.85f, 1f), 0.25f, 0.74f, 0.24f, new Color(0.5f, 0.8f, 1f));
        public static Material MechDark => GetLit("MechDark", new Color(0.18f, 0.23f, 0.29f), 0.62f, 0.48f);
        public static Material City => GetLit("City", new Color(0.2f, 0.28f, 0.39f), 0.1f, 0.52f);
        public static Material Bullet => GetLit("Bullet", new Color(1f, 0.87f, 0.4f), 0f, 1f, 0.75f, new Color(1f, 0.76f, 0.26f));
        public static Material Hit => GetLit("Hit", new Color(1f, 0.56f, 0.2f), 0f, 0.6f, 0.85f, new Color(1f, 0.56f, 0.18f));

        public static void ApplyMaterial(Renderer renderer, Material material)
        {
            if (renderer == null || material == null)
            {
                return;
            }

            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
        }

        private static Material GetLit(
            string cacheKey,
            Color baseColor,
            float metallic,
            float smoothness,
            float emissionIntensity = 0f,
            Color? emissionColor = null)
        {
            if (MaterialCache.TryGetValue(cacheKey, out var cached) && cached != null)
            {
                return cached;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
            {
                return null;
            }

            var material = new Material(shader)
            {
                name = $"Mat_{cacheKey}"
            };

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", baseColor);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", baseColor);
            }

            if (material.HasProperty("_Metallic"))
            {
                material.SetFloat("_Metallic", Mathf.Clamp01(metallic));
            }

            if (material.HasProperty("_Smoothness"))
            {
                material.SetFloat("_Smoothness", Mathf.Clamp01(smoothness));
            }

            if (emissionIntensity > 0.0001f)
            {
                var eColor = (emissionColor ?? baseColor) * emissionIntensity;
                material.EnableKeyword("_EMISSION");

                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor("_EmissionColor", eColor);
                }
            }

            MaterialCache[cacheKey] = material;
            return material;
        }
    }
}
