using ProductNames.DataManagement.Contract;
using ProductNames.DataManagement.Contract.Queries;
using ProductNames.Models.ViewModel.ProductName;
using Microsoft.AspNet.Mvc;

namespace ProductNames.ControllersSingleAction {
    [Route("/")]
    public class GetById : Controller {
        private readonly IHandleProductNameData productNameData;

        public GetById(IHandleProductNameData productNameData) {
            this.productNameData = productNameData;
        }

        [HttpGet("{productId:guid}")]
        public IActionResult Execute(GetByIdInput input) {
            var productName = productNameData.GetByIdHandler(new GetByIdQuery {
                ProductId = input.ProductId
            });

            return Json(BuildGetByIdOutputModel(productName));
        }

        private GetByIdOutput BuildGetByIdOutputModel(Models.DomainModel.ProductName productName) {
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
