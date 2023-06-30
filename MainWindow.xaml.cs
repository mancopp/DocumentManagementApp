using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using DocumentManagementApp.Models;
using CsvHelper;
using System.Globalization;
using DocumentManagementApp.Models.Entities;
using System.Linq;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;

namespace DocumentManagementApp
{
    public partial class MainWindow : Window
    {
        private DocumentManager _documentManager;
        private List<Document> _documentsList;
        private List<DocumentItem> _documentItemsList;

        public MainWindow()
        {
            InitializeComponent();
            _documentManager = new DocumentManager("Data Source=managmentapp.db");
            RefreshDocumentData();
        }

        private void RefreshDocumentData()
        {
            dgDocuments.ItemsSource = _documentManager.GetJoinedDocuments();
        }

        private async Task HandleCSVFileAsync<T>()
        {
            var getfile = GetCsvFileName();
            if (getfile == null)
                return;

            var pleaseWaitDialog = new PleaseWaitDialog();
            pleaseWaitDialog.Show();

            await Task.Run(() =>
            {
                using (var reader = new StreamReader(getfile))
                {
                    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ";"
                    };

                    using (var csv = new CsvReader(reader, csvConfig))
                    {
                        try
                        {
                            var records = csv.GetRecords<T>().ToList();
                            if (typeof(T) == typeof(Document))
                            {
                                _documentsList = records.Cast<Document>().ToList();
                            }
                            else if (typeof(T) == typeof(DocumentItem))
                            {
                                _documentItemsList = records.Cast<DocumentItem>().ToList();
                            }
                        }
                        catch (HeaderValidationException)
                        {
                            MessageBox.Show("Please use the correct file format with the required headers.", "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            });

            pleaseWaitDialog.Close();
        }

        private async void btnSelectDocuments_Click(object sender, RoutedEventArgs e)
        {
            await HandleCSVFileAsync<Document>();
        }

        private async void btnSelectDocumentItems_Click(object sender, RoutedEventArgs e)
        {
            await HandleCSVFileAsync<DocumentItem>();
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            if (_documentsList == null || _documentItemsList == null)
            {
                MessageBox.Show("Please select both documents and document items files.");
                return;
            }

            var pleaseWaitDialog = new PleaseWaitDialog();
            pleaseWaitDialog.Show();

            await Task.Run(() =>
            {
                _documentManager.ImportData(_documentsList, _documentItemsList);
            });

            pleaseWaitDialog.Close();

            MessageBox.Show("File uploaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshDocumentData();
        }

        private string GetCsvFileName()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
