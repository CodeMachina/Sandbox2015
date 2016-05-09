using System;

namespace ProductNames.DataManagement.Contract.Commands {
    public class CreateOrUpdateCommand {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
