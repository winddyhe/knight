## 2017年项目计划Ticket
### 2017/06/29 【进行中】
* 图形渲染模块
	* 实现一个3D的Text和Image渲染，自定义渲染管理，使得渲染效率达到最优。  已完成
	* 优化Standard Shader的复杂度，实现手机上PBR渲染。   开始
	* 实现一些通用的、高效率的Shader效果。
		* Texture						已完成
			* VertexColor + Color + Opaque + Cull切换 + ZWrite切换
			* VertexColor + Color + Transparent + Cull切换 + ZWrite切换 + Blend切换
			* VertexColor + Color + Texture + Opaque + Cull切换 + ZWrite切换
			* VertexColor + Color + Texture + Transparent + Cull切换 + ZWrite切换 + Blend切换
		
		* Diffuse		已完成
			* 把Mobile/Diffuse下的Shader移植出来
			* 有Transparent、Cut off、Opaque、带Fog、带Color
			* ColorBlend: lerp color and maintexture color
			
		* UI
			* UI + Mask切换 + Gray切换
		
		* Normal Effect
			* Texture + RotateUV + Color + 上下翻转UV + 左右翻转UV + Overlay(Additive, Multiply, AlphaBlend)
			* Texture + RotateUV1 + Color1 + RotateUV2 + Color2 + 上下翻转UV + 左右翻转UV  + Overlay(Additive, Multiply, AlphaBlend)
			
		* Post Effect
			* Bloom
			* HDR曝光
			* Motion Blur
			* Radial Blur
			* Global Fog
			* 屏幕扭曲
			* 屏幕碎裂
		
		* Special Effect
			* Grap Screen实现镜面反射
			* 水面效果
			* 溶解效果
			* 自由发挥


### 未来计划
* 输入模块
	* 完善输入模块
	* 统一输入接口
	
* 摄像机模块
	* 接入输入模块，实现触摸屏上操控相机旋转、缩放和移动
	* 碰撞检测

* UI模块
	* 完善UI框架，回退记录功能
	* 重新设计UI的层级结构，以实现 渲染和顶点更新的效率达到最优
	* 编写UI通用控件

* NativeDialog 本地对话框
	* 找一个NativeDialog插件、开源的进行改造。
	* 统一对话框接口，实现android、ios、editor下的本地对话框接口。
	
* GamePlay框架1（侠客风云传的回合制战斗）
	* 六边形的地面网格算法实现
	* 尝试ECS设计思路设计战斗逻辑框架
	* 实现服务器的战斗逻辑
	
* GamePlay框架2（普通MMORPG战斗，站桩）

* GamePlay框架3（ARPG战斗，非站桩）

* GamePlay框架4（2D飞行格斗）








