using MyFinances.Core;
using MyFinances.Core.Dtos;
using MyFinances.Services;
using MyFinances.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyFinances.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private PaginationFilter _paginationFilter = new PaginationFilter();
        public int TotalRowsCount { get; }
        public ObservableCollection<OperationDto> Operations { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command<OperationDto> ItemTapped { get; }

        public Command FirstPageCommand { get; }

        public Command PreviousPageCommand { get; }

        public Command NextPageCommand { get; }

        public Command LastPageCommand { get; }

        public ItemsViewModel()
        {
            Title = "Operacje";
            Operations = new ObservableCollection<OperationDto>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<OperationDto>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            DeleteItemCommand = new Command<OperationDto>(async (x) => await OnDeleteItem(x));

            FirstPageCommand = new Command(async () => await OnFirstPage());
            PreviousPageCommand = new Command(async () => await OnPreviousPage(), CanPreviousPage);
            NextPageCommand = new Command(async () =>  await OnNextPage(), CanNextPage);
            LastPageCommand = new Command(async () => await OnLastPage());
        }

        private async Task<bool> CanNextPageAsync()
        {
            var totalRecords =await OperationSqliteService.UnitOfWork.OperationRepository.OperationCount();

            var totalPages = ((double)totalRecords / (double)_paginationFilter.PageSize);
            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            return _paginationFilter.PageNumber < roundedTotalPages;
        }


        private bool CanNextPage() 
        {
            var nextPageCaller = new Func<Task<bool>>(CanNextPageAsync);
            var asyncResult = nextPageCaller.BeginInvoke(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            var nextPageResult = nextPageCaller.EndInvoke(asyncResult);

            return nextPageResult.Result;
        }

        private bool CanPreviousPage()
        {
            return _paginationFilter.PageNumber > 1;
        }

        private async Task<int> LastPageAsync()
        {
            var totalRecords = await OperationSqliteService.UnitOfWork.OperationRepository.OperationCount();

            var totalPages = ((double)totalRecords / (double)_paginationFilter.PageSize);
            var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

            return roundedTotalPages;
        }

        private async Task OnLastPage()
        {
            var lastPageCaller = new Func<Task<int>>(LastPageAsync);
            var asyncResult = lastPageCaller.BeginInvoke(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            var lastPageResult = lastPageCaller.EndInvoke(asyncResult);

            _paginationFilter.PageNumber = lastPageResult.Result;

            await ExecuteLoadItemsCommand();
        }

        private async Task OnNextPage()
        {
            _paginationFilter.PageNumber++;
            await ExecuteLoadItemsCommand();
        }

        private async Task OnPreviousPage()
        {
            _paginationFilter.PageNumber--;
            await ExecuteLoadItemsCommand();
        }

        private async Task OnFirstPage()
        {
            _paginationFilter.PageNumber = 1;
            await ExecuteLoadItemsCommand();
        }

        private async Task OnDeleteItem(OperationDto operation)
        {
            if (operation == null)
                return;

            var dialog = await Shell.Current.DisplayAlert("Usuwanie!", $"Czy na pewno chcesz usunąć operację {operation.Name}?", "Tak", "Nie");

            if (!dialog)
                return;

            var response = await OperationService.DeleteAsync(operation.Id);

            if (!response.IsSuccess)
                await ShowErrorAlert(response);

            await ExecuteLoadItemsCommand();
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                var response = await OperationService.GetAsync(_paginationFilter);

                if (!response.IsSuccess)
                    await ShowErrorAlert(response);

                Operations.Clear();

                foreach (var item in response.Data)
                {
                    Operations.Add(item);
                }

                PreviousPageCommand.ChangeCanExecute();
                NextPageCommand.ChangeCanExecute();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Wystąpił Błąd!", ex.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(OperationDto operation)
        {
            if (operation == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={operation.Id}");
        }
    }
}