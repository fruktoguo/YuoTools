# 自己用的一些小工具
#扩展方法:
#	Mathf:
		Clamp : 限定范围  传两个参数为设置最小值和最大值,一个参数为设置最大值,最小值为0,无参设置为不小于0
		InRange: 判断一个数是否在范围内
#	Transform:
		ResetTrans:设置为默认值,即Inspector面板上的Reset
		SetposX、Y、Z:单独设置xyz三个坐标中的一个
		SetLocalPos:同上
		Vector2Int.InRange:判断这个点是否在指定范围内
#	GameObject:
		Show/Hide:显示/隐藏物体
#	String:
		一堆网上嫖的,自己瞅瞅
#实用工具:
#	YuoDelay:
						使用 this.YuoDelay(需要延迟执行的方法,可使用兰姆达表达式,延迟执行的时间),
						该方法会返回一个YuoDelayMod,可以通过对mod进行操作来改变延迟方法的状态
						YuoDelayMod:
									AddDelay:可以为还未执行的YuoDelay添加额外的等待时间,不能减少
									SetDelay:可以为还未执行的YuoDelay刚改等待时间,可以比原来时间小
#	MouseClickManger:
						封装了一个判断鼠标是否连点的管理器,如双击三击(可配置),可以通过AddCombo(触发的连击次数,具体方法)
#	YuoTempVar:
						一些全局的静态变量,可作为临时变量使用
