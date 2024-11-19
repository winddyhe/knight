using System;

namespace Knight.Tools.ExcelExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ExcelExporter rExcelPorter = new ExcelExporter();
            rExcelPorter.Initialize("./ExportConfig.json");
        }
    }
}
