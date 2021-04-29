# Azure Blob Manipulation using ASP.NET Core MVC

## Overview

This sample demonstrates the how to do simple manipulation of uploading, listing and downloading files from Azure Blob Storage.

This sample application also aims to demonstrate how use logging using ILogger, deploy to Azure App Service and to manage and use secrets on Azure Key Vault using *Key Vault References*.

## Status of Development

The current status of development along with roadmap is as follows:

- [x] Upload, list and download blobs
- [x] Application scaffolding for logging
- [ ] Logging to Azure Blob Storage and Log Analytics Workspace
- [ ] Azure Key Vault integration

Watch this project on Github to keep track on updates and Star this repository if you like it. You can also fork this project to modify and use to your liking.

## Further Explanation

A detailed discussion on the above topics are available at the following blog location:

* Yet to be published.

## Installing This Project

### Pre-requisites

The following items are required for building and installing this sample:

* .NET Core CLI Version 3.1
* Azure CLI (Latest version)
* Azure Subscription (with rights to create and manipulate resources)

This sample uses the following Azure resources:

* Azure Storage Account V2
* Azure App Service

The installation instruction assumes that you are on Linux, however, with minimal modification this can also be executed on Windows. The process mentioned here uses Azure CLI, however, the same activity can be done through Azure Portal or Azure Power Shell scripts.

Before you start, ensure that you are logged in on Azure CLI. To confirm, execute `az account show --output table` command. You should see an output as shown below:

<pre>az account show --output table
EnvironmentName    HomeTenantId                          IsDefault    Name                      State    TenantId
-----------------  ------------------------------------  -----------  ------------------------  -------  ------------------------------------
AzureCloud         xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx  True         Demo Subscription         Enabled  xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx</pre>

If the result shows that you need to login, execute `az login` command to log into Azure. Ensure that the subscription that you want to work on is set up as the *default* subscription. In order to set a subscription as default, execute `az account set --subscription <subscription_id>` command. Replace *&lt;subscription_id&gt;* with the *id* of the subscription.

### Creating Resources

First, a resource group needs to be created to host the components. This can be done by executing `az group create` command. A sample command along with expected output is given below:

<pre>&gt; az group create --location &quot;centralindia&quot; --name &quot;webapp-storage-blob-demo&quot; 
{
  &quot;id&quot;: &quot;/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/webapp-storage-blob-demo&quot;,
  &quot;location&quot;: &quot;centralindia&quot;,
  &quot;managedBy&quot;: null,
  &quot;name&quot;: &quot;webapp-storage-blob-demo&quot;,
  &quot;properties&quot;: {
    &quot;provisioningState&quot;: &quot;Succeeded&quot;
  },
  &quot;tags&quot;: null,
  &quot;type&quot;: &quot;Microsoft.Resources/resourceGroups&quot;
}</pre>

Next, we need a storage account to store the BLOBS. This can be created by executing `az storage account create` command. A sample command along with expected output is given below: 

<pre>&gt; az storage account create --name &quot;webappblobdemost&quot; --resource-group &quot;webapp-storage-blob-demo&quot; --location &quot;centralindia&quot; --kind &quot;StorageV2&quot; --sku &quot;Standard_LRS&quot; 
{/ Finished ..
  &quot;accessTier&quot;: &quot;Hot&quot;,
  &quot;allowBlobPublicAccess&quot;: null,
  &quot;allowSharedKeyAccess&quot;: null,
  &quot;azureFilesIdentityBasedAuthentication&quot;: null,
  &quot;blobRestoreStatus&quot;: null,
  &quot;creationTime&quot;: &quot;2021-04-28T14:54:04.298592+00:00&quot;,

  ... snip ...

  &quot;primaryEndpoints&quot;: {
    &quot;blob&quot;: &quot;https://webappblobdemost.blob.core.windows.net/&quot;,
    &quot;dfs&quot;: &quot;https://webappblobdemost.dfs.core.windows.net/&quot;,
    &quot;file&quot;: &quot;https://webappblobdemost.file.core.windows.net/&quot;,
    &quot;internetEndpoints&quot;: null,
    &quot;microsoftEndpoints&quot;: null,
    &quot;queue&quot;: &quot;https://webappblobdemost.queue.core.windows.net/&quot;,
    &quot;table&quot;: &quot;https://webappblobdemost.table.core.windows.net/&quot;,
    &quot;web&quot;: &quot;https://webappblobdemost.z29.web.core.windows.net/&quot;
  },
  &quot;primaryLocation&quot;: &quot;centralindia&quot;,
  &quot;privateEndpointConnections&quot;: [],
  &quot;provisioningState&quot;: &quot;Succeeded&quot;,
  &quot;resourceGroup&quot;: &quot;webapp-storage-blob-demo&quot;,
  &quot;routingPreference&quot;: null,
  &quot;secondaryEndpoints&quot;: null,
  &quot;secondaryLocation&quot;: null,
  &quot;sku&quot;: {
    &quot;name&quot;: &quot;Standard_LRS&quot;,
    &quot;tier&quot;: &quot;Standard&quot;
  },
  &quot;statusOfPrimary&quot;: &quot;available&quot;,
  &quot;statusOfSecondary&quot;: null,
  &quot;tags&quot;: {},
  &quot;type&quot;: &quot;Microsoft.Storage/storageAccounts&quot;
}
</pre>

Next, we will create a storage container named `democontainer` in the storage account. In case you wish to use a container with some other name, then refer to subsequent section for updating the configuration. This can either be done through *Storage Explorer* or by executing `az storage container create` command. A sample command along with expected output is given below:

<pre>&gt; az storage container create --name &quot;democontainer&quot; --account-name &quot;webappblobdemost&quot;                      
<font color="#B58900">There are no credentials provided in your command and environment, we will query for the account key inside your storage account. </font>
<font color="#B58900">Please provide --connection-string, --account-key or --sas-token as credentials, or use `--auth-mode login` if you have required RBAC roles in your command. For more information about RBAC roles in storage, visit https://docs.microsoft.com/en-us/azure/storage/common/storage-auth-aad-rbac-cli. </font>
<font color="#B58900">Setting the corresponding environment variables can avoid inputting credentials in your command. Please use --help to get more information.</font>
{
  &quot;created&quot;: true
}
</pre>

Next, we will create an *App Service Plan* where the web application will be hosted. for the purpose of this sample, we recommend *Free* tier. However, you are free to use any other tier as per your requirement. This can be created by executing `az appservice plan create` CLI command. A sample command along with expected output is given below:

<pre>&gt; az appservice plan create --name &quot;demoappserviceplan&quot; --resource-group &quot;webapp-storage-blob-demo&quot; --location &quot;centralindia&quot; --sku FREE
{- Starting ..
  &quot;freeOfferExpirationTime&quot;: null,
  &quot;geoRegion&quot;: &quot;Central India&quot;,
  &quot;hostingEnvironmentProfile&quot;: null,
  &quot;hyperV&quot;: false,
  &quot;id&quot;: &quot;/subscriptions/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/resourceGroups/webapp-storage-blob-demo/providers/Microsoft.Web/serverfarms/demoappserviceplan&quot;,
  &quot;isSpot&quot;: false,
  &quot;isXenon&quot;: false,
  &quot;kind&quot;: &quot;app&quot;,
  &quot;location&quot;: &quot;centralindia&quot;,
  &quot;maximumElasticWorkerCount&quot;: 1,
  &quot;maximumNumberOfWorkers&quot;: 0,
  &quot;name&quot;: &quot;demoappserviceplan&quot;,
  &quot;numberOfSites&quot;: 0,
  &quot;perSiteScaling&quot;: false,
  &quot;provisioningState&quot;: &quot;Succeeded&quot;,
  &quot;reserved&quot;: false,
  &quot;resourceGroup&quot;: &quot;webapp-storage-blob-demo&quot;,
  &quot;sku&quot;: {
    &quot;capabilities&quot;: null,
    &quot;capacity&quot;: 0,
    &quot;family&quot;: &quot;F&quot;,
    &quot;locations&quot;: null,
    &quot;name&quot;: &quot;F1&quot;,
    &quot;size&quot;: &quot;F1&quot;,
    &quot;skuCapacity&quot;: null,
    &quot;tier&quot;: &quot;Free&quot;
  },
  &quot;spotExpirationTime&quot;: null,
  &quot;status&quot;: &quot;Ready&quot;,
  &quot;subscription&quot;: &quot;xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx&quot;,
  &quot;tags&quot;: null,
  &quot;targetWorkerCount&quot;: 0,
  &quot;targetWorkerSizeId&quot;: 0,
  &quot;type&quot;: &quot;Microsoft.Web/serverfarms&quot;,
  &quot;workerTierName&quot;: null
}
</pre>

Next, we will create a Web Application in the App Service which we created earlier to host the sample application. This can be accomplished by executing `az webapp create` CLI command. A sample command along with expected output is given below:

<pre>&gt; az webapp create --name &quot;webapp-storage-blob-demo&quot; --plan &quot;demoappserviceplan&quot; --resource-group &quot;webapp-storage-blob-demo&quot; --runtime &quot;DOTNETCORE|3.1&quot; --assign-identity &quot;[system]&quot;
<font color="#B58900">Webapp &apos;webapp-storage-blob-demo&apos; already exists. The command will use the existing app&apos;s settings.</font>
{- Starting ..
  &quot;availabilityState&quot;: &quot;Normal&quot;,
  &quot;clientAffinityEnabled&quot;: true,
  &quot;clientCertEnabled&quot;: false,
  &quot;clientCertExclusionPaths&quot;: null,
  &quot;cloningInfo&quot;: null,
  &quot;containerSize&quot;: 0,
  &quot;dailyMemoryTimeQuota&quot;: 0,
  &quot;defaultHostName&quot;: &quot;webapp-storage-blob-demo.azurewebsites.net&quot;,
  &quot;enabled&quot;: true,

  ... snip ...

  &quot;slotSwapStatus&quot;: null,
  &quot;state&quot;: &quot;Running&quot;,
  &quot;suspendedTill&quot;: null,
  &quot;tags&quot;: null,
  &quot;targetSwapSlot&quot;: null,
  &quot;trafficManagerHostNames&quot;: null,
  &quot;type&quot;: &quot;Microsoft.Web/sites&quot;,
  &quot;usageState&quot;: &quot;Normal&quot;
}
</pre>

### Configuration, Build and Deployment

First, we need to set the connection string for the Storage account. We will set this in the Web Application's configuration section, which is provided by *Webapp* service. Simplest way is to execute the `az webapp config connection-string set` command with appropriate modifications. This is a one-time activity though. A sample command along with expected output is given below:

<pre>&gt; az webapp config connection-string set --connection-string-type &quot;Custom&quot; --name &quot;webapp-storage-blob-demo&quot; --resource-group &quot;webapp-storage-blob-demo&quot; --settings BLOB_CONN_STRING=`az storage account show-connection-string --name &quot;webappblobdemost&quot; --resource-group &quot;webapp-storage-blob-demo&quot; --query &quot;connectionString&quot; --output tsv` 
{
  &quot;BLOB_CONN_STRING&quot;: {
    &quot;type&quot;: &quot;Custom&quot;,
    &quot;value&quot;: &quot;DefaultEndpointsProtocol=https;EndpointSuffix=core.windows.net;AccountName=webappblobdemost;AccountKey=qxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx+23jKa5A==&quot;
  }
}
</pre>

Next, we need to build and create a *zip* file of the published binaries. The following script shows the commands to build. The `zip` command may be replaced with any other utility.

```bash
cd BlobDemo
mkdir build
dotnet publish --output build 
cd build
zip -r ../build.zip *
cd ..
```

Once build is complete, execute the `az webapp deployment source config-zip` command to deploy the code as shown below:

<pre>&gt; az webapp deployment source config-zip --src build.zip --name &quot;webapp-storage-blob-demo&quot; --resource-group &quot;webapp-storage-blob-demo&quot;
<font color="#B58900">Getting scm site credentials for zip deployment</font>
<font color="#B58900">Starting zip deployment. This operation can take a while to complete ...</font>
<font color="#B58900">Deployment endpoint responded with status code 202</font>
{
  &quot;active&quot;: true,
  &quot;author&quot;: &quot;N/A&quot;,
  &quot;author_email&quot;: &quot;N/A&quot;,
  &quot;complete&quot;: true,
  &quot;deployer&quot;: &quot;ZipDeploy&quot;,
  &quot;end_time&quot;: &quot;2021-04-28T23:31:05.8822483Z&quot;,
  &quot;id&quot;: &quot;68ea6a11cd0e407da73db30cd5a51baf&quot;,
  &quot;is_readonly&quot;: true,
  &quot;is_temp&quot;: false,
  &quot;last_success_end_time&quot;: &quot;2021-04-28T23:31:05.8822483Z&quot;,
  &quot;log_url&quot;: &quot;https://webapp-storage-blob-demo.scm.azurewebsites.net/api/deployments/latest/log&quot;,
  &quot;message&quot;: &quot;Created via a push deployment&quot;,
  &quot;progress&quot;: &quot;&quot;,
  &quot;provisioningState&quot;: &quot;Succeeded&quot;,
  &quot;received_time&quot;: &quot;2021-04-28T23:31:01.7975743Z&quot;,
  &quot;site_name&quot;: &quot;webapp-storage-blob-demo&quot;,
  &quot;start_time&quot;: &quot;2021-04-28T23:31:02.1275465Z&quot;,
  &quot;status&quot;: 4,
  &quot;status_text&quot;: &quot;&quot;,
  &quot;url&quot;: &quot;https://webapp-storage-blob-demo.scm.azurewebsites.net/api/deployments/latest&quot;
}
</pre>

### Cleanup

Execute the following command to delete the resource group and thus cleaning up the entire deployment.

```bash
az group delete --name "webapp-storage-blob-demo"
```
