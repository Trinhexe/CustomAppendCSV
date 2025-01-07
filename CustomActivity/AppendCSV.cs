using System;
using System.Activities;
using System.ComponentModel;
using System.IO;
using System.Text;

using System.Data;
using System.Linq;
using CustomActivity;

namespace OpenRPA.Utilities
{
    [Designer(typeof(ActivityDesigner1), typeof(System.ComponentModel.Design.IDesigner))]
    
    public class AppendCSV : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> Filename { get; set; }

        [RequiredArgument]
        public InArgument<System.Data.DataTable> Data { get; set; }

        public InArgument<string> Delimeter { get; set; } = ",";

        protected override void Execute(CodeActivityContext context)
        {
            string filename = Filename.Get(context);
            filename = Environment.ExpandEnvironmentVariables(filename);

            DataTable data = Data.Get(context);
            string delimiter = Delimeter.Get(context) ?? ",";

            // Ensure directory exists
            string directory = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write or append to the file
            bool fileExists = File.Exists(filename);

            using (StreamWriter writer = new StreamWriter(filename, append: true, encoding: Encoding.UTF8))
            {
                // Write header if file doesn't exist
                if (!fileExists)
                {
                    WriteHeader(data, writer, delimiter);
                }

                // Write data rows
                WriteRows(data, writer, delimiter);
            }
        }

        private void WriteHeader(DataTable data, StreamWriter writer, string delimiter)
        {
            string header = string.Join(delimiter, data.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
            writer.WriteLine(header);
        }

        private void WriteRows(DataTable data, StreamWriter writer, string delimiter)
        {
            foreach (DataRow row in data.Rows)
            {
                string line = string.Join(delimiter, row.ItemArray.Select(field => EscapeField(field?.ToString() ?? string.Empty)));
                writer.WriteLine(line);
            }
        }

        private string EscapeField(string field)
        {
            if (field.Contains(",") || field.Contains("\n") || field.Contains("\""))
            {
                field = "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            return field;
        }
    }
}
