# usm
Ui State Machine

## Table of Contents
- [Introduction](#introduction)
- [Usage](#usage)
- [Example](#example)
- [Monobehaviour](https://github.com/grobiann/usm?tab=readme_en-ov-file##Monobehaviour)
- [License](https://github.com/grobiann/usm?tab=readme_en-ov-file#License)
- [UPM Package](https://github.com/grobiann/usm?tab=readme_en-ov-file#UPM-Package)
  - [Install via git URL](https://github.com/grobiann/usm?tab=readme_en-ov-file#Install-via-git-url)

## Introduction
The `UI State Machine` is designed to help developers manage complex UI states with ease. It provides a clear structure for defining and transitioning between various UI states, making your code more organized and maintainable.

![Example Image](./images/upm_screenshot_1.png)

## Usage
To use the UI State Machine in your project, follow these steps:

1. Import the UI State Machine package into your project.
2. Add the `UiStateMachineBehaviour` class to your GameObject.
3. Open the USM Window by clicking the 'Open USM Window' button in the inspector of the `UiStateMachineBehaviour` class.
4. Create states and set the active state of the gameObjects in the USM Window.
5. Click the green 'Test' button to easily preview each state.
6. Change the state of the USM either through scripts or using Unity events.

## Example
You can see this example in the `USM Sample` included in the package.

![Example Image](./images/upm_screenshot_3.png)

Here's a basic example of how to set up and use the `UI State Machine`:

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

## Monobehaviour
Since usm's `UiStateMachineBehaviour` inherits from MonoBehaviour, it can be saved along with GameObjects or prefabs. This makes it easy to copy, modify, and revert data.

When using usm within a prefab, be careful not to override externally. The usm window supports modifications in prefab mode. Attempting to modify a prefab outside of prefab mode will automatically switch to prefab mode.


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
