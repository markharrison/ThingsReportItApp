# ThingsReportItApp

## Infrastructure as Code using Azure CLI 

### Initialise variables

```
RG="Thingz-rg"
LOCATION="uksouth"
PLANNAME="appserviceplan"
APPNAME="thingzrep"
STORAGENAME="thingzstorage"

```

### Create a RG - if needed

```
az group create -g $RG -l $LOCATION  -o table 

```

### Create a AppService Plan - if needed

```
az appservice plan create -g $RG  \
  --name $PLANNAME \
  --is-linux \
  --number-of-workers 1 \
  --sku B1

```
   
### Get connection string to storage

```
STORAGEKEY=$(az storage account keys list -n $STORAGENAME -g $RG  -o tsv --query "[0].value" )

printf -v STORAGECS "DefaultEndpointsProtocol=https;AccountName=$STORAGENAME;AccountKey=$STORAGEKEY;EndpointSuffix=core.windows.net" 

```

### Create AppService and configure

```
az webapp create -g $RG -p $PLANNAME -n $APPNAME --runtime DOTNETCORE:6.0

az webapp stop -g $RG -n $APPNAME

az webapp config appsettings set -g $RG -n $APPNAME --settings \
 AdminPW="????????" \
 LogicAppEndpoint="????"

az webapp config connection-string set -g $RG -n $APPNAME -t custom --settings \
  ThingsStorageConnectionString=$STORAGECS


az webapp config container set -g $RG -n $APPNAME \
  --docker-custom-image-name https://ghcr.io/markharrison/thingsreportitapp:latest \
  --docker-registry-server-url https://ghcr.io \
  --docker-registry-server-user markharrison \
  --docker-registry-server-password ????

az webapp start -g $RG -n $APPNAME

```

