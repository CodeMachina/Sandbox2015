using ProductNames.DataManagement.Contract;
using ProductNames.DataManagement.Contract.Commands;
using ProductNames.Models.ViewModel.ProductName;
using Microsoft.AspNet.Mvc;

namespace ProductNames.ControllersSingleAction {
    [Route("/")]
    public class CreateOrUpdate : Controller {
        private readonly IHandleProductNameData productNameData;

        public CreateOrUpdate(IHandleProductNameData productNameData) {
            this.productNameData = productNameData;
        }

        [HttpPut("{productId:guid}")]
        public IActionResult Execute(CreateOrUpdateInput input) {
            productNameData.CreateOrUpdateHandler(new CreateOrUpdateCommand {
                ProductId = input.ProductId,
                ProductName = input.ProductName
            });

            return Json(new { success = "PUT" });
        }
    }
}
