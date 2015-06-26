using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BKR.Test.ToscaReportGenerator.Entity;
using Tricentis.TCAPIObjects.Objects;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace BKR.Test.ToscaReportGenerator
{
    public class FileGenerator
    {
        private const string NOTRUNSTATUS = "Not Run";
        private const string RESULTFILESFOLDER = "ResultFiles";

        private string rootFolder = String.Empty; 
        private string filenameXml = String.Empty; 
        private string filenameXslt =  String.Empty;
        private DirectoryInfo folder;
        private DirectoryInfo resultFolder;
        
        private readonly ExecutionListsRoot root;
        private ExecutionEntryEntity exEntry;
        private ExecutionEntryFolderEntity exEntryFolder;
        private ExecutionListEntity exList;
        
        public FileGenerator(string _rootFolder, string _filenameXml, string _filenameXslt, string _foldername)
        {
            root = new ExecutionListsRoot();
            root.Naam = _rootFolder;
            filenameXml = _filenameXml;
            filenameXslt = _filenameXslt;
            folder = new DirectoryInfo(_foldername);
            resultFolder = new DirectoryInfo(Path.Combine(_foldername, RESULTFILESFOLDER));

            //// Map aanmaken als deze nog niet bestaat
            //if (Directory.Exists(folder.FullName) == false)
            //    Directory.CreateDirectory(folder.FullName);

            //Map aanmaken als deze nog niet bestaat voor de resultaatbestanden en direct ook het XML bestand op 1 hoger niveau.
            if (Directory.Exists(resultFolder.FullName) == false)
                Directory.CreateDirectory(resultFolder.FullName);


            root.Aanmaakdatum = DateTime.Now.ToString("dd-MM-yyyy");
            rootFolder = _rootFolder;

            Helper.Clear();
        }
        public void GenerateXML()
        {
            XmlSerializer writer = new XmlSerializer(typeof(Entity.ExecutionListsRoot));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            using (XmlWriter w =  XmlWriter.Create(Path.Combine(folder.FullName, filenameXml), settings))
            {
                w.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\""+filenameXslt+ "\"");
                writer.Serialize(w, root);
            }
        }

        public void GetSubFolders(TCFolder tcf)
        {
            Helper.Schrijf(tcf.Name, "GetSubFolders", 1);
            foreach (TCObject subitem in tcf.Items)
            {
                if (subitem is ExecutionList)
                {
                    AddExecutionList(subitem);
                    GetSubLists(subitem as ExecutionList);
                }
                if (subitem is TCFolder)
                {
                    GetSubFolders(subitem as TCFolder);
                }
            }
        }

        private void GetSubLists(ExecutionList el)
        {
            Helper.Schrijf(el.Name, "GetSubLists", 3);
            foreach (TCObject subitem in el.Items)
            {
                if (subitem is ExecutionEntryFolder)
                {
                    AddExecutionEntryFolder(subitem);
                    GetSubItems(subitem as ExecutionEntryFolder);
                }
                if (subitem is ExecutionEntry)
                {
                    AddExectionEntry(subitem as ExecutionEntry, exList);
                    Helper.Schrijf((subitem as ExecutionEntry).Name, "GetSubLists_Entry", 4);
                }
            }
        }

        private void GetSubItems(ExecutionEntryFolder eef)
        {
            Helper.Schrijf(eef.Name, "GetSubItems", 5);
            foreach (TCObject subitem in eef.Items)
            {
                if (subitem is ExecutionEntryFolder)
                {
                    AddExecutionEntryFolder(subitem);
                    GetSubItems(subitem as ExecutionEntryFolder);
                }
                if (subitem is ExecutionEntry)
                {
                    AddExectionEntry(subitem as ExecutionEntry, exEntryFolder);
                    Helper.Schrijf((subitem as ExecutionEntry).Name, "GetSubItems_Entry", 6);
                }
            }
        }

        private void AddExecutionList(TCObject tco)
        {
            ExecutionList el;
            if (tco is ExecutionList)
            {
                el = tco as ExecutionList;
            }
            else
            {
                return;
            }

            exList = new ExecutionListEntity();
            exList.Naam = GetNodeName(tco);
            exList.NodePath = tco.NodePath;
            root.ExecutionList.Add(exList);
        }

        private void AddExecutionEntryFolder(TCObject tco)
        {
            ExecutionEntryFolder eef;
            if (tco is ExecutionEntryFolder)
            {
                eef = tco as ExecutionEntryFolder;
            }
            else
            {
                return;
            }

            exEntryFolder = new ExecutionEntryFolderEntity();

            exEntryFolder.Naam = GetEntryFolderName(tco, exList.Naam);
            exEntryFolder.NodePath = tco.NodePath;
            if (eef.Items.OfType<ExecutionEntry>().Any())
                exList.ExecutionEntryFolderList.Add(exEntryFolder);
        }

        private void AddExectionEntry (ExecutionEntry ee, ExecutionListEntity master)
        {
            FillExecutionEntry(ee);
            master.ExecutionEntry.Add(exEntry);
        }

        private void AddExectionEntry (ExecutionEntry ee, ExecutionEntryFolderEntity master)
        {
            FillExecutionEntry(ee);
            master.ExecutionEntryList.Add(exEntry);
        }

        private void FillExecutionEntry(ExecutionEntry ee)
        {
            DateTime dts, dte;
            exEntry = new ExecutionEntryEntity();
            exEntry.Naam = ee.Name;
            exEntry.NodePath = ee.NodePath;
            if (ee.ActualLog == null)
            {
                exEntry.Status = NOTRUNSTATUS;
                return;
            }

            string tijds = ee.ActualLog.StartTime;
            string tijde = ee.ActualLog.EndTime;
            if (DateTime.TryParse(ee.ActualLog.StartTime, out dts))
                tijds = dts.ToString();
            if (DateTime.TryParse(ee.ActualLog.StartTime, out dte))
                tijde = dte.ToString();

            List<KeyValuePair> kv = new List<KeyValuePair>();
            
            exEntry.Status = ee.ActualLog.Result.ToString();
            if (ee.ActualLog.LogDetails == null)
            {
                exEntry.Info = new List<string>();
                exEntry.Info.Add(ee.ActualLog.LogInfo);
                return;
            }


            XDocument xdoc = XDocument.Parse(ee.ActualLog.LogDetails.CompressedLogAsString);

            // Alleen de waarde van het "Detail" attribuut selecteren waarvan het attribuut Detail gevuld is met een mogelijk bestand.
            IEnumerable<String> files = xdoc.Root.Descendants("XmlObject").Attributes("Detail").Where(at => at.Value.Contains(".txt")).Select(at => at.Value);
            // En dat moeten er dan ook 2 zijn...
            if (files.Count() == 2)
            { 
                resultFolder = new DirectoryInfo(Path.Combine(folder.FullName,RESULTFILESFOLDER, exEntry.Naam, dts.ToString("yyyy-MM-dd")));
                exEntry.RequestFile = files.Single(f => f.Contains("request", StringComparison.OrdinalIgnoreCase));
                exEntry.ResponseFile = files.Single(f => f.Contains("response", StringComparison.OrdinalIgnoreCase));
                DateTime starttijd = Convert.ToDateTime( ee.ActualLog.ExecutionLog.StartTime);
                
                // Map aanmaken voor draaidatum als deze nog niet bestaat
                if (Directory.Exists(resultFolder.FullName) == false)
                    Directory.CreateDirectory(resultFolder.FullName);

                if (File.Exists(exEntry.RequestFile))
                {
                    File.Copy(exEntry.RequestFile, Path.Combine(resultFolder.FullName, starttijd.ToString("yyyyMMdd_HHmmss-") + exEntry.Naam + "_request.txt"), true);
                    exEntry.RequestFile = RESULTFILESFOLDER + "/" + exEntry.Naam +"/" +resultFolder.Name + "/" + starttijd.ToString("yyyyMMdd_HHmmss-") + exEntry.Naam + "_request.txt";
                }
                else { exEntry.RequestFile = String.Empty; } 

                if (File.Exists(exEntry.ResponseFile))
                {                   
                    File.Copy(exEntry.ResponseFile, Path.Combine(resultFolder.FullName, starttijd.ToString("yyyyMMdd_HHmmss-") + exEntry.Naam + "_response.txt"), true);
                    exEntry.ResponseFile = RESULTFILESFOLDER + "/" + exEntry.Naam +"/" +resultFolder.Name + "/" + starttijd.ToString("yyyyMMdd_HHmmss-") + exEntry.Naam + "_response.txt";
                }
                else { exEntry.ResponseFile = String.Empty; } 
            }

            String ResultInfo = ee.ActualLog.LogDetails.AggregatedDescription;
            exEntry.Info =  new List<string>(ResultInfo.Split(new String[] {Environment.NewLine },StringSplitOptions.RemoveEmptyEntries));
                
            kv.Add(new KeyValuePair { Key = "Starttijd", Value = tijds });
            kv.Add(new KeyValuePair { Key = "Eindttijd", Value = tijde });
            kv.Add(new KeyValuePair { Key = "Doorlooptijd", Value = Math.Round(ee.ActualLog.Duration).ToString() + " ms"  });
            exEntry.Parameters = kv;

        }

        private string GetNodeName(TCObject obj)
        {
            return rootFolder + obj.NodePath.Substring(obj.NodePath.IndexOf(rootFolder) + rootFolder.Length);
        }

        private string GetEntryFolderName(TCObject obj, String master)
        {
            return obj.NodePath.Substring(obj.NodePath.IndexOf(master) + master.Length );
        }


    }
}
