using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Usm.Sample
{
    public class UsmSampleSettingWindow : MonoBehaviour
    {
        public UIStateMachineBehaviour _usmTab;
        public UIStateMachineBehaviour _usmGraphicTabContent;

        private int _graphicTabContentIndex = 0;

        private const string TAB_USM_NAME_GRAPHICS = "Graphics";

        private void Awake()
        {
            SelectTab(TAB_USM_NAME_GRAPHICS);
        }

        public void SelectTab(string tabName)
        {
            _usmTab.SetState(tabName);

            StopAllCoroutines();
            if (tabName == TAB_USM_NAME_GRAPHICS)
            {
                StartCoroutine(ChangeGraphicContentsPeriodically());
            }
        }

        private IEnumerator ChangeGraphicContentsPeriodically()
        {
            var graphicStates = _usmGraphicTabContent.Usm.States;
            Debug.Assert(graphicStates.Count > 0);

            float contentChangeInterval = 2.0f;
            while (true)
            {
                var state = graphicStates[_graphicTabContentIndex];
                _usmGraphicTabContent.SetState(state);

                yield return new WaitForSeconds(contentChangeInterval);

                _graphicTabContentIndex += 1;
                if (_graphicTabContentIndex >= graphicStates.Count)
                    _graphicTabContentIndex = 0;
            }
        }
    }
}
