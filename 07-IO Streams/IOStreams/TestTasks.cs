using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace IOStreams
{

	public static class TestTasks
	{
		/// <summary>
		/// Parses Resourses\Planets.xlsx file and returns the planet data: 
		///   Jupiter     69911.00
		///   Saturn      58232.00
		///   Uranus      25362.00
		///    ...
		/// See Resourses\Planets.xlsx for details
		/// </summary>
		/// <param name="xlsxFileName">source file name</param>
		/// <returns>sequence of PlanetInfo</returns>
		public static IEnumerable<PlanetInfo> ReadPlanetInfoFromXlsx(string xlsxFileName)
		{
            // TODO : Implement ReadPlanetInfoFromXlsx method using System.IO.Packaging + Linq-2-Xml

            // HINT : Please be as simple & clear as possible.
            //        No complex and common use cases, just this specified file.
            //        Required data are stored in Planets.xlsx archive in 2 files:
            //         /xl/sharedStrings.xml      - dictionary of all string values
            //         /xl/worksheets/sheet1.xml  - main worksheet


            // Don't use OpenXML just to make it harder.

            //Uri baseUri = new Uri(xlsxFileName, UriKind.Relative);

            List<PlanetInfo> result;

            using (Package zipFile = ZipPackage.Open(xlsxFileName, FileMode.Open, FileAccess.Read))
            {
                Uri partUriSheet1 = new Uri(@"/xl/worksheets/sheet1.xml", UriKind.Relative);
                Uri partUriDictionary = new Uri(@"/xl/sharedStrings.xml", UriKind.Relative);
                PackagePart worksheetPart = zipFile.GetPart(partUriSheet1);
                PackagePart stringDictionaryPart = zipFile.GetPart(partUriDictionary);
                using (Stream sheetStream = worksheetPart.GetStream())
                using (Stream dictionaryStream = stringDictionaryPart.GetStream())
                {
                    XDocument docSheet = XDocument.Load(sheetStream);
                    var nsSheet = docSheet.Root.Name.Namespace;
                    XDocument docDictionary = XDocument.Load(dictionaryStream);
                    var nsDict = docDictionary.Root.Name.Namespace;

                    var stringDictionary = docDictionary.Descendants(nsDict + "si")
                        .Select(t => t.Element(nsDict + "t").Value).ToArray();

                    if (stringDictionary.Length == 0)
                    {
                        throw new InvalidDataException();
                    }

                    var rawData = docSheet.Descendants(nsSheet + "row")
                            .Where(x => (int)(x.Attribute("r")) > 1);
                    // Assume we have columns only till AA. 
                    //Is it valid to have R[1]C[1] reference style instead?

                    result = rawData.Select(x => new PlanetInfo()
                    {
                        Name = stringDictionary[(int)(x.Descendants(nsSheet + "v")
                                .Where(t => t.Parent.Attribute("r").Value.StartsWith("A"))
                                .FirstOrDefault())],
                        MeanRadius = Convert.ToDouble(x.Descendants(nsSheet + "v")
                                .Where(t => t.Parent.Attribute("r").Value.StartsWith("B"))
                                .FirstOrDefault().Value)
                    }).ToList();
                }                    
            }
            return result;
        }


		/// <summary>
		/// Calculates hash of stream using specifued algorithm
		/// </summary>
		/// <param name="stream">source stream</param>
		/// <param name="hashAlgorithmName">hash algorithm ("MD5","SHA1","SHA256" and other supported by .NET)</param>
		/// <returns></returns>
		public static string CalculateHash(this Stream stream, string hashAlgorithmName)
		{
            // TODO : Implement CalculateHash method
            try
            {
                using (HashAlgorithm hash = HashAlgorithm.Create(hashAlgorithmName))
                {
                    string hex = BitConverter.ToString(hash.ComputeHash(stream));
                    return hex.Replace("-", "");
                };
            }
            catch (Exception e)
            {

                throw new ArgumentException("Not supported algorithm name", "hashAlgorithmName");
            }
		}


		/// <summary>
		/// Returns decompressed strem from file. 
		/// </summary>
		/// <param name="fileName">source file</param>
		/// <param name="method">method used for compression (none, deflate, gzip)</param>
		/// <returns>output stream</returns>
		public static Stream DecompressStream(string fileName, DecompressionMethods method)
		{
			// TODO : Implement DecompressStream method
			throw new NotImplementedException();
		}


		/// <summary>
		/// Reads file content econded with non Unicode encoding
		/// </summary>
		/// <param name="fileName">source file name</param>
		/// <param name="encoding">encoding name</param>
		/// <returns>Unicoded file content</returns>
		public static string ReadEncodedText(string fileName, string encoding)
		{
			// TODO : Implement ReadEncodedText method
			throw new NotImplementedException();
		}
	}


	public class PlanetInfo : IEquatable<PlanetInfo>
	{
		public string Name { get; set; }
		public double MeanRadius { get; set; }

		public override string ToString()
		{
			return string.Format("{0} {1}", Name, MeanRadius);
		}

		public bool Equals(PlanetInfo other)
		{
			return Name.Equals(other.Name)
				&& MeanRadius.Equals(other.MeanRadius);
		}
	}



}
