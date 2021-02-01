// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Spark.Interop.Ipc;
using Microsoft.Spark.Network;
using Microsoft.Spark.Sql;
using Microsoft.Spark.Sql.Types;
using Microsoft.Spark.Utils;
using Moq;
using Razorvine.Pickle;
using Xunit;

namespace XUnit.Spark{

    public class DataFrameTests
    {
        private readonly SparkSession _spark;
        private readonly DataFrame _df;
        private SparkFixture fixture;

        public DataFrameTests()
        {
            fixture = new SparkFixture();
            _spark = fixture.Spark;
            _df = _spark
                .Read()
                .Schema("age INT, name STRING")
                .Json($"Resources/people.json");
            
        }

         [Fact]
         public void TestCollect()
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>>> Dataframe unit testing started >>>>>>>>>>> ");
            _df.Show(10);
            Row[] rows = _df.Collect().ToArray();
            Assert.Equal(3, rows.Length);

            Row row1 = rows[0];
            Assert.Equal("Michael", row1.GetAs<string>("name"));
            Assert.Null(row1.Get("age"));

            Row row2 = rows[1];
            Assert.Equal("Andy", row2.GetAs<string>("name"));
            Assert.Equal(30, row2.GetAs<int>("age"));

            Row row3 = rows[2];
            Assert.Equal("Justin", row3.GetAs<string>("name"));
            Assert.Equal(19, row3.GetAs<int>("age"));
            Console.WriteLine(">>>>>>>>>>>>>>>>>> Dataframe unit testing end >>>>>>>>>>> ");
        }
    }
}