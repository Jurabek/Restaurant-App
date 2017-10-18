﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Restaurant.Abstractions;
using Restaurant.Abstractions.Adapters;
using Restaurant.Abstractions.Api;
using Restaurant.Common.DataTransferObjects;
using Restaurant.Abstractions.Services;

namespace Restaurant.ViewModels
{
	public class FoodsViewModel : BaseViewModel, INavigatableViewModel
	{
		public IBasketViewModel BasketViewModel { get; }

		private readonly IFoodsApi _foodsApi;
		private readonly INavigationService _navigationService;
		private readonly IFoodDetailViewModelAdapter _foodDetailViewModelAdapter;

		public FoodsViewModel(
			IBasketViewModel basketViewModel,
			IFoodsApi foodsApi,
			INavigationService navigationService,
			IFoodDetailViewModelAdapter foodDetailViewModelAdapter)
		{
			BasketViewModel = basketViewModel;
			_foodsApi = foodsApi;
			_navigationService = navigationService;
			_foodDetailViewModelAdapter = foodDetailViewModelAdapter;

			this.WhenAnyValue(x => x.SelectedFood)
				.Where(x => x != null)
				.Subscribe(async food =>
				{
					var viewModel =_foodDetailViewModelAdapter.GetFoodDetailViewModel(Foods.SingleOrDefault(f => f.Id == food.Id));
					await _navigationService.NavigateAsync(viewModel);
				});

			AddToBasket = ReactiveCommand.Create<object, object>(x =>
			{
				//basketViewModel.Orders.Add();

				return null;
			});
		}

		private ObservableCollection<FoodDto> _foods;
		public ObservableCollection<FoodDto> Foods
		{
			get => _foods;
			private set => this.RaiseAndSetIfChanged(ref _foods, value);
		}

		private FoodDto _selectedFood;

		public FoodDto SelectedFood
		{
			get => _selectedFood;
			set => this.RaiseAndSetIfChanged(ref _selectedFood, value);
		}

		public override string Title => "Foods";

		public async Task LoadFoods()
		{
			IsLoading = true;
			var foods = await _foodsApi.GetFoods();
			foreach (var food in foods)
			{
				food.Picture = "http://restaurantserverapi.azurewebsites.net" + food.Picture;
			}
			Foods = new ObservableCollection<FoodDto>(foods);
			IsLoading = false;
		}

		public ICommand Favorite { get; set; }

		public ICommand AddToBasket { get; set; }
	}
}
