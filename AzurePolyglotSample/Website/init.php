<?php

require_once "vendor/autoload.php";

use WindowsAzure\Common\ServicesBuilder;
use WindowsAzure\Common\ServiceException;
use WindowsAzure\Blob\Models\CreateContainerOptions;
use WindowsAzure\Blob\Models\PublicAccessType;

use WindowsAzure\Table\Models\Entity;
use WindowsAzure\Table\Models\EdmType;



$STORAGE_CONNECTION_STRING = 'DefaultEndpointsProtocol="http";AccountName="tr15sharemyphoto";AccountKey="4yhu8YT3Y6A3do0s+anHFAX6ZUA11V2NJJNhjmJc0iAgSLW8Xwk3QvVQn2Um+hgMmO+vGf0UFd2zOo8K63PD4w=="';


$blobRestProxy = ServicesBuilder::getInstance()->createBlobService($STORAGE_CONNECTION_STRING);
$tableRestProxy = ServicesBuilder::getInstance()->createTableService($STORAGE_CONNECTION_STRING);
$queueRestProxy = ServicesBuilder::getInstance()->createQueueService($STORAGE_CONNECTION_STRING);

?>