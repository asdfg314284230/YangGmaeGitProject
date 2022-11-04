using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInterface
{

}

public enum CustomBtnType
{
    None,
    Btn_A_1,
    Btn_A_2,
    Btn_B_1,
    Btn_B_2,
    Btn_C_1,
    Btn_C_2,
    Btn_D_1,
    Btn_D_2,
    Btn_E_1
}

public enum CustomTabType
{
    None,
    Tab_A,
    Tab_B,
    Tab_C,
    Tab_D
}

public enum EnterTriggerState
{
    None,
    Hide,
    Normal,
    Disable
}

public interface IEnterTrigger
{
    void SetEnterState(EnterTriggerState state, System.Action clickFailed);
}

public interface IResLoader
{
    T Load<T>(string asset);
}
