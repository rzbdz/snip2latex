# Snip to Latex
 ![](https://github.com/PanJunzhong/snip2latex/tree/master/project_info/Assets/Logo.png)

一个基于第三方API的UWP应用程序，将图像转换为Latex公式表达式。

该项目使用腾讯OCR公式识别API（我希望使用Mathpix的API，但它需要一张visa信用卡）。很明显，结果有时与预期不符。但它仍然可以用于一些简单的任务，如识别不太复杂的公式。可以重新编写Facade 以适用于不同的公司API (Facade接口目前未完全统一,仍然使用了多个不同的转换类,即将完成这部分工作)

查看呈现器通过加载mathjax联机javascript工作，并基于本地html页面。

当前版本支持公式识别和转换图像文件中的Latex代码和剪贴板中的图像流。已识别的项将存储在“历史记录”页中。

项目结构 :

- Model/
  - HistoryData.cs : 用于保存和导入识别历史的json文件
  - SaveSetting.cs :用于保存和导入设置的json文件
  - TencentWrapper.cs : 使用tencent api序列化和反序列化公式对象
  - Baidu/ (No longger)
    - ~~BaiduDataWrapper.cs~~ : 从api返回的json的jsondserializer和模型(没有使用Baidu的SDK)
- View/
  - AboutPage.xaml/.cs : 关于第页
  - ClipBoardPage.xaml/.cs :  从剪贴板转换（将很快合并到convertpage）
  - ConvertPage.xaml/.cs : convert from file
  - ErrorPage.xaml/.cs : show error message
  - HistoryTemplate.xaml/.cs : StackPanel control template for histoty page
  - History.xaml/.cs : read the history data and show (GridView)
  - Home.xaml/.cs : Home page
  - Settings.xaml/.cs : settings
- MainPage.xaml/.cs : Main page UI 
- ~~BaiduApi.cs~~ : use HttpClient to send POST request to baidu to get token
- ~~BaiduLatexFacade.cs~~ : A facade design pattern to due with the relation between image file and Model class
- TencentLatexFacade.cs : 只识别公式外观
- TencentPaperFacade.cs : 既认公式又认单词
- MathJaxServerBase.cs : 不同公式模型的基本html-对象转换器
- MathJaxServerForBaidu.cs : html-object converter
- MathJaxServerForTencent.cs : as above
- [gitIgnored]MyBaiduToken.cs : three String method return the api key
- [gitIgnored]TencentToken.cs : a TencentToken class and a UserToken class

Dependencies:
 - Tencent Cloud API 3.0 SDK for .NET (Apache-2.0)
 - MathJax Version 3 js (Apache-2.0)
 - Newtonsoft Json.NET (MIT)

要导入项目，请确保您的visual studio至少安装了Windows 10 1903通用平台SDK。

你应该创造TencentToken.cs以及MyBaiduToken.cs在的根文件夹关于TencentLatexFacade.cs文件在Visual Studio中打开.sln项目文件之前。（您可以在“project_info”文件夹中获取示例）

The detailed codes are as follows:

- TencentToken.cs

```csharp
namespace snip2latex
{
    public static class TencentToken
    {
        public static readonly string secrectId = "your id";
        public static readonly string secrectKey = "your key";
    }

	public class UserToken
	{
    	public string userId { get; set; }
    	public string userKey { get; set; }
	}
}

```

- MyBaiduToken.cs

```csharp
namespace snip2latex
{
	public static class MyBaiduToken
	{
		public static String getApiKey()
		{
            return "[your api key]";
        }
        public static String getAppID()
        {
            return "your app id";
        }
        public static String getSecretKey()
        {
            return "your secrect key";
        }
    }
}
```

  Preview screenshots:

![HomePage](https://github.com/PanJunzhong/snip2latex/tree/master/project_info/HomePage.png)

![ConvertPage](https://github.com/PanJunzhong/snip2latex/tree/master/project_info/ConvertPage.png)

![HistoryPage](https://github.com/PanJunzhong/snip2latex/tree/master/project_info/HistoryPage.png)

![HistoryDetails](https://github.com/PanJunzhong/snip2latex/tree/master/project_info/HistoryDetails.png)

 ![](snip2latex\Assets\Logo.png)

An UWP app converting image to LaTeX formula expression based on third party api.

The project use Tencent OCR formulas recognizing API(I was hoping to use Mathpix 's api but it requires an visa credit card). It's obvious that the outcome would not match what was expected sometimes . But it still could be use in some simple task such as recognizing not very complicated formulas.

The viewing renderer works by loading mathjax online javascript and is based on a local html page. 

Current version support formula recognizing and convert latex code from image file and image stream from the clipboard. The recognized item would be stored in the history page.

Project Structure :

- Model/
  - HistoryData.cs : for saving and importing recognizing history json file
  - SaveSetting.cs : for saving and importing application settings json file
  - TencentWrapper.cs : using the tencent api to serialize and deserialize formula objects
  - Baidu/ (No longger)
    - ~~BaiduDataWrapper.cs~~ : JsonDeSerializer and model for the  json returned from api
- View/
  - AboutPage.xaml/.cs : About page
  - ClipBoardPage.xaml/.cs :  convert from clipboard (would be merged into convertpage soon)
  - ConvertPage.xaml/.cs : convert from file
  - ErrorPage.xaml/.cs : show error message
  - HistoryTemplate.xaml/.cs : StackPanel control template for histoty page
  - History.xaml/.cs : read the history data and show (GridView)
  - Home.xaml/.cs : Home page
  - Settings.xaml/.cs : settings
- MainPage.xaml/.cs : Main page UI 
- ~~BaiduApi.cs~~ : use HttpClient to send POST request to baidu to get token
- ~~BaiduLatexFacade.cs~~ : A facade design pattern to due with the relation between image file and Model class
- TencentLatexFacade.cs : recognize only formula facade
- TencentPaperFacade.cs : recognize both formula and words
- MathJaxServerBase.cs : base html-object converter for different formula model
- MathJaxServerForBaidu.cs : html-object converter
- MathJaxServerForTencent.cs : as above
- [gitIgnored]MyBaiduToken.cs : three String method return the api key
- [gitIgnored]TencentToken.cs : a TencentToken class and a UserToken class

Dependencies:
 - Tencent Cloud API 3.0 SDK for .NET (Apache-2.0)
 - MathJax Version 3 js (Apache-2.0)
 - Newtonsoft Json.NET (MIT)

To import the project, make sure your visual studio have intallled at least the Windows Universal Platform  SDK for Windows 10 1903. 

And you are supposed to create the TencentToken.cs and the MyBaiduToken.cs in the root folder of the TencentLatexFacade.cs file before open the .sln project file in Visual Studio. (You can get samples in project_info folder)

The detailed codes are as follows:

- TencentToken.cs

```csharp
namespace snip2latex
{
    public static class TencentToken
    {
        public static readonly string secrectId = "your id";
        public static readonly string secrectKey = "your key";
    }

	public class UserToken
	{
    	public string userId { get; set; }
    	public string userKey { get; set; }
	}
}

```

- MyBaiduToken.cs

  ```csharp
  namespace snip2latex
  {
      public static class MyBaiduToken
      {
          public static String getApiKey()
          {
              return "[your api key]";
          }
          public static String getAppID()
          {
              return "your app id";
          }
          public static String getSecretKey()
          {
              return "your secrect key";
          }
      }
  }
  ```

Preview screenshots:

![HomePage](project_info\HomePage.png)

![ConvertPage](project_info\ConvertPage.png)

![HistoryPage](project_info\HistoryPage.png)

![HistoryDetails](project_info\HistoryDetails.png)





