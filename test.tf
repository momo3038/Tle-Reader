# Configure the Azure Provider
provider "azurerm" {
  # whilst the `version` attribute is optional, we recommend pinning to a given version of the Provider
  version = "=1.38.0"
}
resource "azurerm_resource_group" "myfirsttest" {
  name     = "terraform-signalr"
  location = "West US"
}

resource "azurerm_signalr_service" "myfirsttest" {
  name                = "tfex-signalr"
  location            = "${azurerm_resource_group.myfirsttest.location}"
  resource_group_name = "${azurerm_resource_group.myfirsttest.name}"

  sku {
    name     = "Free_F1"
    capacity = 1
  }

  cors {
    allowed_origins = ["http://www.example.com"]
  }

  features {
    flag  = "ServiceMode"
    value = "Default"
  }
}
