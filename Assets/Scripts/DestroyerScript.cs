using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class DestroyerScript : MonoBehaviour
    {
        public void CallDestroyMethod(float destroyTimeDelay)
        {
            Invoke(nameof(DestroyFinally), destroyTimeDelay);
        }

        private void DestroyFinally() => Destroy(this.gameObject);

    }
}
