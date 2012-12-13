<?php

require_once "WindowsAzure\WindowsAzure.php";

use WindowsAzure\Common\ServicesBuilder;
use WindowsAzure\Common\ServiceException;
use WindowsAzure\Blob\Models\CreateContainerOptions;
use WindowsAzure\Blob\Models\PublicAccessType;
use WindowsAzure\Queue\Models\CreateQueueOptions;


$STORAGE_CONNECTION_STRING = 'DefaultEndpointsProtocol="http";AccountName="tr15sharemyphoto";AccountKey="4yhu8YT3Y6A3do0s+anHFAX6ZUA11V2NJJNhjmJc0iAgSLW8Xwk3QvVQn2Um+hgMmO+vGf0UFd2zOo8K63PD4w=="';


$blobRestProxy = ServicesBuilder::getInstance()->createBlobService($STORAGE_CONNECTION_STRING);
$tableRestProxy = ServicesBuilder::getInstance()->createTableService($STORAGE_CONNECTION_STRING);
$queueRestProxy = ServicesBuilder::getInstance()->createQueueService($STORAGE_CONNECTION_STRING);

function CreateQueueStorages()
{
	global $queueRestProxy;

	$createQueueOptions = new CreateQueueOptions();

//	$queueRestProxy->createQueue("tagger", $createQueueOptions);
	$queueRestProxy->createQueue("thumb", $createQueueOptions);

}

function CreateAppBlobStorage()
{
	global $blobRestProxy;

	echo "aaaaAAA";

	try 
	{
		$containerOpts = new CreateContainerOptions(); 
		$containerOpts->setPublicAccess(PublicAccessType::CONTAINER_AND_BLOBS);
		$blobRestProxy->createContainer("photos", $containerOpts);		

		$blobRestProxy->createContainer("thubnails", $containerOpts);		
	}

	catch(ServiceException $e)
	{
		echo "<pre>";
		print_r($e);
		echo "</pre>";

		echo "Error during creating container blob: 'photos'";
		exit();
	}

	echo "Photos Blob created";
 
}

function CreateCommentTable()
{
	global $tableRestProxy;

	echo "AAA";

	try
	{
		$tableRestProxy->createTable("photos");

	}

	catch(ServiceException $e)
	{

		echo "<pre>";
		print_r($e);
		echo "</pre>";

		echo "Error during creating container blob: 'photos'";
		exit();
	}

	echo "\nPhotos table created";
}

function InitStorage()
{
//	CreateAppBlobStorage();
//	CreateCommentTable();
	CreateQueueStorages();
}

InitStorage();



?>

