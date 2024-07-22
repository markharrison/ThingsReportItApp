# ThingsReportItApp

Example response for AppDev Challenge <https://markharrison.io/appdev-challenge/>

Works as a PWA (progressive web application)

Complete solution <https://markharrison.io/appdev-challenge/day2-complete>

![](docs/scrn1.png)


## Configuration

Environment variables / configuration 

| Key                      | Value     |  
|--------------------------|-----------| 
| ThingsDbConnectionString | Connection string for in Azure Storage  |  
| ImagesContainer          | Name of container in Azure Storage to store images  |  
| EventGridTopicEndpoint   | Endpoint for EventGrid to receive event  - see <https://github.com/markharrison/ThingsLogicAppV1> |   
| EventGridKey             | Key for EventGrid |   


## Infrastructure as Code

<https://github.com/markharrison/ThingsReportItApp/blob/main/infra/README.md>


## Package 

<https://github.com/markharrison/ThingsReportItApp/pkgs/container/thingsreportitapp>
