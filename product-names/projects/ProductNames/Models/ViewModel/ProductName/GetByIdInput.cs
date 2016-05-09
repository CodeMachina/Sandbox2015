using Microsoft.AspNet.Mvc;
using System;

namespace ProductNames.Models.ViewModel.ProductName {
    public class GetByIdInput {
        [FromRoute]
        public Guid ProductId { get; set; }
    }
}
