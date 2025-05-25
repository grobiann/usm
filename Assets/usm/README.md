[![README_EN](https://img.shields.io/badge/Usm-English-red)](https://github.com/grobiann/usm/blob/master/README_EN.md)

# usm
Ui State Machine

## Table of Contents
- [소개](https://github.com/grobiann/usm?tab=readme-ov-file#소개)
- [사용법](https://github.com/grobiann/usm?tab=readme-ov-file#사용법)
- [예시](https://github.com/grobiann/usm?tab=readme-ov-file#예시)
- [MonoBehaviour](https://github.com/grobiann/usm?tab=readme-ov-file#MonoBehaviour)
- [License](https://github.com/grobiann/usm?tab=readme-ov-file#License)
- [UPM Package](https://github.com/grobiann/usm?tab=readme-ov-file#UPM-Package)
  - [Install via git URL](https://github.com/grobiann/usm?tab=readme-ov-file#Install-via-git-url)

## 소개
`UI State Machine`은 복잡한 UI 상태를 쉽게 관리할 수 있도록 설계되었습니다. 다양한 UI 상태를 정의하고 전환할 수 있는 명확한 구조를 제공하여, 코드를 더 구조화하고 유지보수하기 쉽게 만듭니다.

![Example Image](./images/upm_screenshot_1.png)

## 사용법
`UI State Machine`을 프로젝트에서 사용하려면 다음 단계를 따르세요.

1. `UI State Machine` 패키지를 프로젝트에 가져옵니다.
2. `UiStateMachineBehaviour` 클래스를 GameObject에 추가합니다.
3. `UiStateMachineBehaviour` 클래스의 인스펙터에서 'Open USM Window' 버튼을 클릭하여 USM 창을 엽니다.
4. USM 창에서 각 GameObject의 상태를 생성하고 활성 상태를 설정합니다.
5. 초록색 'Test' 버튼을 클릭하여 각 상태를 미리 확인해 볼 수 있습니다.
6. 스크립트를 통해 또는 Unity 이벤트를 사용하여 USM의 상태를 변경합니다.

## 예시
이 예제는 패키지에 포함된 `USM Sample`에서 확인할 수 있습니다.

![Example Image](./images/upm_screenshot_3.png)

다음은 `UI State Machine`을 사용하는 기본적인 예제입니다.

```csharp
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
```

## MonoBehaviour
usm의 `UiStateMachineBehaviour`은 `MonoBehaviour`를 상속받았기 때문에, 게임오브젝트 혹은 프리팹과 함께 저장할 수 있습니다.
따라서 데이터를 복사하거나, 수정하고, 되돌리는데에 용이합니다.

프리팹 내에서 usm을 사용하는경우, 외부에서 override하지 않도록 주의하십시오. `usm window`는 프리팹모드에서의 수정을 지원합니다. 프리팹모드가 아닌 상태에서 프리팹을 수정하려 하면, 자동으로 프리팹모드로 이동됩니다.


<img src="./images/upm_screenshot_2.png" alt="Example Image" width="300px">

## UPM Package
### Install via git URL
Requires a version of unity that supports path query parameter for git packages (Unity >= 2019.3.4f1, Unity >= 2020.1a21).
You can add `https://github.com/grobiann/usm.git?path=Assets/usm` to Package Manager

![Example Image](./images/upm_install_guide_1.png)
![Example Image](./images/upm_install_guide_2.png)

or add "com.jjs.usm": `"https://github.com/grobiann/usm.git?path=Assets/usm"` to Packages/manifest.json.

If you want to set a target version, UniTask uses the `*.*.*` release tag so you can specify a version like `#0.0.4`. For example `https://github.com/grobiann/usm.git?path=Assets/usm#0.0.4`.

## License
This library is under the MIT License.
