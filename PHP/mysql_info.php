<?php

//Error message
$errormsg = "<h1>Server error</h1><p>We will fix this problem as soon as possible!</p>";

// treatsart mysql connection
mysql_connect("localhost","xxxxxxxx", "xxxx") or die($errormsg);
mysql_select_db("xxxxxxxx") or die($errormsg);

?>