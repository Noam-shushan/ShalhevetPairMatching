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

namespace GuiWpf.ViewModels
{
    public class ExcelExportViewModel<T> : ViewModelBase, IPopupViewModle
    {

        public ExitPopupModelViewModel ExitPopupVM { get; set; }

        public ExcelExportViewModel()
        {
            ExitPopupVM = new ExitPopupModelViewModel(this);
        }

        private ObservableCollection<string> _propsName;
        public ObservableCollection<string> PropsName
        {
            get => _propsName;
            set => SetProperty(ref _propsName, value);
        }

        public ObservableCollection<string> SelectedProps = new();

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


        private bool _isSelectedAllProps = true;
        public bool IsSelectedAllProps
        {
            get => _isSelectedAllProps;
            set => SetProperty(ref _isSelectedAllProps, value);
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
                var spreadSheetInfoBuilder = new SpredsheetInfoBuilder<T>(FileName, FilePath);
                var spreadSheetInfo = spreadSheetInfoBuilder
                    .AddItems(Values, "Items")
                    .AddProperties(SelectedProps.Select(propName => typeof(T).GetProperty(propName)).ToArray())
                    .Build();
                IsLoaded = true;
                var excel = new ExcelGenerator();
                await excel.Generate(spreadSheetInfo);
                IsLoaded = false;

            },
            () => Values?.Count > 0 
                && !string.IsNullOrEmpty(FileName) 
                && !string.IsNullOrEmpty(FilePath)
        );

        DelegateCommand<object> _SelectPropCommand;
        public DelegateCommand<object> SelectPropCommand => _SelectPropCommand ??= new(
        (propParam) =>
        {
            if (propParam is string prop)
            {
                SelectedProps.Add(prop);
            }
        });

        DelegateCommand<object> _UnselectPropCommand;
        public DelegateCommand<object> UnselectPropCommand => _UnselectPropCommand ??= new(
        (propParam) =>
        {
            if (propParam is string prop)
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
            if (IsSelectedAllProps)
            {
                PropsName = new(typeof(T).GetProperties().Select(x => x.Name));
            }
            IsOpen = isOpen;
            
        }
    }
}
