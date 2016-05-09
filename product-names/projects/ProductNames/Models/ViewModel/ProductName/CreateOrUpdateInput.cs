using System;

namespace ProductNames.Models.ViewModel.ProductName {
    public class CreateOrUpdateInput {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
