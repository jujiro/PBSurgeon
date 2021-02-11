using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PBSurgeon;
using Tab = Microsoft.AnalysisServices.Tabular;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        string connStr = "localhost:54166";
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
        public void RenameTable()
        {
            PBSurgeon.Renamer.ConnectionString = connStr;
            PBSurgeon.Renamer.DryRun = false;
            PBSurgeon.Renamer.Verbose = true;
            PBSurgeon.Renamer.RenameTable("Sales", "NewSales");
        }
        [TestMethod]
        public void RenameColumn()
        {
            PBSurgeon.Renamer.ConnectionString = connStr;
            PBSurgeon.Renamer.DryRun = false;
            PBSurgeon.Renamer.Verbose = true;
            PBSurgeon.Renamer.RenameColumn("Sales", "DiscAmount", "DiscountAmount");
        }

        [TestMethod]
        public void InsertMeasure()
        {
            PBSurgeon.Updator.ConnectionString = connStr;
            PBSurgeon.Updator.DryRun = false;
            PBSurgeon.Updator.Verbose = true;
            var fld = new PBSurgeon.Field();            
            fld.TableName = "Category";
            fld.FieldType = PBSurgeon.FieldType.Measure;
            fld.Name = "A5";
            fld.FormatString = "#,0.0";
            fld.Expression = "13.34 * 3.445566";
            PBSurgeon.Updator.UpsertField(fld, true);
        }
        [TestMethod]
        public void InsertColumn()
        {
            PBSurgeon.Updator.ConnectionString = connStr;
            PBSurgeon.Updator.DryRun = false;
            PBSurgeon.Updator.Verbose = true;
            var fld = new PBSurgeon.Field();
            fld.TableName = "Sales";
            fld.FieldType = PBSurgeon.FieldType.CalculatedColumn;
            fld.Name = "CalcCol1";
            fld.FormatString = "#,0.0";
            fld.Expression = "[UnitPrice] * [TotalProductCost]";
            PBSurgeon.Updator.UpsertField(fld, true);
        }
        [TestMethod]
        public void InsertColumn2()
        {
            PBSurgeon.Updator.ConnectionString = connStr;
            PBSurgeon.Updator.DryRun = false;
            PBSurgeon.Updator.Verbose = true;
            var fld = new PBSurgeon.Field();
            fld.TableName = "Sales";
            fld.FieldType = PBSurgeon.FieldType.Column;
            fld.Name = "CalcCol2";
            fld.FormatString = "#,0.0000000";
            fld.SourceColumnName = "[UnitPrice]";
            //fld.Expression = "[UnitPrice] * [TotalProductCost]";
            PBSurgeon.Updator.UpsertField(fld, true);
        }
    }
}
