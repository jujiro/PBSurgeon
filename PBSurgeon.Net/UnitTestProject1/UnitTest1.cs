using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PBSurgeon;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string connStr = "localhost:49883";
        [TestMethod]
        public void DumpSchema()
        {
            PBSurgeon.Reporter.ConnectionString=connStr;
            var s = PBSurgeon.Reporter.DumpSchema();
        }
        [TestMethod]
        public void InsertMEasure()
        {
            PBSurgeon.Updator.ConnectionString = connStr;
            PBSurgeon.Updator.DryRun = true;
            PBSurgeon.Updator.Verbose = true;
            PBSurgeon.Updator.UpdateMeasure(new Measure("Sales", "AshMeasure", "SUM([TaxAmt])", null, null), true);
        }
    }
}
