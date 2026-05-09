using ScriptSupport.States;
using ScriptSupport.Stores;
using ScriptSupport.Interfaces;
using ScriptSupport.Environment;

namespace ScriptSupport.Services
{
    public class FilterConfigService : IFilterConfigInterface
    {
        private readonly AppEnvironment _appEnvironment;
        private readonly FilterConfigState _filterState;
        private readonly ConfigStore _configStore;
        

        public FilterConfigService(AppEnvironment appEnvironment, FilterConfigState filterState, ConfigStore configStore)
        {
            _appEnvironment = appEnvironment;
            _filterState = filterState;
            _configStore = configStore;
        }

        public async Task<(bool Success, string Message)> LoadAsync()
        {
            try
            {
                await Task.CompletedTask;
                _filterState.SetValue(_configStore.FilterSetting);

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
