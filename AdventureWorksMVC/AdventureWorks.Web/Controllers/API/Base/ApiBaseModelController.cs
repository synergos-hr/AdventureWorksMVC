using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AdventureWorks.Data.Contracts.Repository;
using AdventureWorks.Model.Kendo;
using AutoMapper;

namespace AdventureWorks.Web.Controllers.API.Base
{
    public class ApiBaseModelController<TModel, TViewModel, TRepository> : ApiBaseController
        where TModel : class
        where TViewModel : class
        where TRepository : IRepository<TModel, TViewModel>, new()
    {
        private readonly string _keyFieldName;
        private readonly string _defaultSortFieldName;
        private readonly IList<string> _includeList;

        protected TRepository _repository;

        protected ApiBaseModelController(string keyFieldName, string defaultSortFieldName)
            : this(keyFieldName, defaultSortFieldName, new List<string>())
        {
        }

        protected ApiBaseModelController(string keyFieldName, string defaultSortFieldName, IList<string> includeList)
        {
            Debug.Assert(HttpContext.Current.User != null);

            _keyFieldName = keyFieldName;
            _defaultSortFieldName = defaultSortFieldName;
            _includeList = includeList;

            _repository = new TRepository();

            //_repository.SetUserId(HttpContext.Current.User.Identity.GetUserId<int>());
        }

#if DEBUG
        [HttpGet]
        public virtual GridResult<TViewModel> Test()
        {
            try
            {
                GridRequest request = new GridRequest()
                {
                    Skip = 0,
                    Take = 20
                };

                return _repository.GridList(request, _defaultSortFieldName);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new GridResult<TViewModel> { Status = "error", Message = "Error: " + ex.Message };
            }
        }
#endif

        [HttpPost]
        public virtual async Task<GridResult<TViewModel>> List(GridRequest request)
        {
            try
            {
                return await Task.Run(() => _repository.GridList(request, _defaultSortFieldName));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new GridResult<TViewModel> { Status = "error", Message = "Error: " + ex.Message };
            }
        }

        [HttpPost]
        public virtual CreateResult<TViewModel> Create(TViewModel viewModel)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return new CreateResult<TViewModel> { Status = "error", Message = "Changing of data is prohibited in demo mode." };

                TModel model = Mapper.Map<TViewModel, TModel>(viewModel);

                _repository.Insert(model);

                TViewModel viewModelChanged = Mapper.Map<TModel, TViewModel>(model);

                List<TViewModel> records = new List<TViewModel>() { viewModelChanged };

                return new CreateResult<TViewModel> { Status = "success", Records = records };
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new CreateResult<TViewModel> { Status = "error", Message = ex.Message };
            }
        }

        [HttpPost]
        public virtual SaveResult Update(TViewModel viewModel)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return new SaveResult { Status = "error", Message = "Changing of data is prohibited in demo mode." };

                object keyValue = typeof(TViewModel).GetProperty(_keyFieldName).GetValue(viewModel, null);

                TModel model = _repository.GetById(keyValue);

                Mapper.Map<TViewModel, TModel>(viewModel, model);

                _repository.Update(model);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
#if DEBUG
                return new SaveResult { Status = "error", Message = "Error: " + ex.Message + "<br/>" + ex.InnerException };
#else
                return new SaveResult { Status = "error", Message = "Error: " + ex.Message };
#endif
            }
        }

        [HttpPost]
        public virtual SaveResult Delete(TViewModel viewModel)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return new SaveResult { Status = "error", Message = "Changing of data is prohibited in demo mode." };

                object keyValue = typeof(TViewModel).GetProperty(_keyFieldName).GetValue(viewModel, null);

                TModel model = _repository.GetById(keyValue);

                Mapper.Map<TViewModel, TModel>(viewModel, model);

                _repository.Delete(model);

                return new SaveResult { Status = "success" };
            }
            catch (Exception ex)
            {
                return new SaveResult { Status = "error", Message = "Error: " + ex.Message };
            }
        }
    }
}