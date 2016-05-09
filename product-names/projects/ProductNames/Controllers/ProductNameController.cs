using Microsoft.AspNet.Mvc;
using ProductNames.DataManagement.Contract;
using ProductNames.DataManagement.Contract.Commands;
using ProductNames.DataManagement.Contract.Queries;
using ProductNames.Models.ViewModel.ProductName;

namespace ProductNames.Controllers {
    [Route("/")]
    public class ProductNameController : Controller {
        private readonly IHandleProductNameData productNameData;

        public ProductNameController(IHandleProductNameData productNameData) {
            this.productNameData = productNameData;
        }

        [HttpGet]
        public IActionResult Get() {
            return View("~/Views/ReadMe.cshtml");
        }

        [HttpGet("{productId:guid}")]
        public IActionResult GetById(GetByIdInput input) {
            var productName = productNameData.GetByIdHandler(new GetByIdQuery {
                ProductId = input.ProductId
            });

            return Json(BuildGetByIdOutputModel(productName));
        }

        [HttpPut("{productId:guid}")]
        public IActionResult CreateOrUpdate(CreateOrUpdateInput input) {
            productNameData.CreateOrUpdateHandler(new CreateOrUpdateCommand {
                ProductId = input.ProductId,
                ProductName = input.ProductName
            });

            return Json(new { success = "PUT" });
        }

        private GetByIdOutput BuildGetByIdOutputModel(ProductNames.Models.DomainModel.ProductName productName) {
            if(productName == null) {
                return new GetByIdOutput();
            }

            return new GetByIdOutput {
                ProductId = productName.ProductId,
                Name = productName.Name
            };
        }
    }
}
