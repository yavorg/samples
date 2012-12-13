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
<div id="body"><section class="featured"><div class="content-wrapper"><hgroup class="title"><h1>Upload.  </h1><h2>Pick the image to share</h2></hgroup></div></section><section class="main-content">

<form method="POST" enctype="multipart/form-data">
<label>Title:</label><input name="title" type="text" value=""><br />
<label>Keyword 1:</label><input name="kw1" type="text" value="" /><br />
<label>Keyword 2:</label><input name="kw2" type="text" value="" /><br />
<label>Keyword 3:</label><input name="kw3" type="text" value="" /><br />
<lable>Description:</lable><textarea name="message" ></textarea><br />
<input type="file" name="photo" />
<button type="submit">Sumbit</button>
</form>
</section></div><footer><div class="content-wrapper"><div class="float-left"><p>&copy; 2012</p></div><div class="float-right"><ul id="social"><li><a href="http://facebook.com" class="facebook">Facebook</a><a href="http://twitter.com" class="twitter">Twitter</a></li></ul></div></div></footer></body></html>
