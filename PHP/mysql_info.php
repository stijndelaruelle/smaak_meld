<?php

//Error message
$errormsg = "<h1>Server error</h1><p>We will fix this problem as soon as possible!</p>";

// treatsart mysql connection
mysql_connect("localhost","blargal_main", "wafels") or die($errormsg);
mysql_select_db("blargal_main") or die($errormsg);

?>