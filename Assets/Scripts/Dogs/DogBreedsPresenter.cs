using System;
using Core;
using TabSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dogs
{
    public class DogBreedsPresenter : IInitializable, IDisposable
    {
        private const string BREEDS_URL = "https://dogapi.dog/api/v2/breeds?page[size]=10";
        private const string BREED_DETAIL_URL = "https://dogapi.dog/api/v2/breeds/{0}";
        private const string TAG_LIST = "dogs_list";
        private const string TAG_DETAIL = "dogs_detail";

        private readonly DogBreedsModel _model;
        private readonly DogBreedsView _view;
        private readonly RequestQueue _queue;
        private readonly TabModel _tabModel;
        private readonly CompositeDisposable _disposables = new();

        private bool _listLoaded;

        public DogBreedsPresenter(
            DogBreedsModel model,
            DogBreedsView view,
            RequestQueue queue,
            TabModel tabModel)
        {
            _model = model;
            _view = view;
            _queue = queue;
            _tabModel = tabModel;
        }

        public void Initialize()
        {
            _tabModel.ActiveTab
                .Subscribe(OnTabChanged)
                .AddTo(_disposables);
            
            _model.IsLoadingList.Subscribe(loading =>
            {
                if (loading) _view.ShowListLoading();
            }).AddTo(_disposables);

            _model.IsLoadingDetail.Subscribe(loading =>
            {
                if (loading) _view.ShowDetailLoading();
                else _view.HideDetailLoading();
            }).AddTo(_disposables);

            _model.Breeds.Subscribe(breeds =>
            {
                if (breeds != null) _view.ShowBreeds(breeds);
            }).AddTo(_disposables);

            _model.SelectedBreed.Subscribe(breed =>
            {
                if (breed != null) _view.ShowBreedPopup(breed);
            }).AddTo(_disposables);

            _model.Error.Subscribe(err =>
            {
                if (!string.IsNullOrEmpty(err)) _view.ShowError(err);
            }).AddTo(_disposables);
            
            _view.OnBreedSelected += OnBreedSelected;
        }

        // ── Tab lifecycle ─────────────────────────────────────────────────────

        private void OnTabChanged(TabType tab)
        {
            if (tab == TabType.Dogs)
            {
                if (!_listLoaded)
                    EnqueueListRequest();
            }
            else
            {
                _queue.CancelByTag(TAG_LIST);
                _queue.CancelByTag(TAG_DETAIL);
                _model.SetLoadingList(false);
                _model.ClearSelectedBreed();
            }
        }

        // ── Breed list ────────────────────────────────────────────────────────

        private void EnqueueListRequest()
        {
            _model.SetLoadingList(true);

            var request = new WebRequest<BreedsResponse>(
                id: $"{TAG_LIST}_{DateTime.UtcNow.Ticks}",
                url: BREEDS_URL,
                deserializer: JsonUtility.FromJson<BreedsResponse>,
                onSuccess: data =>
                {
                    if (data?.data != null && data.data.Count > 0)
                    {
                        _listLoaded = true;
                        _model.SetBreeds(data.data);
                    }
                    else
                    {
                        _model.SetError("Список пород пуст.");
                    }
                },
                onError: err => _model.SetError(err)
            );

            _queue.Enqueue(request);
        }

        // ── Breed detail ──────────────────────────────────────────────────────

        private void OnBreedSelected(BreedData breed)
        {
            _queue.CancelByTag(TAG_DETAIL);
            _model.ClearSelectedBreed();

            _model.SetLoadingDetail(true);

            var url = string.Format(BREED_DETAIL_URL, breed.id);

            var request = new WebRequest<BreedDetailResponse>(
                id: $"{TAG_DETAIL}_{breed.id}_{DateTime.UtcNow.Ticks}",
                url: url,
                deserializer: JsonUtility.FromJson<BreedDetailResponse>,
                onSuccess: data =>
                {
                    if (data?.data != null)
                    {
                        _model.SetSelectedBreed(data.data);
                    }
                    else
                    {
                        _model.SetError("Не удалось загрузить информацию о породе.");
                    }
                },
                onError: err =>
                {
                    _model.SetError(err);
                    _model.ClearSelectedBreed();
                }
            );

            _queue.Enqueue(request);
        }

        public void Dispose()
        {
            _view.OnBreedSelected -= OnBreedSelected;
            _disposables?.Dispose();
        }
    }
}
