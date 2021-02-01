# XUnit.Spark
Unit Testing Spark Dotnet application sample

Sample application create to test the spark dotnet application. 

### Steps to execute code:
1. Go to project home director and run $dotnet build
2. It will build the **bin\Debug\netcoreapp3.1\XUnit.Spark.dll**
3. Execute the below command to test spark application:

```
spark-submit --class org.apache.spark.deploy.dotnet.DotnetRunner --master local bin\Debug\netcoreapp3.1\microsoft-spark-3-0_2.12-1.0.0.jar dotnet test bin\Debug\netcoreapp3.1\XUnit.Spark.dll
```


