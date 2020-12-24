using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PBSurgeon;
using Tab = Microsoft.AnalysisServices.Tabular;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string connStr = "localhost:50051";
        [TestMethod]
        public void GetConnections()
        {
            PBSurgeon.Connector.GetActiveConnections();
            var x = PBSurgeon.Connector.Connections;
        }
        [TestMethod]
        public void DumpRawSchema()
        {
            PBSurgeon.Reporter.ConnectionString=connStr;
            var s = PBSurgeon.Reporter.DumpRawSchema();
        }
        [TestMethod]
        public void DumpSchema()
        {
            PBSurgeon.Reporter.ConnectionString = connStr;
            var s = PBSurgeon.Reporter.DumpSchema();
            Console.WriteLine(s);
        }
        [TestMethod]
        public void PrintSchema()
        {
            PBSurgeon.Reporter.ConnectionString = connStr;
            PBSurgeon.Reporter.PrintSchema();            
        }
        [TestMethod]
        public void InsertMeasure()
        {
            PBSurgeon.Updator.ConnectionString = connStr;
            PBSurgeon.Updator.DryRun = false;
            PBSurgeon.Updator.Verbose = true;
            var m = new Tab.Measure();
            
            m.Table.Name = "Sales";
            m.Name = "AshMeasure";
            m.Expression = "SUM([TaxAmt]) / 10";
            PBSurgeon.Updator.UpdateMeasure(m, true);
        }
        [TestMethod]
        public void InsertColumn()
        {
            PBSurgeon.Updator.ConnectionString = connStr;
            PBSurgeon.Updator.DryRun = false;
            PBSurgeon.Updator.Verbose = true;
            //PBSurgeon.Updator.UpdateColumn(new PBSurgeon.Column("Sales", "SalesAmount", Tab.DataType.Decimal, ""), true);
        }
    }
}
