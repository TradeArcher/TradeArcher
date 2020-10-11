using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using TradeArcher.Core.Models;
using TradeArcher.Core.Services;
using TradeArcher.Helpers;
using TradeArcher.Views;

namespace TradeArcher.ViewModels
{
    public class ImportBackTestsViewModel : Screen
    {
        private readonly BackTestSessionImporterService _tradesImporterService;
        private StorageFile _selectedFile;
        private StorageFolder _selectedFolder;

        public ImportBackTestsViewModel(BackTestSessionImporterService tradesImporterService)
        {
            _tradesImporterService = tradesImporterService;
        }

        private string _selectedFileOrFolderPath;

        public string SelectedFileOrFolder
        {
            get => _selectedFileOrFolderPath;
            set => Set(ref _selectedFileOrFolderPath, value);
        }

        private bool _canImportTrades;
        public bool CanImportTades
        {
            get => _canImportTrades;
            set => Set(ref _canImportTrades, value);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var view = GetView() as IShellView;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            PropertyChanged += OnPropertyChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            PropertyChanged -= OnPropertyChanged;
        }

        public async void SelectFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".csv");

            _selectedFolder = null;
            _selectedFile = await picker.PickSingleFileAsync();

            SelectedFileOrFolder = _selectedFile?.Path;
        }

        public async void SelectFolderAsync()
        {
            var picker = new FolderPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            picker.FileTypeFilter.Add(".csv");

            _selectedFile = null;
            _selectedFolder = await picker.PickSingleFolderAsync();

            SelectedFileOrFolder = _selectedFolder?.Path;
        }

        public async void ImportBackTest()
        {
            if (_selectedFolder != null)
            {
                List<string> fileTypeFilter = new List<string>();
                fileTypeFilter.Add(".csv");
                var queryOptions = new QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter);
                var query = _selectedFolder.CreateFileQueryWithOptions(queryOptions);
                var queryResults = await query.GetFilesAsync();
                foreach (var file in queryResults)
                {
                    ImportFile(await file.GetBytesAsync());
                }
            }
            else
            {
                ImportFile(await _selectedFile.GetBytesAsync());
            }
        }

        private void ImportFile(byte[] file)
        {
            try
            {
                var parserType = GetFileParserType();

                if (parserType != BackTestFileParserType.NotSupported)
                {
                    if (!_tradesImporterService.Import(parserType, file))
                    {
                        // TODO: show import failure message
                    }
                    else
                    {
                        // TODO: show import success message
                    }
                }
                else
                {
                    // TODO: show import failure message due to not supported
                }
            }
            catch (NotSupportedException ex)
            {
                // TODO: show import failure message due to not supported
            }
            catch (Exception ex)
            {
                // TODO: show import failure message
            }
        }

        private BackTestFileParserType GetFileParserType()
        {
            //TODO: support other platforms that export backtesting if they exist
            return BackTestFileParserType.ThinkOrSwim;
        }

        private bool ValidateForm()
        {
            return SelectedFileOrFolder != null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CanImportTades = ValidateForm();
        }
    }
}
