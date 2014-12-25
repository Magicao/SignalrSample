using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalrSample.Models;
namespace SignalrSample.Controllers
{
    public class ControllerBase : Controller 
    {
        protected JsonResult DataView(Func<bool> successful)
        {
            try
            {
                return Json(new ResultBase { IsSuccessful = successful() });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        protected JsonResult DataList<TSubModel>(Func<IEnumerable<TSubModel>> listGetter, SearchBase searchBase = null)
        {
            try
            {
                var model = new ResultBase<TSubModel>()
                {
                    DataList = listGetter(),
                    IsSuccessful = true
                };

                if (searchBase != null)
                {
                    model.TotailRecords = searchBase.TotailRecords;
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        protected JsonResult DataList<TSubModel>(Func<ResultBase<TSubModel>> result, SearchBase searchBase = null)
        {
            try
            {
                var model = result;
                if (searchBase != null)
                {
                    searchBase.TotailRecords = searchBase.TotailRecords;
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        protected JsonResult ErrorResult(Exception ex)
        {
            return Json(new ResultBase {IsSuccessful = false, Message = ex.Message});
        }

        protected JsonResult DataView(Func<ResultBase> modelBaseCreator, MarkSuccessfulAutomatically successful = MarkSuccessfulAutomatically.Yes)
        {
            try
            {
                var model = modelBaseCreator();
                if (successful == MarkSuccessfulAutomatically.Yes)
                {
                    model.IsSuccessful = true;
                }

                return Json(model);
            }
            catch (Exception ex)
            {
                return ErrorResult(ex);
            }
        }

        protected enum MarkSuccessfulAutomatically
        {
            Yes,
            No
        }
    }
}