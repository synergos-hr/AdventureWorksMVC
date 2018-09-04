using System;

namespace AdventureWorks.Data.Repository
{
    public class RepositoryAudits : RepositoryBase
    {
        public RepositoryAudits()
        {
        }

        public RepositoryAudits(int userId, string userName)
        {
            Context.UserId = userId;
            Context.UserName = userName;
        }

        //public GridResult<AuditEventsLogViewModel> ListEventsLog(GridRequest request, string defaultSortFieldName)
        //{
        //    try
        //    {
        //        var query = from s in Context.AuditEventsLog
        //                    select s;

        //        GridSortFilter.SortFilter sortFilter = GridSortFilter.FromRequest<AuditEventsLogViewModel>(request, defaultSortFieldName);

        //        if (!string.IsNullOrEmpty(sortFilter.Sort))
        //            query = query.OrderBy(sortFilter.Sort);

        //        string filter = sortFilter.Filter;

        //        if (!string.IsNullOrEmpty(filter))
        //            query = query.Where(filter);

        //        List<AuditEventsLog> page = query
        //            .Skip(request.Skip)
        //            .Take(request.Take)
        //            .ToList();

        //        GridResult<AuditEventsLogViewModel> result = new GridResult<AuditEventsLogViewModel>
        //        {
        //            Status = "success",
        //            TotalCount = query.Count(),
        //            Records = page.Select(Mapper.Map<AuditEventsLog, AuditEventsLogViewModel>).ToList()
        //        };

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //        return new GridResult<AuditEventsLogViewModel> { Status = "error", Message = ex.Message };
        //    }
        //}

        public void AddEvent(string eventName)
        {
            try
            {
                //AuditEventsLog log = new AuditEventsLog
                //{
                //    UserId = Context.UserId,
                //    EventName = eventName,
                //    EventDate = DateTime.Now
                //};

                //Context.AuditEventsLog.Add(log);

                //Context.Database.Log = DatabaseLog.Write;

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
