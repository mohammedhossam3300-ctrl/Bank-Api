# Bank Management System Infrastructure
terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# Resource Group
resource "azurerm_resource_group" "bank_rg" {
  name     = var.resource_group_name
  location = var.location

  tags = {
    Environment = var.environment
    Project     = "BankManagementSystem"
  }
}

# Azure Kubernetes Service
resource "azurerm_kubernetes_cluster" "bank_aks" {
  name                = "${var.cluster_name}-aks"
  location            = azurerm_resource_group.bank_rg.location
  resource_group_name = azurerm_resource_group.bank_rg.name
  dns_prefix          = "${var.cluster_name}-aks"

  default_node_pool {
    name       = "default"
    node_count = var.node_count
    vm_size    = var.node_vm_size
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "azure"
  }

  tags = {
    Environment = var.environment
    Project     = "BankManagementSystem"
  }
}

# Azure Container Registry
resource "azurerm_container_registry" "bank_acr" {
  name                          = "${var.cluster_name}acr"
  resource_group_name           = azurerm_resource_group.bank_rg.name
  location                      = azurerm_resource_group.bank_rg.location
  sku                           = "Standard"
  admin_enabled                 = false
  public_network_access_enabled = false

  tags = {
    Environment = var.environment
    Project     = "BankManagementSystem"
  }
}

# Assign AKS managed identity permission to pull from ACR
resource "azurerm_role_assignment" "aks_acr_pull" {
  scope              = azurerm_container_registry.bank_acr.id
  role_definition_name = "AcrPull"
  principal_id       = azurerm_kubernetes_cluster.bank_aks.identity[0].principal_id
}

# Azure SQL Database
resource "azurerm_mssql_server" "bank_sql_server" {
  name                         = "${var.cluster_name}-sql-server"
  resource_group_name          = azurerm_resource_group.bank_rg.name
  location                     = azurerm_resource_group.bank_rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password
  public_network_access_enabled = false

  tags = {
    Environment = var.environment
    Project     = "BankManagementSystem"
  }
}

resource "azurerm_mssql_database" "bank_database" {
  name           = "BankDB"
  server_id      = azurerm_mssql_server.bank_sql_server.id
  collation      = "SQL_Latin1_General_CP1_CI_AS"
  license_type   = "LicenseIncluded"
  max_size_gb    = 20
  sku_name       = "S1"

  tags = {
    Environment = var.environment
    Project     = "BankManagementSystem"
  }
}

# Azure Key Vault
# SECURITY: rbac_authorization_enabled is set to true for role-based access control
# This ensures only authorized identities (users/services) can access secrets
# Protects against unauthorized resource access (CWE-668)
resource "azurerm_key_vault" "bank_kv" {
  name                            = "${var.cluster_name}-kv"
  location                        = azurerm_resource_group.bank_rg.location
  resource_group_name             = azurerm_resource_group.bank_rg.name
  tenant_id                       = data.azurerm_client_config.current.tenant_id
  sku_name                        = "standard"
  purge_protection_enabled        = true
  soft_delete_retention_days      = 90
  public_network_access_enabled   = false
  rbac_authorization_enabled      = true

  # Legacy access policies are disabled when RBAC is enabled
  # All access control is now managed through Azure RBAC role assignments
  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Get",
    ]

    secret_permissions = [
      "Get",
      "Set",
    ]
  }

  tags = {
    Environment = var.environment
    Project     = "BankManagementSystem"
  }
}

data "azurerm_client_config" "current" {}

# Store secrets in Key Vault
resource "azurerm_key_vault_secret" "sql_connection_string" {
  name         = "sql-connection-string"
  value        = "Server=${azurerm_mssql_server.bank_sql_server.fully_qualified_domain_name};Database=${azurerm_mssql_database.bank_database.name};User ID=${var.sql_admin_username};Password=${var.sql_admin_password};Encrypt=true;TrustServerCertificate=false;"
  key_vault_id = azurerm_key_vault.bank_kv.id
}

resource "azurerm_key_vault_secret" "jwt_secret" {
  name         = "jwt-secret-key"
  value        = var.jwt_secret_key
  key_vault_id = azurerm_key_vault.bank_kv.id
}