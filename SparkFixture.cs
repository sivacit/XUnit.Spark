// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Spark.Interop;
using Microsoft.Spark.Interop.Ipc;
using Microsoft.Spark.Sql;
using Moq;
using Xunit;

namespace XUnit.Spark
{
    public sealed class SparkFixture : IDisposable
    {        
        public const string DefaultLogLevel = "ERROR";

        internal SparkSession Spark { get; }

        internal Mock<IJvmBridge> MockJvm { get; private set; }

        public SparkFixture()
        {
            Spark = SparkSession
                .Builder()
                // Lower the shuffle partitions to speed up groupBy() operations.
                .Config("spark.sql.shuffle.partitions", "3")
                .Config("spark.ui.enabled", false)
                .Config("spark.ui.showConsoleProgress", false)
                .AppName("Microsoft.Spark.E2ETest")
                .GetOrCreate();
                
            Spark.SparkContext.SetLogLevel(DefaultLogLevel);

            SetupBasicMockJvm();

            // Unit tests may contain calls that hit the AssemblyLoader.
            // One of the AssemblyLoader assembly search paths is populated
            // using SparkFiles. Unless we are running in an E2E scenario and
            // on the Worker, SparkFiles will attempt to call the JVM. Because
            // this is a (non E2E) Unit test, it is necessary to mock this call.
            SetupSparkFiles();

            var mockJvmBridgeFactory = new Mock<IJvmBridgeFactory>();
            mockJvmBridgeFactory
                .Setup(m => m.Create(It.IsAny<int>()))
                .Returns(MockJvm.Object);

            SparkEnvironment.JvmBridgeFactory = mockJvmBridgeFactory.Object;
        }

        public void Dispose()
        {
        }

        internal IJvmBridge Jvm { get; }

        private void SetupBasicMockJvm()
        {
            MockJvm = new Mock<IJvmBridge>();

            MockJvm
                .Setup(m => m.CallStaticJavaMethod(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()))
                .Returns(
                    new JvmObjectReference("result", MockJvm.Object));
            MockJvm
                .Setup(m => m.CallStaticJavaMethod(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<object>()))
                .Returns(
                    new JvmObjectReference("result", MockJvm.Object));
            MockJvm
                .Setup(m => m.CallStaticJavaMethod(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>()))
                .Returns(
                    new JvmObjectReference("result", MockJvm.Object));

            MockJvm
                .Setup(m => m.CallNonStaticJavaMethod(
                    It.IsAny<JvmObjectReference>(),
                    It.IsAny<string>(),
                    It.IsAny<object>()))
                .Returns(
                    new JvmObjectReference("result", MockJvm.Object));
            MockJvm
                .Setup(m => m.CallNonStaticJavaMethod(
                    It.IsAny<JvmObjectReference>(),
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<object>()))
                .Returns(
                    new JvmObjectReference("result", MockJvm.Object));
            MockJvm
                .Setup(m => m.CallNonStaticJavaMethod(
                    It.IsAny<JvmObjectReference>(),
                    It.IsAny<string>(),
                    It.IsAny<object[]>()))
                .Returns(
                    new JvmObjectReference("result", MockJvm.Object));
        }

        private void SetupSparkFiles()
        {
            MockJvm
                .Setup(m => m.CallStaticJavaMethod(
                    "org.apache.spark.SparkFiles",
                    "getRootDirectory"))
                .Returns("SparkFilesRootDirectory");
        }
    }

    [CollectionDefinition("Spark Unit Tests")]
    public class SparkCollection : ICollectionFixture<SparkFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}