param name string
param location string

resource swa 'Microsoft.Web/staticSites@2023-01-01' = {
  name: 'swa-${name}'
  location: location
  properties: {
    buildProperties: {
      appLocation: 'app'
    }
  }
  sku: {
    name: 'Free'
    tier: 'Free'
  }
}
