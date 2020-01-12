# Configure the Azure Provider
terraform {
  backend "azurerm" {
    storage_account_name = "githubterraformstate"
    container_name       = "deploy"
  }
}

provider "azurerm" {
  # whilst the `version` attribute is optional, we recommend pinning to a given version of the Provider
  version = "=1.38.0"
}

resource "azurerm_resource_group" "TleReader" {
  name     = "azure-functions-tle"
  location = "westus2"
}

resource "azurerm_storage_account" "TleReader" {
  name                     = "mletlereaderfunction"
  resource_group_name      = "${azurerm_resource_group.TleReader.name}"
  location                 = "${azurerm_resource_group.TleReader.location}"
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_app_service_plan" "TleReader" {
  name                = "azure-functions-tlereader-service-plan"
  location            = "${azurerm_resource_group.TleReader.location}"
  resource_group_name = "${azurerm_resource_group.TleReader.name}"
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "TleReader" {
  name                      = "TleReader"
  location                  = "${azurerm_resource_group.TleReader.location}"
  resource_group_name       = "${azurerm_resource_group.TleReader.name}"
  app_service_plan_id       = "${azurerm_app_service_plan.TleReader.id}"
  storage_connection_string = "${azurerm_storage_account.TleReader.primary_connection_string}"
}
