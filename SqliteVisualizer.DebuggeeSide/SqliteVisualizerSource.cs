using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace SqliteVisualizer
{
    public class SqliteVisualizerSource : VisualizerObjectSource
    {
        public override void TransferData(object target, Stream incomingData, Stream outgoingData)
        {
            var connection = (DbConnection)target;

            var deserializableObject = GetDeserializableObject(incomingData);
            Type type = null;
            if (!deserializableObject.IsBinaryFormat)
            {
                // TODO: Use deserializableObject.GetJsonStringPropertyValue() to determine type
                type = typeof(string);
            }

            var request = (string)deserializableObject.ToObject(type);

            if (request == "/tables")
            {
                var command = connection.CreateCommand();
                command.CommandText =
                @"
                    SELECT name
                    FROM sqlite_master
                    WHERE type = 'table'
                        AND name NOT LIKE 'sqlite_%'
                ";
    
                using (var reader = command.ExecuteReader())
                {
                    var tables = new List<string>();
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
    
                    Serialize(outgoingData, tables.ToArray());
                }
            }
            else if (request.StartsWith("/table/"))
            {
                // SELECT * FROM [table] LIMIT 15 OFFSET 15 * page
            }
        }
    }
}
