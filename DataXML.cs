using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

public class DataXML
{

	public DataTable GetDataTable(string xmlFile, string topLevelElement, string filterType = "", string filterValue = "")
	{
		//// CHECK TO SEE IF LOCALIZED XML FILE EXISTS, IF SO USE THAT INSTEAD
		string currentCulture = System.Globalization.CultureInfo.CurrentCulture.ToString().ToLower();
		if ((currentCulture != "en-us" & !String.IsNullOrEmpty(currentCulture)))
		{
			int currentFileNameLength = xmlFile.Trim().Length;
			string localizedXmlFile = xmlFile.Substring(0, currentFileNameLength - 4) + "." + currentCulture + ".xml";
			if (File.Exists(localizedXmlFile) == true)
			{
				xmlFile = localizedXmlFile;
			}
		}

		topLevelElement = topLevelElement.Trim();
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		XmlReader xmlReader = XmlReader.Create(xmlFile, xmlReaderSettings);
		xmlReader.ReadToFollowing(topLevelElement);

		//// CREATE A DATATABLE TO CONTAINS ONLY THE DATE WE NEED
		DataTable dataTableXml = new DataTable();
		DataColumn dataColumnID = new DataColumn("id", typeof(int));
		dataTableXml.Columns.Add(dataColumnID);


		int tempID = 0;
		//// CREATE A HASHTABLE, TO HOLD TEMPORARY VALUES AS WE WORK THROUGH THE XML FILE
		System.Collections.Hashtable hashtable = new System.Collections.Hashtable();

		//// READ THROUGH THE XML DATASOURCE AND ADD TITLES AND DESCRIPTIONS TO DATATABLE	
		while (xmlReader.Read())
		{
			if (xmlReader.NodeType != XmlNodeType.Whitespace)
			{
				try
				{
					DataRow dataRow = dataTableXml.NewRow();


					if (xmlReader.NodeType == XmlNodeType.EndElement & xmlReader.LocalName.ToString() == topLevelElement)
					{
						//// REACHED THE END OF THE PARENT ELEMENT, ADD THE ROW TO THE DATATABLE
						tempID = tempID + 1;
						//// PULL TEMPORARY VALUES FROM HASHTABLE
						if ((filterType == "id" & filterValue != tempID.ToString()))
						{
							//// DO NOTHING, RECORD SHOULD BE FILTERED OUT
						}
						else
						{
							bool isFilter = false;
							foreach (DictionaryEntry dictionaryEntry in hashtable)
							{
								if (filterType == dictionaryEntry.Key.ToString() & filterValue != dictionaryEntry.Value.ToString())
								{
									isFilter = true;
									break;
								}
								else
								{
									dataRow[dictionaryEntry.Key.ToString()] = dictionaryEntry.Value.ToString();
								}
							}
							if (isFilter == false)
							{
								dataRow["id"] = tempID;
								dataTableXml.Rows.Add(dataRow);
							}
						}
                        //// EMPTY OUT THE HASHTABLE
                        hashtable.Clear();

					}
					else if (xmlReader.NodeType != XmlNodeType.EndElement & xmlReader.LocalName.ToString() != topLevelElement)
					{
						//// IF FIRST TIME THIS ELEMENT ENCOUNTERED, ADD IT TO THE DATABASE COLUMN
						if (dataTableXml.Columns.Contains(xmlReader.LocalName.ToString()) == false)
						{
							DataColumn dataColumn = new DataColumn(xmlReader.LocalName.ToString(), typeof(string));
							dataTableXml.Columns.Add(dataColumn);
							dataColumn = null;
						}

						//Current.Response.Write("<B>tempID " + xmlReader.LocalName.ToString + "(" + xmlReader.NodeType.ToString + ")" + ": </B>")
						if (xmlReader.NodeType != XmlNodeType.CDATA)
						{
							//Current.Response.Write("<I>" + xmlReader.ReadElementContentAsString.ToString + "</I><br />")
							//dataRow(xmlReader.LocalName.ToString) = xmlReader.ReadElementContentAsString.ToString
							hashtable.Add(xmlReader.LocalName.ToString(), xmlReader.ReadElementContentAsString().ToString());
						}
					}
				}
				catch //(Exception ex)
				{
					//// SUPPRESS ERRORS
				}
			}
		}

		xmlReader.Close();
		return dataTableXml;

	}

}

