using UnityEngine;
namespace Miscalculation.CharacterLobby
{
    [CreateAssetMenu(menuName = "Miscalculation/Character Lobby/Art")]
    public sealed class LobbyArt : ScriptableObject
    {
        public Texture2D lampOff, lampOn, lampOffWindowShadow, lightField, rainOcclusionMask;
        public Texture2D[] smokeAtlases = new Texture2D[4];
        [Tooltip("每帧左上原点的像素锚点；离线生成，运行时不扫描。")]
        public Vector2[] anchors = new Vector2[24];
        public string assetVersion = "lobby-window-shadow-v1-lightfield-v1-rain-occlusion-v1-smoke-v4";
        public string contentHash, sourceHash;
        public Material backgroundMaterial, screenMaterial, particleMaterial, sceneItemMaterial;
        public int FrameCount => anchors == null ? 0 : anchors.Length;
        public bool IsValid
        {
            get
            {
                if(!lampOff || !lampOn || !lampOffWindowShadow || !lightField || !rainOcclusionMask || smokeAtlases == null || smokeAtlases.Length != 4 || anchors == null || anchors.Length != 24 || !backgroundMaterial || !screenMaterial || !particleMaterial || !sceneItemMaterial)return false;
                foreach(Texture2D atlas in smokeAtlases)if(!atlas)return false;
                return true;
            }
        }
    }
}
