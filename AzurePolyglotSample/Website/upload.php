<?php

require_once "init.php";

use WindowsAzure\Table\Models\Entity;
use WindowsAzure\Table\Models\EdmType;
use WindowsAzure\Queue\Models\CreateMessageOptions;





function getPostOrDefault($paramName, $def)
{
	$value = $def;

	if(isset($_POST[$paramName])) $value = $_POST[$paramName];

	return $value;
}

function AppendForTagging($id)
{
	global $queueRestProxy;
	$queueRestProxy->createMessage("tagger", $id);
}

function AppendForThumbnailing($id)
{
	global $queueRestProxy;
	$queueRestProxy->createMessage("thumb", $id);
}

function InsertRow($id)
{
	global $tableRestProxy;


	$entity = new Entity();
	
	$entity->setPartitionKey("pk");
	$entity->setRowKey($id);
	$entity->addProperty("title", EdmType::STRING, getPostOrDefault('title', ""));
	$entity->addProperty("keyword1", EdmType::STRING, getPostOrDefault('kw1', ""));
	$entity->addProperty("keyword2", EdmType::STRING, getPostOrDefault('kw2', ""));
	$entity->addProperty("keyword3", EdmType::STRING, getPostOrDefault('kw3', ""));
	$entity->addProperty("message", EdmType::STRING, getPostOrDefault('message', ""));
		

	try
	{
	    $tableRestProxy->insertEntity("photos", $entity);
	}
	catch(ServiceException $e)
	{
	    $code = $e->getCode();
	    $error_message = $e->getMessage();
//	    echo $code.": ".$error_message."<br />";
	}	
}

function InsertImage($id)
{
	global $blobRestProxy;
	global $_FILES;

	try
	{
		$content = fopen($_FILES["photo"]["tmp_name"], "r");
		$blobRestProxy->createBlockBlob("photos", $id, $content);
	}
	catch(ServiceExceptio $e)
	{
	    $code = $e->getCode();
	    $error_message = $e->getMessage();
	//    echo $code.": ".$error_message."<br />";
	}
}


if(isset($_POST["title"]) && (strlen($_POST['title']) > 0))
{
	$id = uniqid();
	$id = $id . substr($_FILES["photo"]["name"], strrpos($_FILES["photo"]["name"], "."));

	InsertRow($id);
	InsertImage($id);
	AppendForTagging($id);
	AppendForThumbnailing($id);

	echo '<html><head>';
	echo '<meta HTTP-EQUIV="REFRESH" content="0;url=index.php" />';
	echo '</head><body></body></html>'; 
	exit;
}
else
{
	require_once "templates/upload.php.inc";
}

?>