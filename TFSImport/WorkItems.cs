using Microsoft.TeamFoundation.Client;

using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Data;
using System.IO;

namespace TFSImport
{
    class Workitems
    {
        public void AddFromFile(TfsTeamProjectCollection tfs, string filePath)
        {
            WorkItemTrackingHttpClient witClient = tfs.GetClient<WorkItemTrackingHttpClient>();
            string witType = "Task";
            string projectName = "Project1";

            string jsonTasksSrc = File.ReadAllText(filePath);
            string jsonTasks = "{'Tasks' : " + jsonTasksSrc + "}";

            DataSet x = JsonConvert.DeserializeObject<DataSet>(jsonTasks);

            foreach (DataRow r in x.Tables["Tasks"].Rows)
            {
                JsonPatchDocument wit = new JsonPatchDocument();

                foreach (DataColumn c in r.Table.Columns)
                {
                    string sPath = "/fields/" + c.ColumnName;
                    string sValue = r[c.ColumnName].ToString();

                    wit.Add(new JsonPatchOperation()
                    {
                        Path = sPath,
                        Operation = Operation.Add,
                        Value = sValue
                    });

                    string relation = @"{
'rel' : 'System.LinkTypes.Hierarchy-Reverse',
'url' : 'http://tfsserveraddress/tfs/CollectionName/ProjectName/_workitems/edit/12345'
}";

                    wit.Add(new JsonPatchOperation()
                    {
                        Path = "/relations/-",
                        Operation = Operation.Add,
                        Value = JToken.Parse(relation)
                    });
                }

                // adding workitem
                WorkItem w = witClient.CreateWorkItemAsync(wit, projectName, witType).Result;

                // setting as closed
                JsonPatchDocument p = new JsonPatchDocument
                {
                    new JsonPatchOperation()
                    {
                        Operation = Operation.Add,
                        Path = "/fields/System.State",
                        Value = "Closed"
                    }
                };

                _ = witClient.UpdateWorkItemAsync(p, w.Id.Value).Result;
            }
        }
    }
}
