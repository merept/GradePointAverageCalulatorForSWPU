# SWPU平均学分绩点计算器

可以快速计算平均学分绩点

计算方法仅适用于西南石油大学的平均学分绩点

或满足以下计算方法的学校

> 每门课的学分乘以该科对应的学分绩点后相加，再除以总学分
>
> 单科学分绩点的计算方法是 (期末成绩 - 60) / 10 + 1

### 更新

- [2022.5.7 更新 V0.4.4](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.4.4)

> 新增了异常日志记录（使用 MereyLog 进行记录）

- [2022.2.17 更新 V0.4.3](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.4.3)

> 优化了视觉效果和部分操作逻辑，控件外观匹配当前系统

- [2022.2.11 更新 V0.4.2](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.4.2)

> 1.新增了删除单条历史记录的功能
>
> 2.修复了当输入框有空格或换行依旧能输出结果的错误
>
> 3.修复了在结果详情及历史记录窗口未选中条目依旧能使用右键菜单的错误

- [2022.2.9 更新 V0.4.1](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.4.1)

> 1.更新了历史记录的存储方式，优化了对于相同数据的查重判定，现在结果详情页修改数据可同步至历史记录，历史记录的名称可重命名
>
> 2.上一次退出时输入的数据可以保存了，在有历史记录的情况下，关闭程序重新进入会保留上一次输入的内容

- [2022.1.27 更新 V0.3.5](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.3.5)

> 优化了一些操作逻辑

- [2022.1.27 更新 V0.3.4](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.3.4)

> 1.修复了历史记录窗口双击空白处会闪退的bug
>
> 2.优化了操作逻辑，双击历史记录后会自动打开结果

- [2022.1.20 更新 V0.3.3](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.3.3)

> 增加历史记录功能，可快捷查看历史记录

- [2022.1.15 更新 V0.3.1](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.3.1)

> 优化了电源的使用情况，减少耗电以及CPU占用

### 使用说明

请在输入框输入您每科的学分及期末成绩，并点击输入框下方 ”开始计算“ 按钮进行计算

输入时请严格遵守以下几点：

- 以先输入学分再输入成绩的顺序，否则结果可能会出错 
- 每个数据之间需用任何除数字和小数点外的符号进行间隔
- 可以加上每个科目的名称以方便核对，但是科目名称中请不要包含数字

计算完成后弹出结果框，点击确定可进入详情窗口

在详情窗口可以核对每科学分及成绩

如出现错误可以右击错误的成绩进行修改

### 输入示例

```
高数 5 71
大物 3.5 74
电路 5 73
C语言 3.5 81
```

### 关于如何备份历史记录

[查看说明（GitHub）](https://github.com/merept/GradePointAverageCalulatorForSWPU/blob/master/关于如何备份历史记录.md)

[查看说明（Gitee）](https://gitee.com/merept/GradePointAverageCalulatorForSWPU/blob/master/关于如何备份历史记录.md)
