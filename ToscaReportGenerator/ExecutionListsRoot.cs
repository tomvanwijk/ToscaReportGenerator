using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BKR.Test.ToscaReportGenerator.Entity
{
    [Serializable]
    [XmlRoot("ExecutionListsRoot")]
    public class ExecutionListsRoot : ObjectEntity
    {
        [XmlAttribute]
        public string Aanmaakdatum { get; set; }
        
        [XmlElement("Folder")]
        public List<FolderEntity> Folder { get; set; }
        [XmlElement("ExecutionList")]
        public List<ExecutionListEntity> ExecutionList { get; set; }

        public ExecutionListsRoot()
        {
            Folder = new List<FolderEntity>();
            ExecutionList = new List<ExecutionListEntity>();
        }

         
    }
    [Serializable]
    public class FolderEntity : ObjectEntity
    {

        [XmlElement("ExecutionList")]
        public List<ExecutionListEntity> ExecutionList { get; set; }
        [XmlElement("Folder")]
        public List<FolderEntity> FolderEntityList { get; set; }

        public FolderEntity()
        {
            ExecutionList = new List<ExecutionListEntity>();
            FolderEntityList = new List<FolderEntity>();
        }
    }
    [Serializable]
    public class ExecutionListEntity: ObjectEntity
    {
        [XmlElement("ExecutionEntryFolder")]
        public List<ExecutionEntryFolderEntity> ExecutionEntryFolderList { get; set; }
        [XmlElement("ExecutionEntry")]
        public List<ExecutionEntryEntity> ExecutionEntry { get; set; }

        public ExecutionListEntity()
        {
            ExecutionEntryFolderList = new List<ExecutionEntryFolderEntity>();
            ExecutionEntry = new List<ExecutionEntryEntity>();
        }
    
    }
    [Serializable]
    public class ExecutionEntryFolderEntity: ObjectEntity
    {
        [XmlElement("ExecutionEntry")]
        public List<ExecutionEntryEntity> ExecutionEntryList { get; set; }
        [XmlElement("ExecutionEntryFolder")]
        public List<ExecutionEntryFolderEntity> ExecutionEntryFolderList { get; set; }

        public ExecutionEntryFolderEntity()
        {
            ExecutionEntryList = new List<ExecutionEntryEntity>();
            ExecutionEntryFolderList = new List<ExecutionEntryFolderEntity>();
        }

    }
    [Serializable]
    public class ExecutionEntryEntity: ObjectEntity
    {


        [XmlAttribute]
        public string Status { get; set; }

        [XmlElement]
        public string RequestFile { get; set; }

        [XmlElement]
        public string ResponseFile { get; set; }

        [XmlElement("Parameter")]
        public List<KeyValuePair> Parameters { get; set; }

        public ExecutionEntryEntity()
        {
            Parameters = new List<KeyValuePair>();
        }
        [XmlElement("Info")]
        public List<String> Info { get; set; }
    }

    public class KeyValuePair
    {
        [XmlAttribute]
        public string Key {get; set;}
        [XmlAttribute]
        public string Value { get; set; }
    }

    public class ObjectEntity
    {
        [XmlAttribute]
        public string Naam { get; set; }

        [XmlAttribute]
        public string NodePath { get; set; }


    }

}
