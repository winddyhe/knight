# WindJson解析库
* WindJson解析库中的词法分析算法采用词法状态转换矩阵算法，该算法的代码非常精简，将每个类别的单词分析高度抽象成为一个状态转换矩阵进行表示，同时将分析Json的词法和Json语法抽象成了两个层次，因此代码逻辑结构变得非常简单。有兴趣的朋友可以去读LexicalAnalysis.cs和JsonParser.cs文件。

## WindJson支持的功能
* 支持标准的Json格式解析
* 方便的从object/jsonstring/jsonnode三者之间相互转化
* 兼容重复的逗号，分号
* 支持枚举类型、true/false关键字的识别
* 支持 // /**/ 注释的识别
* 能够在热更新端使用该Json库

## WindJson的API
* jsonstring ==> jsonnode
```C#
JsonParser rJsonParser = new JsonParser(rText);
JsonNode rJsonNode = rJsonParser.Parser();
```
* jsonnode ==> object
```C#
var rObjet = rJsonNode.ToObject(rObjectType);
var rObjet1 = rJsonNode.ToObject<ObjectType>();			// To object

var rObjectList = rJsonNode.ToList<ObjectType>();		// To list

var rObjectDict = rJsonNode.ToDict<TKey, TObject>();	// To dictionary
```
* object ==> jsonnode
```C#
JsonNode rJsonNode = JsonParser.ToJsonNode(rObject);
```

* jsonnode ==> jsonstring
```C#
string rJsonString = rJsonNode.ToString();
```

## 测试用例
![JsonParser1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/json_1.png)
![JsonParser1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/json_2.png)

## 词法分析算法的扩展
* 该词法分析算法能够解析出数字、标识符、字符串、注释、一些特殊字符等等，因此我们可以用它来做其他格式的配置的解析。
* 在框架中解析了一种特殊格式的配置，这个是一个技能配置，算法详情参见热更DLL中的GamePlayComponentParser.cs。
* ![JsonParser1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/json_3.png)
