# YuoTools
# 日常用的和总结的一些东西
# 1-简单的ECS框架
## System继承YuoSystem<T>,并且实现Run方法,然后System继承对应的接口,比如IAwake会在创建对应组件时调用,IDestroy会在销毁时调用,IUiCreate会在UI窗口被创建时调用
## Component继承YuoComponent,组件可以添加YuoSave特性进行自动保存数据,具体实现见SaveManager
## UI通过检查器右键YuoUi/添加UI创建,默认带了dotween用作窗口动画,点击UI上的UISetting的生成UI代码,会自动创建对应的组件和系统脚本,当前UI中前缀为C_的为自动生成代码的检测物体
## 扩展方法:
###  	Log:可以在任何类型后面.Log直接输出
### 	Mathf:
		Clamp : 限定范围 传两个参数为设置最小值和最大值, 一个参数为设置最大值, 最小值为0, 无参设置为不小于0

        InRange: 判断一个数是否在范围内
### 	Transform:
        ResetTrans:设置为默认值,即Inspector面板上的Reset
        SetposX、Y、Z:单独设置xyz三个坐标中的一个
        SetLocalPos:同上
        Vector2Int.InRange:判断这个点是否在指定范围内
### 	GameObject:
    ReShow:隐藏再显示
        Show/Hide:显示/隐藏物体
### 	String:
        一堆网上嫖的, 自己瞅瞅
## 实用工具:
### 	YuoDelay:
                        使用 this.YuoDelay(需要延迟执行的方法, 可使用兰姆达表达式, 延迟执行的时间),
                        该方法会返回一个YuoDelayMod, 可以通过对mod进行操作来改变延迟方法的状态
                        YuoDelayMod:

                                    AddDelay:可以为还未执行的YuoDelay添加额外的等待时间, 不能减少
                                    SetDelay:可以为还未执行的YuoDelay刚改等待时间, 可以比原来时间小
### 	MouseClickManger:
                        封装了一个判断鼠标是否连点的管理器, 如双击三击(可配置), 可以通过AddCombo(触发的连击次数, 具体事件)
### 	Temp:
                        一些全局的静态变量, 可作为临时变量使用
