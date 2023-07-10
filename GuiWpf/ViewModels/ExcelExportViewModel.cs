using Prism.Commands;
using PairMatching.ExcelTool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;
using PairMatching.Models;
using PairMatching.DomainModel.Services;
using PairMatching.DomainModel.BLModels;

namespace GuiWpf.ViewModels
{
    public class ExcelExportViewModel<T> : ViewModelBase, IPopupViewModle
    {
        public ExitPopupModelViewModel ExitPopupVM { get; set; }

        readonly ExcelExportingService _excelService;

        public ExcelExportViewModel(ExcelExportingService excelService)
        {
            ExitPopupVM = new ExitPopupModelViewModel(this);
            _excelService = excelService;

            Properties = new(excelService.GetPropertiesOfType<T>());
        }
        
        public ObservableCollection<PropWithText> Properties { get; init; }

        public ObservableCollection<PropWithText> SelectedProps = new();

        private ObservableCollection<T> _values;
        public ObservableCollection<T> Values
        {
            get => _values;
            set => SetProperty(ref _values, value);
        }

        private string _fileName;
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }

        private string _filePath;
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set => SetProperty(ref _isOpen, value);
        }


        private bool _isSelectedAllProps;
        public bool IsSelectedAllProps
        {
            get => _isSelectedAllProps;
            set 
            { 
                if(SetProperty(ref _isSelectedAllProps, value))
                {
                    if (_isSelectedAllProps)
                    {
                        SelectedProps = new(Properties);
                    }
                    else
                    {
                        SelectedProps.Clear();
                    }
                    SelectedProps = new(Properties);
                }
            }
        }

        DelegateCommand _SelectFilePathCommand;
        public DelegateCommand SelectFilePathCommand => _SelectFilePathCommand ??= new(
        () =>
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "Please select a folder.";
            dialog.UseDescriptionForTitle = true; // This applies to the Vista style dialog only, not the old dialog.

            if (!VistaFolderBrowserDialog.IsVistaFolderDialogSupported)
            {
                Messages.MessageBoxWarning("Your operating system doesn't support this dialog");
            }

            if ((bool)dialog.ShowDialog())
            {
                FilePath = dialog.SelectedPath;
            }
        });
        
        DelegateCommand _ExportCommand;
        public DelegateCommand ExportCommand => _ExportCommand ??= new(
            async () =>
            {   
                IsLoaded = true;
                await _excelService.Export(new ExcelFileInfo<T>
                {
                    FileName = FileName,
                    FilePath = FilePath,
                    Values = Values,
                    Properties = SelectedProps.Select(p => p.Property).ToArray(),
                    SheetName = ""
                });
                IsLoaded = false;

            }//,
            //() => Values?.Count > 0
            //    && !string.IsNullOrEmpty(FileName)
            //    && !string.IsNullOrEmpty(FilePath)
        );

        DelegateCommand<object> _SelectPropCommand;
        public DelegateCommand<object> SelectPropCommand => _SelectPropCommand ??= new(
        (propParam) =>
        {
            if (propParam is PropWithText prop)
            {
                SelectedProps.Add(prop);
            }
        });

        DelegateCommand<object> _UnselectPropCommand;
        public DelegateCommand<object> UnselectPropCommand => _UnselectPropCommand ??= new(
        (propParam) =>
        {
            if (propParam is PropWithText prop)
            {
                SelectedProps.Remove(prop);
            }
        });

        public void CloseDialog()
        {
            IsOpen = false;
        }
        
        public void Init(IEnumerable<T> participiants, bool isOpen)
        {
            Values = new(participiants);
            IsOpen = isOpen;
        }
    }
}
