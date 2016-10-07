using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Tests.Daterpillar.Helper
{
    public class XmlValidator
    {
        public XmlValidator()
        {
            _schemaSet = new XmlSchemaSet();
            _errorList = new List<ValidationEventArgs>();

            LoadXsdFiles();
        }

        public bool XmlDocIsValid
        {
            get { return _errorList.Count == 0; }
        }

        public void Load(string xml)
        {
            Load(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
        }

        public void Load(Stream stream)
        {
            using (stream)
            {
                _errorList.Clear();

                var settings = new XmlReaderSettings();
                settings.Schemas = _schemaSet;
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.ValidationEventHandler += HandleValidationEvent;

                using (var reader = XmlReader.Create(stream, settings))
                {
                    while (reader.Read()) ;
                }
            }
        }

        public void Load(FileInfo file)
        {
            Load(file.OpenRead());
        }

        public string GetErrorLog()
        {
            var message = new StringBuilder();

            foreach (var error in _errorList)
            {
                message.AppendLine($"{error.Severity}: {error.Message}");
            }

            return message.ToString();
        }

        protected void HandleValidationEvent(object sender, ValidationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"{e.Severity}: {e.Message}");
            _errorList.Add(e);
        }

        #region Private Members

        private XmlSchemaSet _schemaSet;

        private IList<ValidationEventArgs> _errorList;

        private void LoadXsdFiles()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var file in new DirectoryInfo(baseDirectory).GetFiles("*.xsd", SearchOption.AllDirectories))
            {
                _schemaSet.Add(XmlSchema.Read(file.OpenRead(), HandleValidationEvent));
            }
        }

        #endregion Private Members
    }
}