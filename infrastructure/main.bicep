targetScope = 'subscription'

param projectName string
param location string = 'westeurope'

resource resourceGroup 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: '${projectName}-rg'
  location: location
}

module monitoring './modules/monitoring.bicep' = {
  name: 'monitoring'
  scope: resourceGroup
  params: {
    name: projectName
    location: location
  }
}

module frontend './modules/frontend.bicep' = {
  name: 'frontend'
  scope: resourceGroup
  params: {
    name: projectName
    location: location
  }
}

module backend './modules/backend.bicep' = {
  name: 'backend'
  scope: resourceGroup
  params: {
    name: projectName
    location: location
  }
}
