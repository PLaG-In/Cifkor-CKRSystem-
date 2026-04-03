using System.Collections.Generic;
using UniRx;

namespace Dogs
{
    public class DogBreedsModel
    {
        public IReadOnlyReactiveProperty<List<BreedData>> Breeds => _breeds;
        public IReadOnlyReactiveProperty<BreedData> SelectedBreed => _selectedBreed;
        public IReadOnlyReactiveProperty<bool> IsLoadingList => _isLoadingList;
        public IReadOnlyReactiveProperty<bool> IsLoadingDetail => _isLoadingDetail;
        public IReadOnlyReactiveProperty<string> Error => _error;

        private readonly ReactiveProperty<List<BreedData>> _breeds = new();
        private readonly ReactiveProperty<BreedData> _selectedBreed = new();
        private readonly ReactiveProperty<bool> _isLoadingList = new(false);
        private readonly ReactiveProperty<bool> _isLoadingDetail = new(false);
        private readonly ReactiveProperty<string> _error = new();

        public void SetLoadingList(bool v) => _isLoadingList.Value = v;
        public void SetLoadingDetail(bool v) => _isLoadingDetail.Value = v;
        public void SetBreeds(List<BreedData> breeds)
        {
            _breeds.Value = breeds;
            _isLoadingList.Value = false;
        }
        
        public void SetSelectedBreed(BreedData breed)
        {
            _selectedBreed.Value = breed;
            _isLoadingDetail.Value = false;
        }
        
        public void SetError(string err)
        {
            _error.Value = err;
            _isLoadingList.Value = false;
            _isLoadingDetail.Value = false;
        }
        
        public void ClearSelectedBreed()
        {
            _selectedBreed.Value = null;
            _isLoadingDetail.Value = false;
        }
    }
}
