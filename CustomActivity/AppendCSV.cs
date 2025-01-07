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
            // lấy đường dẫn tệp
            string filename = Filename.Get(context);
            filename = Environment.ExpandEnvironmentVariables(filename);
            // lấy dữ liệu datatable
            DataTable data = Data.Get(context);
            string delimiter = Delimeter.Get(context) ?? ",";

            // đảm bảo thư mục tồn tại
            string directory = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

           
            bool fileExists = File.Exists(filename);

            // dữ liệu sẽ được ghi thêm vào cuối chứ không phải đè
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
