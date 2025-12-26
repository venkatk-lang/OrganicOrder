using UnityEngine;
namespace IACGGames
{
    [CreateAssetMenu(fileName = "ParticleSystemData", menuName = "IACGGameSDK/ParticleSystemData", order = 1)]
    public class ParticleSystemSO : ScriptableObject
    {
        [Header("ParticleSystem FXs Data")]
        public ParticleSystem[] FX;

        public void PlayFXs(int FxsId, Vector3 pos, Vector3 posOffSet, Vector3 size, Vector3 rot, Transform parent)
        {
            ParticleSystem p = PlayFXs(FxsId, pos, posOffSet, size, rot);
            p.transform.parent = parent;
        }
        public ParticleSystem PlayFXs(int FxsId, Vector3 pos, Vector3 posOffSet, Vector3 size, Vector3 rot)
        {
            ParticleSystem p = Instantiate(FX[FxsId], (pos + posOffSet), Quaternion.Euler(rot));
            p.transform.localScale = size;
            // p.transform.Rotate(rot, Space.Self);
            p.Play();
            return p;
        }
    }
}