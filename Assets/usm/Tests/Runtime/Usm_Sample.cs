using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USM.Sample
{
    public class Usm_Sample : MonoBehaviour
    {
        public UiStateMachineBehaviour usm_tab;
        public UiStateMachineBehaviour usm_graphic;

        private int _indexGraphics = 0;

        private void Awake()
        {
            SelectTab("Graphics");
        }

        public void SelectTab(string tabName)
        {
            usm_tab.Play(tabName);

            StopAllCoroutines();
            if (tabName == "Graphics")
            {
                StartCoroutine(ChangeGraphics_Periodically());
            }
        }

        IEnumerator ChangeGraphics_Periodically()
        {
            var graphicStates = usm_graphic.usm.states;
            Debug.Assert(graphicStates.Count > 0);

            while (true)
            {
                var state = graphicStates[_indexGraphics];
                usm_graphic.Play(state);
                yield return new WaitForSeconds(2.0f);

                _indexGraphics += 1;
                if (_indexGraphics >= graphicStates.Count)
                    _indexGraphics = 0;
            }
        }
    }
}
