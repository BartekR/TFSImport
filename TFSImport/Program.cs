using Microsoft.TeamFoundation.Client;
using System;

namespace TFSImport
{
    class Program
    {
        static void Main(string[] args)
        {
            // TFS Server address, with the collection name
            Uri tfsServerUri = new Uri("http://tfsserveraddress/tfs/CollectionName");

            // connect to TFS
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsServerUri);

            //AddWorkItems(tfs);
            string filePath = @"C:\tmp\Data.json";

            Workitems mwi = new Workitems();
            mwi.AddFromFile(tfs, filePath);
        }
    }
}
