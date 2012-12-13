<!DOCTYPE html>
<html>
<head><title>Share my photos</title>
<link rel="stylesheet" href="stylesheets/style.css">
<script src="javascripts/jquery-1.7.min.js"></script>
</head>
<body>
<header>
<div class="content-wrapper">
<div class="float-left"><p class="site-title"><a href="/">Share my photos</a></p></div><nav><ul id="menu"><li> <a href="index.php">Photos</a></li><li> <a href="chat.php">Chat</a></li></ul></nav></div></div></header>
<div id="body"><section class="featured"><div class="content-wrapper"><hgroup class="title"><h1>Photos.  </h1><h2>Use this page to see the photos that have been shared</h2></hgroup></div></section><section class="main-content">
<a href="upload.php">Upload image</a>
<br />
<br />
<table>
<tr>
<th>Name</th>
<th>Keywords</th>
<th></th>
</tr>
<?php

header("refresh:8;url=./index.php");
require_once "init.php";

$filter = "";

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


foreach($entities as $entity){
	echo '<tr><td>';
	echo '<a href="view.php?id=' . $entity->getPropertyValue("RowKey") . '">';
	echo $entity->getPropertyValue("title");
	echo '</a></td>';
	echo '<td>';
	echo $entity->getPropertyValue("keyword1");
	echo ' ';
	echo $entity->getPropertyValue("keyword2");
	echo ' ';
	echo $entity->getPropertyValue("keyword3");
	echo '</td><td><a href="delete.php?id=' . $entity->getPropertyValue("RowKey") . '">Delete</a>';

	echo "</td></tr>";
}
?>
</table>
</section></div><footer><div class="content-wrapper"><div class="float-left"><p>&copy; 2012</p></div><div class="float-right"><ul id="social"><li><a href="http://facebook.com" class="facebook">Facebook</a><a href="http://twitter.com" class="twitter">Twitter</a></li></ul></div></div></footer></body></html>
