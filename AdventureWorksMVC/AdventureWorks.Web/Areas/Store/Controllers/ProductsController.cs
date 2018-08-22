using System;
using System.Web.Mvc;
using System.Threading.Tasks;
using AdventureWorks.Model.Models;
using AdventureWorks.Data.Repository;
using AdventureWorks.Data.Entity.Tables;
using AdventureWorks.Web.Areas.Store.Controllers.Base;
using AdventureWorks.Web.Areas.Store.Models;
using ProductModel = AdventureWorks.Model.Models.ProductModel;

namespace AdventureWorks.Web.Areas.Store.Controllers
{
    //[Authorize(Roles = "Admin, Manager")]
    public class ProductsController : BaseController
    {
        public ActionResult Index()
        {
            //if (!ControllerContext.RouteData.DataTokens.ContainsKey("area"))
            //    ControllerContext.RouteData.DataTokens.Add("area", "Store");

            return View();
        }

        // GET: Store/Products/Bikes
        public ActionResult Bikes(int subcategory)
        {
            ProductsModel model = new ProductsModel()
            {
                SubcategoryID = subcategory
            };

            switch(subcategory)
            {
                case 1:
                    model.ViewTitle = "Mountain Bikes";
                    break;
                case 2:
                    model.ViewTitle = "Road Bikes";
                    break;
                case 3:
                    model.ViewTitle = "Touring Bikes";
                    break;
                default:
                    throw new ArgumentException();
            }

            return View("Bikes", model);
        }

        // GET: Store/Products/MountainBikes
        //public ActionResult MountainBikes()
        //{
        //    if (!ControllerContext.RouteData.DataTokens.ContainsKey("area"))
        //        ControllerContext.RouteData.DataTokens.Add("area", "Store");

        //    ProductsViewModel model = new ProductsViewModel()
        //    {
        //        ViewTitle = "Mountain Bikes",
        //        Subcategory = 1
        //    };

        //    return View("Bikes", model);
        //}

        public async Task<ActionResult> Display(int id)
        {
            RepositoryProducts repo = new RepositoryProducts();

            ProductModel model = await Task.Run(() => repo.GetModelById(id));

            return View(model);
        }

        public async Task<FileContentResult> Photo(int id)
        {
            RepositoryProducts repo = new RepositoryProducts();

            ProductPhoto productPhoto = await Task.Run(() => repo.GetPhoto(id));

            if (productPhoto == null)
                return null;

            int fileExtPos = productPhoto.LargePhotoFileName.LastIndexOf(".");

            if (fileExtPos <= 0)
                return null;

            string contentType = string.Format("image/{0}", productPhoto.LargePhotoFileName.Substring(fileExtPos + 1, productPhoto.LargePhotoFileName.Length - fileExtPos - 1));

            return File(productPhoto.LargePhoto, contentType);
        }

        public async Task<FileContentResult> Thumbnail(int id)
        {
            RepositoryProducts repo = new RepositoryProducts();

            ProductPhoto productPhoto = await Task.Run(() => repo.GetPhoto(id));

            if (productPhoto == null)
                return null;

            int fileExtPos = productPhoto.LargePhotoFileName.LastIndexOf(".");

            if (fileExtPos <= 0)
                return null;

            string contentType = string.Format("image/{0}", productPhoto.LargePhotoFileName.Substring(fileExtPos + 1, productPhoto.LargePhotoFileName.Length - fileExtPos - 1));

            return File(productPhoto.ThumbNailPhoto, contentType);
        }
    }
}