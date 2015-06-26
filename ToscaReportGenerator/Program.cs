using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tricentis.TCAPI;
using Tricentis.TCAPIObjects.Objects;
using System.Xml.Serialization;

namespace BKR.Test.ToscaReportGenerator
{
    class Program
    {
        private const string EXLISTROOTNODE = "ExecutionLists";
          
        static void Main(string[] args)
        {           
            TCWorkspace myWorkspace = InitializeAPIAndOpenWorkSpace();
            TCFolder root = GetRootFolderForExecLists(myWorkspace);
            if (root != null)
            {
                FileGenerator fg = new FileGenerator(root.Name, ToscaReportGenerator.Default.filenameXml, ToscaReportGenerator.Default.filenameXslt, ToscaReportGenerator.Default.foldername);
                fg.GetSubFolders(root);
                fg.GenerateXML();
            }
            CloseWorkspaceAndAPI();
            Console.ReadLine(); 
        }

        private static TCWorkspace InitializeAPIAndOpenWorkSpace()
        {
            Console.WriteLine("Connecting to Tosca Workspace");
            TCAPI api = TCAPI.CreateInstance();

            //Path to the Workspace to open 
            String workspacePath = ToscaReportGenerator.Default.workspacePath;

            //Credentials for the login 
            String loginName = ToscaReportGenerator.Default.loginName;
            String loginPassword = ToscaReportGenerator.Default.loginPassword;
            //Open Workspace and retrieve TCWorkspace Object 
            TCWorkspace myWorkspace = TCAPI.Instance.OpenWorkspace(workspacePath, loginName, loginPassword);
            return myWorkspace;
        }

        private static TCFolder GetRootFolderForExecLists(TCWorkspace myWorkspace)
        {
            //Retrieve the project
            TCProject project = myWorkspace.GetProject();

            //SearchFor ExecutionListFolder
            TCFolder execListFolder = null;
            foreach (OwnedItem item in project.Items)
            {
                if (item.Name == EXLISTROOTNODE)
                {
                    execListFolder = item as TCFolder;
                    break;
                }
            }
            if (execListFolder == null)
            {
                Console.WriteLine("ExecutionList TopFolder was not found");
                return null;
            }

            TCFolder execStartFolder = null;
            foreach (OwnedItem subfolder in execListFolder.Items)
            {
                if (String.Equals(ToscaReportGenerator.Default.mainExList,subfolder.Name,StringComparison.InvariantCultureIgnoreCase ))
                {
                    execStartFolder = subfolder as TCFolder;
                    break;
                }
            }

            if (execStartFolder == null)
            {
                Console.WriteLine(String.Format("ExecutionFolder {0} was not found", ToscaReportGenerator.Default.mainExList));
                return null;
            }

            return execStartFolder;
        }

        private static void CloseWorkspaceAndAPI()
        {
            Console.WriteLine("Closing Tosca Workspace");
            //close the workspace
            TCAPI.Instance.CloseWorkspace();
            //close the instance
            TCAPI.CloseInstance();
        }


    }
}
