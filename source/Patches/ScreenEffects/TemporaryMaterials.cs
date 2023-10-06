using Reactor.Utilities;
using System.Collections;
using UnityEngine;

namespace TownOfUs.Patches.ScreenEffects
{
    public abstract class TemporaryMaterial
    {
        public abstract Material mat { get; }
        public abstract float duration { get; }
        protected private Material original_material;
        private GameObject target { get; }
        private Renderer rtarget { get; }
        protected TemporaryMaterial(GameObject target)
        {
            this.target = target;
            Coroutines.Start(apply_Material());
        }
        protected TemporaryMaterial(Renderer target)
        {
            this.rtarget = target;
            Coroutines.Start(rapply_Material());
        }

        protected IEnumerator apply_Material()
        {
            original_material = target.GetComponent<Renderer>().material;
            target.GetComponent<Renderer>().material = mat;
            yield return new WaitForSecondsRealtime(duration);
            target.GetComponent<Renderer>().material = original_material;
        }

        protected IEnumerator rapply_Material()
        {
            original_material = rtarget.material;
            var sharedmat = rtarget.sharedMaterial;

            rtarget.material = mat;
            rtarget.sharedMaterial = mat;
            yield return new WaitForSecondsRealtime(duration);
            rtarget.material = original_material;
            rtarget.sharedMaterial = sharedmat;
        }

    }
}