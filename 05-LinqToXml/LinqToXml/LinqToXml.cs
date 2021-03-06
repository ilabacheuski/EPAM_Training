﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace LinqToXml
{
    public static class LinqToXml
    {
        /// <summary>
        /// Creates hierarchical data grouped by category
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation (refer to CreateHierarchySourceFile.xml in Resources)</param>
        /// <returns>Xml representation (refer to CreateHierarchyResultFile.xml in Resources)</returns>
        public static string CreateHierarchy(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            var newdata =
                new XElement("Root",
                    from data in doc.Elements("Data")
                    group data by (string)data.Element("Category") into groupedData
                    select new XElement("Group",
                        new XAttribute("ID", groupedData.Key),
                        from g in groupedData
                        select new XElement("Data",
                            g.Element("Quantity"),
                            g.Element("Price")
                        )
                    )
                );
            return newdata.ToString();
        }

        /// <summary>
        /// Get list of orders numbers (where shipping state is NY) from xml representation
        /// </summary>
        /// <param name="xmlRepresentation">Orders xml representation (refer to PurchaseOrdersSourceFile.xml in Resources)</param>
        /// <returns>Concatenated orders numbers</returns>
        /// <example>
        /// 99301,99189,99110
        /// </example>
        public static string GetPurchaseOrders(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            XNamespace aw = "http://www.adventure-works.com";
            var purchases =
                (from data in doc.Elements(aw + "PurchaseOrder")
                 where
                     (from addr in data.Elements(aw + "Address")
                      where
                         (string)addr.Attribute(aw + "Type") == "Shipping" &&
                         (string)addr.Element(aw + "State") == "NY"
                      select addr)
                     .Any()
                 select (string) data.Attribute(aw + "PurchaseOrderNumber"));
            return String.Join(",", purchases.ToArray());
        }

        /// <summary>
        /// Reads csv representation and creates appropriate xml representation
        /// </summary>
        /// <param name="customers">Csv customers representation (refer to XmlFromCsvSourceFile.csv in Resources)</param>
        /// <returns>Xml customers representation (refer to XmlFromCsvResultFile.xml in Resources)</returns>
        public static string ReadCustomersFromCsv(string customers)
        {
            string[] source = customers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            XElement cust = new XElement("Root",
                from str in source
                let fields = str.Split(',')
                select new XElement("Customer", 
                    new XAttribute("CustomerID",    fields[0]),
                    new XElement("CompanyName",     fields[1]),
                    new XElement("ContactName",     fields[2]),
                    new XElement("ContactTitle",    fields[3]),
                    new XElement("Phone",           fields[4]),
                    new XElement("FullAddress",
                        new XElement("Address",     fields[5]),
                        new XElement("City",        fields[6]),
                        new XElement("Region",      fields[7]),
                        new XElement("PostalCode",  fields[8]),
                        new XElement("Country",     fields[9])
                        )
                    )
                );
            return cust.ToString();
        }

        /// <summary>
        /// Gets recursive concatenation of elements
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation of document with Sentence, Word and Punctuation elements. (refer to ConcatenationStringSource.xml in Resources)</param>
        /// <returns>Concatenation of all this element values.</returns>
        public static string GetConcatenationString(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            var res =
                from str in doc.Elements()
                select str.Value;
            return res.Aggregate(new StringBuilder(), (sb, el) => sb.Append(el), r=>r.ToString());
        }

        /// <summary>
        /// Replaces all "customer" elements with "contact" elements with the same childs
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with customers (refer to ReplaceCustomersWithContactsSource.xml in Resources)</param>
        /// <returns>Xml representation with contacts (refer to ReplaceCustomersWithContactsResult.xml in Resources)</returns>
        public static string ReplaceAllCustomersWithContacts(string xmlRepresentation)
        {
            XDocument doc = XDocument.Parse(xmlRepresentation);
            foreach (var el in doc.Root.Elements("customer"))
            {
                el.Name = "contact";
            }
            return doc.ToString();

        }

        /// <summary>
        /// Finds all ids for channels with 2 or more subscribers and mark the "DELETE" comment
        /// </summary>
        /// <param name="xmlRepresentation">Xml representation with channels (refer to FindAllChannelsIdsSource.xml in Resources)</param>
        /// <returns>Sequence of channels ids</returns>
        public static IEnumerable<int> FindChannelsIds(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            var res =
                from el in doc.Elements("channel")
                where
                    el.Elements("subscriber").Count() > 1 &&
                    el.Nodes().OfType<XComment>().Any(com=>com.Value == @"DELETE")
                select (int)el.Attribute("id");
            return res.ToList();
        }

        /// <summary>
        /// Sort customers in docement by Country and City
        /// </summary>
        /// <param name="xmlRepresentation">Customers xml representation (refer to GeneralCustomersSourceFile.xml in Resources)</param>
        /// <returns>Sorted customers representation (refer to GeneralCustomersResultFile.xml in Resources)</returns>
        public static string SortCustomers(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            XElement newCustord =
                new XElement("Root",
                from cust in doc.Elements("Customers")
                orderby (string)cust.Element("FullAddress").Element("Country"), (string)cust.Element("FullAddress").Element("City")
                select cust
                );
            return newCustord.ToString();
        }

        /// <summary>
        /// Gets XElement flatten string representation to save memory
        /// </summary>
        /// <param name="xmlRepresentation">XElement object</param>
        /// <returns>Flatten string representation</returns>
        /// <example>
        ///     <root><element>something</element></root>
        /// </example>
        public static string GetFlattenString(XElement xmlRepresentation)
        {
            var descendants = xmlRepresentation.Descendants().ToList();

            // So we need to strip child elements from everywhere...
            // (but only elements, not text nodes). The ToList() call
            // materializes the query, so we're not removing while we're iterating.
            foreach (var nested in descendants.Elements().ToList())
            {
                nested.Remove();
            }
            xmlRepresentation.ReplaceNodes(descendants); //FLATTENING

            return xmlRepresentation.ToString(SaveOptions.DisableFormatting); //JUST REMOVE WHITESPACES!
        }

        /// <summary>
        /// Gets total value of orders by calculating products value
        /// </summary>
        /// <param name="xmlRepresentation">Orders and products xml representation (refer to GeneralOrdersFileSource.xml in Resources)</param>
        /// <returns>Total purchase value</returns>
        public static int GetOrdersValue(string xmlRepresentation)
        {
            XElement doc = XElement.Parse(xmlRepresentation);
            return doc.Element("Orders").Elements("Order").Select(e => (int)e.Element("product"))
                .Join(doc.Element("products").Elements("product"), a => a, b => (int)b.Attribute("Id"), (a, b) => (int)b.Attribute("Value")).Sum();
        }
    }
}
