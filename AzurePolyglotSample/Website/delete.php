<?php

require_once "init.php";

$filter = "RowKey eq '" . $_GET["id"] . "'";

try
{
    $result = $tableRestProxy->queryEntities("photos", $filter);
}

catch(ServiceException $e){
    $code = $e->getCode();
    $error_message = $e->getMessage();
    echo $code.": ".$error_message."<br />";
}

$entities = $result->getEntities();


foreach($entities as $entity)
{
	$pk = $entity->getPartitionKey();
	$rowKey = $entity->getRowKey();

	try
	{
		$tableRestProxy->deleteEntity("photos", $pk, $rowKey);
	}
	catch(Exception $e) {;}

	try
	{
		$blobRestProxy->deleteBlob("photos", $pk);
	}
	catch(Exception $e) {;}

}


	header("refresh:1;url=./index.php")


?>

