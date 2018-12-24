# Filtering Service Bus Topics

## Deploying

This solution contains an ARM Template for deploying a new environment into Azure. The template and parameter file is found in the `/build` folder and can be run using the following PowerShell from the root:

```powershell
Connect-AzureRmAccount

New-AzureRmResourceGroup -Name "ExampleResourceGroup" -Location "West Europe"
New-AzureRmResourceGroupDeployment -Name ExampleDeployment -ResourceGroupName "ExampleResourceGroup" -TemplateFile build\armtemplate.json
```

## Configuration

When the deployment has finished the connection strings need to be added to each of the projects, as these are secrets they are not committed to the repo. This can be done by opening a PowerShell window in each of the project folders (eg. `/src/Sample.ServiceBusFiltering.Sender/`) and running:

```powershell
dotnet user-secrets set serviceBus:connectionString "{your value from the Azure portal}"
```
