using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace LinqToXml.Test
{
    [TestClass]
    public class LinqToXmlTests
    {
        [TestMethod]
        [TestCategory("LinqToXml.CreateHierarchyTest")]
        public void CreateHierarchyTest()
        {
            Assert.AreEqual(LinqToXmlResources.CreateHierarchyResultFile, LinqToXml.CreateHierarchy(LinqToXmlResources.CreateHierarchySourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetPurchaseOrders")]
        public void GetPurchaseOrdersTest()
        {
            Assert.AreEqual("99505,99607", LinqToXml.GetPurchaseOrders(LinqToXmlResources.PurchaseOrdersSourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.ReadCustomersFromCsv")]
        public void ReadCustomersFromCsvTest()
        {
            Assert.AreEqual(LinqToXmlResources.XmlFromCsvResultFile, LinqToXml.ReadCustomersFromCsv(LinqToXmlResources.XmlFromCsvSourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetConcatenationString")]
        public void GetConcatenationStringTest()
        {
            Assert.AreEqual(LinqToXmlResources.ConcatenationStringResult, LinqToXml.GetConcatenationString(LinqToXmlResources.ConcatenationStringSource));
        }

        [TestMethod]
        [TestCategory("LinqToXml.ReplaceAllCustomersWithContacts")]
        public void ReplaceAllCustomersWithContactsTest()
        {
            Assert.AreEqual(LinqToXmlResources.ReplaceCustomersWithContactsResult, LinqToXml.ReplaceAllCustomersWithContacts(LinqToXmlResources.ReplaceCustomersWithContactsSource));
        }

        [TestMethod]
        [TestCategory("LinqToXml.FindChannelsIds")]
        public void FindChannelsIdsTest()
        {
            Assert.IsTrue(new int[] { 7 }.SequenceEqual(LinqToXml.FindChannelsIds(LinqToXmlResources.FindAllChannelsIdsSource)));
        }

        [TestMethod]
        [TestCategory("LinqToXml.SortCustomers")]
        public void SortCustomersTest()
        {
            Assert.AreEqual(LinqToXmlResources.GeneralCustomersResultFile, LinqToXml.SortCustomers(LinqToXmlResources.GeneralCustomersSourceFile));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetFlattenString")]
        public void GetFlattenStringTest()
        {
            var doc = XElement.Parse(LinqToXmlResources.GeneralOrdersFileSource);
            Assert.AreEqual("<Root><Orders /><Order /><CustomerID>HANAR</CustomerID><EmployeeID>3</EmployeeID><OrderDate>1996-07-10T00:00:00</OrderDate><RequiredDate>1996-07-24T00:00:00</RequiredDate><product>1</product><Order /><CustomerID>CHOPS</CustomerID><EmployeeID>5</EmployeeID><OrderDate>1996-07-11T00:00:00</OrderDate><RequiredDate>1996-08-08T00:00:00</RequiredDate><product>2</product><Order /><CustomerID>RICSU</CustomerID><EmployeeID>9</EmployeeID><OrderDate>1996-07-12T00:00:00</OrderDate><RequiredDate>1996-08-09T00:00:00</RequiredDate><product>1</product><products /><product Id=\"1\" Value=\"300\" /><product Id=\"2\" Value=\"910\" /></Root>", 
                LinqToXml.GetFlattenString(doc));
        }

        [TestMethod]
        [TestCategory("LinqToXml.GetFlattenString")]
        public void GetOrdersValueTest()
        {
            Assert.AreEqual(1510, LinqToXml.GetOrdersValue(LinqToXmlResources.GeneralOrdersFileSource));
        }
    }
}
