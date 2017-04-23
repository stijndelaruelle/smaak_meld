<?php 
    require("mysql_info.php");
    require("utility.php");

    //Strings must be escaped to prevent SQL injection attack. 
    $name = mysql_real_escape_string($_GET['n']);
    $password = $_GET['p'];
    $hash = $_GET['h']; 

    $error = "";
    if(CheckHash($name, $password, $hash, $error))
    {
        $user = CheckLogin($name, $password, $error);
        if ($user != null)
        {
            //Update the database
            $timestamp = date("Y-m-d H:i:s");

            mysql_query("UPDATE Flavour_Users SET last_login_timestamp='$timestamp' WHERE name='$name'")
            or die($error .= "E_Query failed: " . mysql_error() . "\n");
        }
    }

    echo $error;
?>