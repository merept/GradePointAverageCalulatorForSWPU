# SWPU平均学分绩点计算器

可以快速计算平均学分绩点

计算方法仅适用于西南石油大学的平均学分绩点

或满足以下计算方法的学校

> 每门课的学分乘以该科对应的学分绩点后相加，再除以总学分
>
> 单科学分绩点的计算方法是 (期末成绩 - 60) / 10 + 1

### 更新

- [2022.5.7 更新 V0.5](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.5)

> 重大功能更新：
>
> 现在可以直接把教务系统成绩页里的全部内容复制过来，粘贴好后直接点击 “开始计算” 即可获得结果，无需再做更改以前的方法依然可用

- [2022.5.7 更新 V0.4.4](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases/tag/V0.4.4)

> 新增了异常日志记录（使用 MereyLog 进行记录）

- [更多历史版本更新内容](https://github.com/merept/GradePointAverageCalulatorForSWPU/releases)

### 使用说明

请在输入框输入您每科的学分及期末成绩，**可直接将教务系统成绩页的全部内容粘贴进输入框**，并点击输入框下方 ”开始计算“ 按钮进行计算

输入时请严格遵守以下几点（**直接粘贴结果时不用管这里，直接计算结果即可**）：

- 以先输入学分再输入成绩的顺序，否则结果可能会出错 
- 每个数据之间需用任何除数字和小数点外的符号进行间隔
- 可以加上每个科目的名称以方便核对，但是科目名称中请不要包含数字

计算完成后弹出结果框，点击确定可进入详情窗口

在详情窗口可以核对每科学分及成绩

如出现错误可以右击错误的成绩进行修改

### 输入示例

> **直接粘贴结果时不用管这里，直接计算结果即可**

```
高数 5 71
大物 3.5 74
电路 5 73
C语言 3.5 81
```

### 关于如何备份历史记录

[查看说明（GitHub）](https://github.com/merept/GradePointAverageCalulatorForSWPU/blob/master/关于如何备份历史记录.md)

[查看说明（Gitee）](https://gitee.com/merept/GradePointAverageCalulatorForSWPU/blob/master/关于如何备份历史记录.md)
